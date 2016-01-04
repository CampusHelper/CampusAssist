using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CampusAssist
{
    class POP3Mail
    {
        struct mail                             //邮件结构体
        {
            public string fromAddress;          //发信地址
            public string fromName;             //发信人
            public string subject;              //主题
            public string body;                 //正文
            public string time;                 //日期 时间
            public bool read;                   //已读/未读
        };

        Chilkat.MailMan mailman;
        private string userName = "";           //用户名
        private string pwd = "";                //密码
        mail[] mailList = null;                 //存储邮件
        int all_num = 0;                        //目前所有邮件数量
        int new_num = 0;                        //新的邮件数量
        public POP3Mail()
        {
            mailman = new Chilkat.MailMan();
        }
        public void getEmail(int emailNum)         //获取email_num封邮件，将邮件总数保存到all_num
        {
            
            bool success = mailman.UnlockComponent("30-day trial");
            if (success != true)
            {
                Console.WriteLine("Component unlock failed");
                return;
            }
            mailman.MailHost = "smail.ecnu.edu.cn";
            mailman.PopUsername = userName;
            mailman.PopPassword = pwd;
            

            Chilkat.StringArray saUidls = null;
            //  Get the complete list of UIDLs
            saUidls = mailman.GetUidls();

            if (saUidls == null)
            {
                Console.WriteLine(mailman.LastErrorText);
                return;
            }

            //  Get the 10 most recent UIDLs
            //  The 1st email is the oldest, the last email is the newest
            //  (usually)
            int i;
            int n;
            int startIdx;
            //n = saUidls.Count;
            n = mailman.GetMailboxCount();
            if (n > emailNum)
            {
                startIdx = n - emailNum;
            }
            else
            {
                startIdx = 0;
            }

            Chilkat.StringArray saUidls2 = new Chilkat.StringArray();
            for (i = startIdx; i <= n - 1; i++)
            {
                saUidls2.Append(saUidls.GetString(i)/*string.Format("{0}@ecnu.cn", i + 2)*/);
            }

            //  Download in full the 10 most recent emails:
            Chilkat.EmailBundle bundle = null;

            bundle = mailman.FetchMultiple(saUidls2);
            if (bundle == null)
            {
                Console.WriteLine(mailman.LastErrorText);
                return;
            }

            Chilkat.Email email = null;
            int count = 0;
            mailList = new mail[bundle.MessageCount];
            for (i = 0; i < bundle.MessageCount; i++)
            {
                email = bundle.GetEmail(i);
                DateTime time = email.LocalDate;
                string timeStr = time.ToString();
                mailList[count].fromAddress = email.FromAddress;
                mailList[count].fromName = email.FromName;
                mailList[count].subject = email.Subject;
                mailList[count].time = timeStr;
                mailList[count].body = email.Body;
                count++;

            }

            mailman.Pop3EndSession();

            all_num = n;

            return;
        }

        public int getNew(int emailNum)            //获取emailNum封邮件，如果邮件总数增加，将增加数保存到new_num
        {
            int oldNum = all_num;
            getEmail(emailNum);
            if (all_num >= oldNum)
            {
                new_num = all_num - oldNum;
                return new_num;
            }
            return 0;
        }

        public string getFromAddress(int index)     //获取第index封邮件的发信地址
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return null;
            }
            return mailList[index].fromAddress;
        }

        public string getFromName(int index)     //获取第index封邮件的发信人
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return null;
            }
            return mailList[index].fromName;
        }

        public string getSubject(int index)     //获取第index封邮件的主题
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return null;
            }
            return mailList[index].subject;
        }

        public string getBody(int index)     //获取第index封邮件的正文
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return null;
            }
            return mailList[index].body;
        }

        public string getTime(int index)     //获取第index封邮件的发信 日期 时间
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return null;
            }
            return mailList[index].time;
        }

        public bool getRead(int index)     //获取第index封邮件已读/未读
        {
            if (index >= mailList.Length)
            {
                MessageBox.Show("Out of range!");
                return false;
            }
            return mailList[index].read;
        }

        public void setAccount(string _username, string _password)
        {
            userName = _username;
            pwd = _password;
        }

    }
}
