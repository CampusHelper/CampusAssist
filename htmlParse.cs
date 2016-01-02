using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CampusAssist
{
    class HtmlParse
    {
        /*
          获取姓名，IP，收件箱数量，未读邮件数量
        */
        static String getInfo(string htmlstr)          // 首页html
        {
            Regex re = new Regex(@"&#65281;\w{1,5}</font>");
            String name = re.Match(htmlstr).Value;
            int i = name.IndexOf(';') + 1;
            name = name.Substring(i);
            i = name.IndexOf('<');
            name = name.Substring(0, i);         // 用户姓名

            re = new Regex(@"IP:<br>.{0,15}</td>");
            String ip = re.Match(htmlstr).Value;
            i = ip.IndexOf('>') + 1;
            ip = ip.Substring(i);
            i = ip.IndexOf('<');
            ip = ip.Substring(0, i);             // 用户IP


            return name + " " + ip;
        }
        /*
            获取收件箱，未读邮件数量
        */
        static String getEmail(string htmlstr)
        {
            Regex re = new Regex(@"收件箱（\d{0,4}）");
            String emailNum = re.Match(htmlstr).Value;
            re = new Regex(@"未读邮件（\d{0,4}）");
            String unreadNum = re.Match(htmlstr).Value;
            return emailNum + " " + unreadNum;
        }
    }
}
