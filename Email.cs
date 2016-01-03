using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.IO;


namespace CampusAssist
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

    public partial class Email : Form
    {
        private string userName = "";           //用户名
        private string pwd = "";                //密码
        mail[] mailList = null;                 //存储邮件
        int all_num=0;                            //目前所有邮件数量
        int new_num=0;                            //新的邮件数量
        public Email()
        {
            InitializeComponent();
        }

        private void Email_Load(object sender, EventArgs e)
        {
        }

        public void getEmail(int emailNum)         //获取email_num封邮件，将邮件总数保存到all_num
        {
            Chilkat.MailMan mailman = new Chilkat.MailMan();
            bool success = mailman.UnlockComponent("30-day trial");
            if (success != true)
            {
                Console.WriteLine("Component unlock failed");
                return;
            }
            mailman.MailHost = "smail.ecnu.edu.cn";
            mailman.PopUsername = userName;
            mailman.PopPassword = pwd;

            Chilkat.EmailBundle bundle = null;
            bundle = mailman.CopyMail();
            if (bundle == null)
            {
                Console.WriteLine(mailman.LastErrorText);
                return;
            }
            mailList = new mail[emailNum];
            int count = 0;
            Chilkat.Email email = null;
            for (int i = bundle.MessageCount - 1; i >= bundle.MessageCount - 1 - emailNum; i--)
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

            all_num = bundle.MessageCount;

            return;
        }

        public void getNew(int emailNum)            //获取emailNum封邮件，如果邮件总数增加，将增加数保存到new_num
        {
            int oldNum = all_num;
            getEmail(emailNum);
            if(all_num >= oldNum)
            {
                new_num = all_num - oldNum;
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            userName = "10142510186";
            pwd = "payon951224";
            getEmail(10);
            MessageBox.Show(getBody(2));
        }
    }

 
}
