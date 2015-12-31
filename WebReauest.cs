using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CampusAssist
{
    class WebReauest
    {
        /// <summary>  
        /// 通过get方式请求页面，传递一个实例化的cookieContainer  
        /// </summary>  
        /// <param name="postUrl"></param>  
        /// <param name="cookie"></param>  
        /// <returns></returns>  
        public static ArrayList GetHtmlData(string postUrl, CookieContainer cookie)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            ArrayList list = new ArrayList();
            request = WebRequest.Create(postUrl) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0";
            request.CookieContainer = cookie;
            request.KeepAlive = true;

            request.CookieContainer = cookie;
            try
            {
                //获取服务器返回的资源  
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        cookie.Add(response.Cookies);
                        //保存Cookies  
                        list.Add(cookie);
                        list.Add(reader.ReadToEnd());
                        list.Add(Guid.NewGuid().ToString());//图片名  
                    }
                }
            }
            catch (WebException ex)
            {
                list.Clear();
                list.Add("发生异常/n/r");
                WebResponse wr = ex.Response;
                using (Stream st = wr.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(st, System.Text.Encoding.Default))
                    {
                        list.Add(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                list.Clear();
                list.Add("5");
                list.Add("发生异常：" + ex.Message);
            }
            return list;
        }

    }
}
