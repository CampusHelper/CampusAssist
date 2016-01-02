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
            int i = emailNum.IndexOf('（') + 1;
            emailNum = emailNum.Substring(i);
            i = emailNum.IndexOf('）');
            emailNum = emailNum.Substring(0, i);

            re = new Regex(@"未读邮件（\d{0,4}）");
            string unreadNum = re.Match(htmlstr).Value;
            i = unreadNum.IndexOf('（') + 1;
            unreadNum = unreadNum.Substring(i);
            i = unreadNum.IndexOf('）');
            unreadNum = unreadNum.Substring(0, i);

            string[] rtnStr = new string[2];
            rtnStr[0] = emailNum;
            rtnStr[1] = unreadNum;
            return rtnStr;                      // rtn[0]为 收件箱数量 rtn[1]为 未读邮件数量
        }
        /*
            获取姓名，校园卡状态，余额
        */
        public static string[] getBalance(string htmlstr)
        {
            string[] rtnStr = new string[2];
            Regex re = new Regex(@"<span id=""lblCustInfo"">.{0,20}元");
            string status = re.Match(htmlstr).Value;
            int i = status.IndexOf('>') + 1;
            status = status.Substring(i);      // 卡的状态和余额

            i = status.IndexOf("：") + 1;
            status = status.Substring(i);
            i = status.IndexOf(" ");
            rtnStr[0] = status.Substring(0,i);
            i = status.IndexOf("：") + 1;
            status = status.Substring(i);
            i = status.IndexOf("元");
            rtnStr[1] = status.Substring(0, i);
            return rtnStr;                      // rtn[0]状态   rtn[1]余额
        }
        /*
           学生专栏通知公告
       */
        public static string[] getAnnounce(string htmlstr)
        {
            Regex re = new Regex(@"target=_blank title="".{0,50}"">");
            int num = re.Matches(htmlstr).Count;
            if (num > 7)
                num = 7;
            string[] rtnStr = new string[num];
            for (int n = 0; n < num; n++)                   // title
            {
                MatchCollection mc = re.Matches(htmlstr);
                rtnStr[n] = mc[n].Value;
                int i = rtnStr[n].IndexOf('"') + 1;
                rtnStr[n] = rtnStr[n].Substring(i);
                i = rtnStr[n].IndexOf('"');
                rtnStr[n] = rtnStr[n].Substring(0, i);
            }

            re = new Regex(@"<div style='white-space:nowrap'>.{0,15}</div>");
            string[] tmp = new string[num];
            for (int n = 0; n < num; n++)                   // time
            {
                MatchCollection mc = re.Matches(htmlstr);
                tmp[n] = mc[n].Value;
                int i = tmp[n].IndexOf('>') + 1;
                tmp[n] = tmp[n].Substring(i);
                i = tmp[n].IndexOf('<');
                tmp[n] = tmp[n].Substring(0, i);
                rtnStr[n] += " " + tmp[n];
            }

            re = new Regex(@"<a href='.{0,40}' target=_blank");
            tmp = new string[num];
            for (int n = 0; n < num; n++)                   //url example:/s/110/t/486/1e/45/info138821.htm
            {
                MatchCollection mc = re.Matches(htmlstr);
                tmp[n] = mc[n].Value;
                int i = tmp[n].IndexOf('\'') + 1;
                tmp[n] = tmp[n].Substring(i);
                i = tmp[n].IndexOf('\'');
                tmp[n] = tmp[n].Substring(0, i);
                rtnStr[n] += " " + tmp[n];
            }

            return rtnStr;                     // 每个string为 通知标题+空格+发布时间+空格+url
        }

        /*
           获取考试安排
       */
        public static string[] getExam(string htmlstr)
        {
            Regex re = new Regex(@"height=""23px"">([\s]*)?<td>.{17}</td>([\s]*)?<td>\w{0,20}</td>");
            MatchCollection mc = re.Matches(htmlstr);
            int num = mc.Count;
            string[] rtnStr = new string[num];
            for (int n = 0; n < num; n++)
            {
                string tmp = mc[n].Value;
                int i = tmp.IndexOf("<td>") + 4;
                string id = tmp.Substring(i, 17);
                rtnStr[n] += id;
                tmp = tmp.Substring(i);

                i = tmp.IndexOf("<td>") + 4;
                tmp = tmp.Substring(i);
                i = tmp.IndexOf('<');
                tmp = tmp.Substring(0, i);
                rtnStr[n] += " " + tmp;

                i = htmlstr.IndexOf(tmp + "</td>");
                tmp = htmlstr.Substring(i);
                i = tmp.IndexOf("<td");

                if (tmp.Substring(i, 4) == "<td ")
                {
                    rtnStr[n] += " [考试情况尚未发布]";
                }
                else
                {
                    re = new Regex(@"<td>\d{4}-\d{2}-\d{2}</td>");
                    string token = re.Match(tmp).Value;
                    i = token.IndexOf('>') + 1;
                    token = token.Substring(i);
                    i = token.IndexOf('<');
                    token = token.Substring(0, i);
                    rtnStr[n] += " " + token;

                    re = new Regex(@"<td>第.{10,20}</td>");
                    token = re.Match(tmp).Value;
                    i = token.IndexOf('>') + 1;
                    token = token.Substring(i);
                    i = token.IndexOf('<');
                    token = token.Substring(0, i);
                    rtnStr[n] += " ";
                    for (int j = 0; j < token.Length; j++)
                    {
                        if (token[j] != ' ')
                            rtnStr[n] += token[j];
                    }

                    re = new Regex(@"<a href=.+>\w{0,20}</a>");
                    token = re.Match(tmp).Value;
                    i = token.IndexOf('>') + 1;
                    token = token.Substring(i);
                    i = token.IndexOf('<');
                    token = token.Substring(0, i);
                    rtnStr[n] += " " + token;


                    re = new Regex(@"<td>\w+</td>");
                    token = re.Match(tmp).Value;
                    i = token.IndexOf('>') + 1;
                    token = token.Substring(i);
                    i = token.IndexOf('<');
                    token = token.Substring(0, i);
                    rtnStr[n] += " " + token;

                    re = new Regex(@"<td>\d+</td>");
                    token = re.Match(tmp).Value;
                    i = token.IndexOf('>') + 1;
                    token = token.Substring(i);
                    i = token.IndexOf('<');
                    token = token.Substring(0, i);
                    rtnStr[n] += " " + token;
                }
            }


            return rtnStr;              // 课程编号 课程名称 考试日期 考试时间 考试地点 考试情况 座位号
                                        // 或 课程编号 课程名称 [考试情况尚未发布]
        }
        /*
            校园卡消费记录
        */
        public static string[] getConsume(string htmlstr)
        {
            Regex re = new Regex(@"style=""width:138px;"">.{10,20}</span>");
            MatchCollection mc = re.Matches(htmlstr);
            int num = mc.Count;
            string[] rtnStr = new string[num];
            for (int n = 0; n < num; n++)
            {
                string tmp = mc[n].Value;
                int i = tmp.IndexOf('>') + 1;
                tmp = tmp.Substring(i);
                i = tmp.IndexOf('<');
                tmp = tmp.Substring(0, i);
                rtnStr[n] = tmp;            //交易日期 交易时间

                i = htmlstr.IndexOf(tmp);
                tmp = htmlstr.Substring(i);
                re = new Regex(@"style=""width:64px;"">\w{0,20}</span>");
                string token = re.Match(tmp).Value;
                i = token.IndexOf('>') + 1;
                token = token.Substring(i);
                i = token.IndexOf('<');
                token = token.Substring(0, i);
                rtnStr[n] += " " + token;         //交易名称

                re = new Regex(@">￥\d+.\d+</span>");
                token = re.Match(tmp).Value;
                i = token.IndexOf('>') + 1;
                token = token.Substring(i);
                i = token.IndexOf('<');
                token = token.Substring(0, i);
                rtnStr[n] += " " + token;         //交易金额

                re = new Regex(@"cc"">￥\d+.\d+</span>");
                token = re.Match(tmp).Value;
                i = token.IndexOf('>') + 1;
                token = token.Substring(i);
                i = token.IndexOf('<');
                token = token.Substring(0, i);
                rtnStr[n] += " " + token;         //余额

                re = new Regex(@"style=""width:100px;"">.{0,20}</span>");
                token = re.Match(tmp).Value;
                i = token.IndexOf('>') + 1;
                token = token.Substring(i);
                i = token.IndexOf('<');
                token = token.Substring(0, i);
                rtnStr[n] += " " + token;         //刷卡地点

                re = new Regex(@"Label11"">\d+</span>");
                token = re.Match(tmp).Value;
                i = token.IndexOf('>') + 1;
                token = token.Substring(i);
                i = token.IndexOf('<');
                token = token.Substring(0, i);
                rtnStr[n] += " " + token;         //卡操作计数
            }
            return rtnStr;                         //交易日期 交易时间 交易名称 交易金额 余额 刷卡地点 卡操作计数
        }
    }
}
