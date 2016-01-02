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
    class webProcess
    {
        CookieContainer cookies;
        string requestUA = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; BOIE9;ZHCN)";
        public webProcess(ref System.Windows.Controls.Image img)
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
            string redict;
            if (loginSuccess(response,out redict))
            {
                ret = true;
            }
            return ret;
        }

        private string responseToString(HttpWebResponse res, Encoding encoding)
        {
            StreamReader r = new StreamReader(res.GetResponseStream(), encoding);
            string html = r.ReadToEnd();
            return html;
        }

        private string getCASTicket(HttpWebResponse res)
        {
            string html = responseToString(res, Encoding.Default);
            int start = html.IndexOf("LT_");
            int end = html.IndexOf("\"", start);
            string ticket = html.Substring(start, end - start);
            return ticket;
        }
        private bool loginSuccess(HttpWebResponse res,out string redict)
        {
            string html = responseToString(res, Encoding.Default);
            int start = html.IndexOf("http://");
            int end = html.IndexOf("\"", start);
            redict = html.Substring(start, end - start);
            return html.IndexOf("LT_") < 0;
        }

        public static BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
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
    }
}
