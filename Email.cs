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
    public partial class Email : Form
    {    
        public TcpClient Server;
        public NetworkStream NetStrm;
        public StreamReader RdStrm;
        public string Data;
        public byte[] szData;
        public string CRLF = "\r\n";

        public Email()
        {
            InitializeComponent();
        }

        private void Username_TextChanged(object sender, EventArgs e)
        {}

        private void Connect_Click(object sender, System.EventArgs e)
        {
            //将光标置为等待状态 
            Cursor cr = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //用110端口新建POP3服务器连接 
            Server = new TcpClient("pop.163.com", 110);
            Status.Items.Clear();

            try
            {
                //初始化 
                NetStrm = Server.GetStream();
                RdStrm = new StreamReader(Server.GetStream());
                Status.Items.Add(RdStrm.ReadLine());

                //登录服务器过程 
                Data = "USER " + Username.Text + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                Status.Items.Add(RdStrm.ReadLine());

                Data = "PASS " + Password.Text + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                Status.Items.Add(RdStrm.ReadLine());

                //向服务器发送STAT命令，从而取得邮箱的相关信息：邮件数量和大小 
                Data = "STAT" + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                Status.Items.Add(RdStrm.ReadLine());

                //改变按钮的状态 
                Connect.Enabled = false;
                Disconnect.Enabled = true;
                Retrieve.Enabled = true;

                //将光标置回原来的状态 
                Cursor.Current = cr;

            }
            catch (InvalidOperationException err)
            {
                Status.Items.Add("Error: " + err.ToString());
            }
        }

        private void Email_Load(object sender, EventArgs e)
        {}

        private void Disconnect_Click(object sender, EventArgs e)
        {
            //将光标置为等待状态 
            Cursor cr = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //向服务器发送QUIT命令从而结束和POP3服务器的会话 
            Data = "QUIT" + CRLF;
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            NetStrm.Write(szData, 0, szData.Length);
            Status.Items.Add(RdStrm.ReadLine());

            //断开连接 
            NetStrm.Close();
            RdStrm.Close();

            //改变按钮的状态 
            Connect.Enabled = true;
            Disconnect.Enabled = false;
            Retrieve.Enabled = false;

            //将光标置回原来的状态 
            Cursor.Current = cr; 
        }

        private void Retrieve_Click(object sender, EventArgs e)
        {
            //将光标置为等待状态 
            Cursor cr = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            string szTemp;
            Message.Clear();

            try
            {
                //根据邮件编号从服务器获得相应邮件 
                Data = "RETR " + MailNum.Text + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                szTemp = RdStrm.ReadLine();

                if (szTemp[0] != '-')
                {
                    //不断地读取邮件内容，只到结束标志：英文句号 
                    while (szTemp != ".")
                    {
                        Message.Text += szTemp;
                        szTemp = RdStrm.ReadLine();
                    }

                    //若BackupChBox未选中，则收取邮件后，删除保留在服务器上的邮件 
                   /* if (BackupChBox.Checked == false)
                    {
                        Data = "DELE" + MailNum.Text + CRLF;
                        szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                        NetStrm.Write(szData, 0, szData.Length);
                        Status.Items.Add(RdStrm.ReadLine());
                    }*/
                }
                else
                {
                    Status.Items.Add(szTemp);
                }

                //将光标置回原来的状态 
                Cursor.Current = cr;
            }
            catch (InvalidOperationException err)
            {
                Status.Items.Add("Error: " + err.ToString());
            }
        }



    }

 
}
