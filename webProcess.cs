using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CampusAssist
{
    public class WebProcess
    {
        CookieContainer cookies;
        const int MAX_REDICT = 10;
        string redict;
        string requestUA = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; BOIE9;ZHCN)";
        public WebProcess(ref System.Windows.Controls.Image img)
        {
            Uri captchaUri = new Uri("https://portal1.ecnu.edu.cn/cas/Captcha.jpg");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(captchaUri);
            request.Accept = "*/*";
            request.Method = "GET";
            request.UserAgent = requestUA;
            request.CookieContainer = new CookieContainer();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            cookies = request.CookieContainer;
            Stream resStream = response.GetResponseStream();
            Bitmap sourcebm = new Bitmap(resStream);
            img.Source = ChangeBitmapToBitmapSource(sourcebm);
        }

        //登录认证
        public bool login(string username, string password, string captcha)
        {
            // 获取验证码ticket
            bool ret = false;
            HttpWebRequest ticketRequest = (HttpWebRequest)WebRequest.Create("https://portal1.ecnu.edu.cn/cas/login?service=http%3A%2F%2Fportal.ecnu.edu.cn%2Fneusoftcas.jsp");
            //ticketRequest.ContentType = "application/x-www-form-urlencoded";
            //ticketRequest.AllowAutoRedirect = true;
            ticketRequest.CookieContainer = cookies;
            //ticketRequest.KeepAlive = true;
            string ticket = getCASTicket((HttpWebResponse)ticketRequest.GetResponse());

            // 登录
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal1.ecnu.edu.cn/cas/login?service=http%3A%2F%2Fportal.ecnu.edu.cn%2Fneusoftcas.jsp");
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = true;
            request.CookieContainer = cookies;
            request.KeepAlive = true;
            request.Method = "POST";

            string postData = string.Format("encodedService=http%253a%252f%252fportal.ecnu.edu.cn%252fneusoftcas.jsp&service=http%3A%2F%2Fportal.ecnu.edu.cn%2Fneusoftcas.jsp&serviceName=null&username={0}&password={1}&lt={2}&captcha={3}&Submit=%B5%C7%C2%BC",
                username, password, ticket, captcha);
            byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postdatabyte.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postdatabyte, 0, postdatabyte.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string html;
            if (needRedict(response, out redict, out html, Encoding.Default))
            {
                ret = true;
            }
            return ret;
        }

        private string responseToString(HttpWebResponse res, Encoding encoding)
        {
            cookies.Add(res.Cookies);
            StreamReader r = new StreamReader(res.GetResponseStream(), encoding);
            string html = r.ReadToEnd();
            return html;
        }

        // 获取CAS授权信息
        private string getCASTicket(HttpWebResponse res)
        {
            string html = responseToString(res, Encoding.Default);
            int start = html.IndexOf("LT_");
            int end = html.IndexOf("\"", start);
            string ticket = html.Substring(start, end - start);
            return ticket;
        }

        // 判断是否发生重定向
        private bool needRedict(HttpWebResponse res, out string redict, out string html, Encoding encoding)
        {
            html = responseToString(res, encoding);
            int start = html.IndexOf("http://");
            int end = html.IndexOf("\"", start);
            if (end < 0)
                end = html.IndexOf('\'', start);
            redict = html.Substring(start, end - start);

            return html.Contains(".location.href");
        }

        private static BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        // 【常用】获取指定url的全部页面
        public string getDocument(string url, Encoding encoding)
        {
            int cnt = 0;
            redict = url;
            string html;
            HttpWebResponse res = null;
            do
            {
                Uri captchaUri = new Uri(redict);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(captchaUri);
                request.Accept = "*/*";
                request.UserAgent = requestUA;
                request.CookieContainer = cookies;
                try
                {
                    res = (HttpWebResponse)request.GetResponse();
                }catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
            } while (needRedict(res, out redict, out html, encoding) && ++cnt <= MAX_REDICT);
            return html;
        }

        // 首次登录需要重定向一次
        public string doRedict()
        {
            return getDocument(redict, Encoding.UTF8);
        }
    }
}
