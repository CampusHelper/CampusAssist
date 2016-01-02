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
        public static string[] getInfo(string htmlstr)          // 首页html
        {
            Regex re = new Regex(@"&#65281;\w{1,5}</font>");
            string name = re.Match(htmlstr).Value;
            int i = name.IndexOf(';') + 1;
            name = name.Substring(i);
            i = name.IndexOf('<');
            name = name.Substring(0, i);         // 用户姓名

            re = new Regex(@"IP:<br>.{0,15}</td>");
            string ip = re.Match(htmlstr).Value;
            i = ip.IndexOf('>') + 1;
            ip = ip.Substring(i);
            i = ip.IndexOf('<');
            ip = ip.Substring(0, i);             // 用户IP

            string[] rtnStr = new string[2];
            rtnStr[0] = name;
            rtnStr[1] = ip;
            return rtnStr;                      // rtnStr[0]为用户名 [1]为IP
        }
        /*
            获取收件箱，未读邮件数量
        */
        public static string[] getEmail(string htmlstr)
        {
            Regex re = new Regex(@"收件箱（\d{0,4}）");
            string emailNum = re.Match(htmlstr).Value;
            re = new Regex(@"未读邮件（\d{0,4}）");
            string unreadNum = re.Match(htmlstr).Value;

            string[] rtnStr = new string[2];
            rtnStr[0] = emailNum;
            rtnStr[1] = unreadNum;
            return rtnStr;                      // rtn[0]为 "收件箱(XXX)" rtn[1]为 "未读邮件(XXX)"
        }
        /*
            获取姓名，校园卡状态，余额
        */
        public static string[] getBalance(string htmlstr)
        {
            Regex re = new Regex(@"<span id=""Header2_lblUser"" style=""color:OrangeRed;"">\w{1,5}</span>");
            string name = re.Match(htmlstr).Value;
            int i = name.IndexOf('>') + 1;
            name = name.Substring(i);
            i = name.IndexOf('<');
            name = name.Substring(0, i);         // 用户姓名

            re = new Regex(@"<span id=""lblCustInfo"">.{0,20}元");
            string status = re.Match(htmlstr).Value;
            i = status.IndexOf('>') + 1;
            status = status.Substring(i);      // 卡的状态和余额

            string[] rtnStr = new string[2];
            rtnStr[0] = name;
            rtnStr[1] = status;
            return rtnStr;                      // rtn[0]姓名   rtn[1]卡状态与余额
        }
    }
}
