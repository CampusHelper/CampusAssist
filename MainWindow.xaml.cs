using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CampusAssist
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebProcess web;
        bool onInit;
        string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        byte[,] color = { { 96, 192, 230 },
                        { 147, 209, 98 },
                        { 136, 172, 234 },
                        { 117, 192, 172 },
                        { 226, 151, 181 },
                        { 132, 216, 164 },
                        { 130, 211, 212 },
                        { 245, 172, 139} };
        int[,] semesterId = new int[,] { { 449, 481 }, { 385, 417 }, { 321, 353 }, { 89, 288 } };
        int[] examType = new int[] { 1, 3, 4, 2, 100 };
        System.Windows.Forms.NotifyIcon notify;

        private string username;
        private string password;
        POP3Mail mail;
        bool mailock;
        int recentMailCnt = 10;
        public MainWindow(WebProcess _web, string _username, string _password)
        {
            onInit = true;
            web = _web;
            username = _username;
            password = _password;
            mail = new POP3Mail();
            mail.setAccount(username, password);
            mailock = false;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* 一以下最好开新的线程处理否则会阻塞 */
            Thread oThread = new Thread(init);
            oThread.Start();
            /* 以上 */
            dateLbl.Content = DateTime.Now.ToLongDateString().ToString() + " " + weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            notify = new System.Windows.Forms.NotifyIcon();
            notify.Icon = new System.Drawing.Icon("icon.ico");
            notify.Click += new EventHandler(reSize);
            onInit = false;

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                string page;
                switch (tabControl.SelectedIndex)
                {
                    // 教务通知
                    case 1:
                        announcementGrid.Children.RemoveRange(0, announcementGrid.Children.Count);
                        // 抓整个网页已经封装宰了web.getDocument中 直接调用即可获取整个网页
                        page = web.getDocument("http://www.jwc.ecnu.edu.cn/", Encoding.UTF8);

                        // 用已经实现好的HtmlParse中的相关函数获取需要的信息, 请参考HtmlParse
                        string[] info = HtmlParse.getAnnounce(page);

                        // 动态向announcementGrid添加元素
                        for (int i = 0; i < info.Length; i++)
                        {
                            Label lbl = new Label();
                            string[] cur = info[i].Split(' ');
                            lbl.Content = cur[0];

                            // 添加新的一行
                            RowDefinition rd = new RowDefinition();
                            announcementGrid.RowDefinitions.Add(rd);
                            // 添加元素
                            announcementGrid.Children.Add(lbl);
                            //指定行与列
                            Grid.SetRow(lbl, i);
                            Grid.SetColumn(lbl, 0);
                            lbl = new Label();
                            lbl.Foreground = Brushes.Blue;
                            lbl.Content = cur[1];
                            announcementGrid.Children.Add(lbl);
                            Grid.SetRow(lbl, i);
                            Grid.SetColumn(lbl, 1);
                        }
                        break;
                    // 课程表
                    case 3:
                        semesterClassChanged(null, null);
                        break;
                    // 成绩
                    case 4:
                        semesterChanged(null, null);
                        break;

                    // 考试安排
                    case 5:
                        semesterTimeChanged(null, null);
                        break;

                    // 校园卡信息
                    case 6:
                        Thread balanceThread = new Thread(updateBalanceRecord);
                        balanceThread.Start();
                        break;
                }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                notify.Visible = true;
                notify.BalloonTipText = "ECNU校园助手已隐藏到托盘";
                notify.BalloonTipTitle = "ECNU校园助手";
                notify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                notify.ShowBalloonTip(1000);
            }
        }

        private void reSize(object sender, EventArgs e)
        {
            notify.Visible = false;
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }

        private void semesterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!onInit)
            {
                examGrid.Children.RemoveRange(6, examGrid.Children.Count - 6);
                int semeId = 0;
                semeId = semesterId[yearCombo.SelectedIndex, semesterCombo.SelectedIndex];
                string url = String.Format("http://applicationnewjw.ecnu.edu.cn/eams/teach/grade/course/person!search.action?semesterId={0}", semeId);
                string page = web.getDocument(url, Encoding.UTF8);
                string[] exam = HtmlParse.getScores(page);
                for (int i = 0; i < exam.Length; i++)
                {
                    string[] cur = exam[i].Split(' ');
                    examGrid.RowDefinitions.Add(new RowDefinition());
                    for (int j = 0; j < cur.Length; j++)
                    {
                        Label lbl = new Label();
                        lbl.FontSize = 16;
                        lbl.Content = cur[j];
                        examGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, i + 1);
                        Grid.SetColumn(lbl, j);
                    }
                }
            }
        }

        private void semesterTimeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!onInit)
            {
                examTimeGrid.Children.RemoveRange(6, examTimeGrid.Children.Count - 6);
                int semeId = semesterId[yearExamCombo.SelectedIndex, semesterExamCombo.SelectedIndex];
                int typeId = examType[typeExamCombo.SelectedIndex];
                string url = String.Format("http://applicationnewjw.ecnu.edu.cn/eams/stdExamTable!examTable.action?semester.id={0}&examType.id={1}", semeId, typeId);
                string page = web.getDocument(url, Encoding.UTF8);
                string[] exam = HtmlParse.getExam(page);
                for (int i = 0; i < exam.Length; i++)
                {
                    string[] cur = exam[i].Split(' ');
                    examTimeGrid.RowDefinitions.Add(new RowDefinition());
                    for (int j = 1; j < cur.Length; j++)
                    {
                        Label lbl = new Label();
                        lbl.FontSize = 16;
                        if (cur[j].Contains("尚未发布"))
                        {
                            cur[j] = "[未发布]";
                        }
                        lbl.Content = cur[j];
                        examTimeGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, i + 1);
                        Grid.SetColumn(lbl, j - 1);
                    }
                }
            }
        }

        private void semesterClassChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!onInit)
            {
                classGrid.Children.RemoveRange(17, classGrid.Children.Count - 17);

                int semeId = semesterId[yearClassCombo.SelectedIndex, semesterClassCombo.SelectedIndex];
                int ids = 260366;
                int week = weekCombo.SelectedIndex + 1;
                string url = String.Format("http://applicationnewjw.ecnu.edu.cn/eams/courseTableForStd!courseTable.action?ignoreHead=1&setting.kind=std&semester.id={0}&ids={1}&startWeek={2}", semeId, ids, week);
                string page = web.getDocument(url, Encoding.UTF8);
                string[] schedule = HtmlParse.getSchedule(page, week);
                for (int i = 0; i < schedule.Length; i++)
                {
                    string[] cur = schedule[i].Split('|');
                    if (cur.Length == 7) continue;
                    TextBlock lbl = new TextBlock();
                    lbl.Text = string.Format("{0}\n({1} {2})", cur[0], cur[1], cur[2]);
                    lbl.FontSize = 20;
                    lbl.Background = new SolidColorBrush(Color.FromRgb(color[i % 8, 0], color[i % 8, 1], color[i % 8, 2]));
                    lbl.TextWrapping = TextWrapping.Wrap;
                    classGrid.Children.Add(lbl);
                    int y = int.Parse(cur[3]);
                    int x = int.Parse(cur[4]);
                    int d = int.Parse(cur[5]);
                    Grid.SetRow(lbl, x);
                    Grid.SetColumn(lbl, y);
                    Grid.SetRowSpan(lbl, d);
                }
            }
        }

        private void init()
        {
            string mainPage = web.doRedict();           // 首次进入需要重定向一次
            string[] info = HtmlParse.getInfo(mainPage);
            Dispatcher.Invoke((Action)delegate
            {
                nameLbl.Content = info[0];
                ipLbl.Content = info[1];
            });
            string html = web.getDocument("http://applicationidc.ecnu.edu.cn/ecnuidc/sso/ssoemailchh.jsp", Encoding.Default);
            info = HtmlParse.getEmail(html);
            Dispatcher.Invoke((Action)delegate
            {
                mailCntLbl.Content = info[0];
                unreadCntLbl.Content = info[1];
            });
            string balance = getBalance();
            if (double.Parse(balance) < 10)
            {
                Dispatcher.Invoke((Action)delegate
                {
                    balanceMainLbl.Content = "你的校园卡余额不足10元，请及时充值！";
                });
            }
            //mail.getEmail(10);
        }

        private string getBalance()
        {
            string page = web.getDocument("http://portal.ecnu.edu.cn/eapdomain/neudcp/sso/sso_ecard_xxcx.jsp", Encoding.Default);
            string[] bal = HtmlParse.getBalance(page);
            return bal[1];
        }

        private void updateBalanceRecord()
        {
            string page = web.getDocument("http://portal.ecnu.edu.cn/eapdomain/neudcp/sso/sso_ecard_xxcx.jsp", Encoding.Default);
            string[] bal = HtmlParse.getBalance(page);
            Dispatcher.Invoke((Action)delegate
            {
                balanceLbl.Content = bal[1];
                statusLbl.Content = bal[0];
            });

            page = web.getDocument("http://www.ecard.ecnu.edu.cn/Ecard/cqmoney.aspx", Encoding.Default);
            string[] consume = HtmlParse.getConsume(page);
            Dispatcher.Invoke((Action)delegate
            {
                recordProcessLbl.Content = "[正在获取中...]";
            });
            Dispatcher.Invoke((Action)delegate
            {
                costGrid.Children.RemoveRange(6, costGrid.Children.Count - 6);
                for (int i = 0; i < consume.Length; i++)
                {
                    string[] cur = consume[i].Split(' ');
                    costGrid.RowDefinitions.Add(new RowDefinition());
                    for (int j = 0; j < 6; j++)
                    {
                        Label lbl = new Label();
                        lbl.FontSize = 16;
                        lbl.Content = cur[j];
                        costGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, i + 1);
                        Grid.SetColumn(lbl, j);
                    }
                }
                recordProcessLbl.Content = "";
            });

        }

        // 刷新邮件
        private void refreshMail()
        {
            if (!mailock  /*mail.getNew(10) > 0*/)
            {
                mailock = true;
                mail.getEmail(recentMailCnt);
                updateMailList();
                mailock = false;
            }
        }

        // 更新邮箱列表
        private void updateMailList()
        {
            Dispatcher.Invoke((Action)delegate
            {
                mailGrid.Children.RemoveRange(3, mailGrid.Children.Count - 3);
            });
            for (int i = 0; i < recentMailCnt; i++)
            {
                Dispatcher.Invoke((Action)delegate
                {
                    mailGrid.RowDefinitions.Add(new RowDefinition());
                    Label lbl = new Label();
                    lbl.Content = mail.getFromName(i);
                    lbl.FontSize = 16;
                    mailGrid.Children.Add(lbl);
                    Grid.SetRow(lbl, recentMailCnt - i);
                    Grid.SetColumn(lbl, 0);

                    Button btn = new Button();
                    btn.Tag = i;
                    btn.FontSize = 16;
                    btn.Click += new RoutedEventHandler(onSubject);
                    btn.Content = mail.getSubject(i);
                    mailGrid.Children.Add(btn);
                    Grid.SetRow(btn, recentMailCnt - i);
                    Grid.SetColumn(btn, 1);

                    lbl = new Label();
                    lbl.FontSize = 16;
                    lbl.Content = mail.getTime(i);
                    mailGrid.Children.Add(lbl);
                    Grid.SetRow(lbl, recentMailCnt - i);
                    Grid.SetColumn(lbl, 2);
                });
            }

        }

        private void onMail(object sender, RoutedEventArgs e)
        {
            refreshMailBtn.IsEnabled = false;
            refreshMail();
            refreshMailBtn.IsEnabled = true;
        }

        private void onSubject(object sender, RoutedEventArgs e)
        {
            int d = (int)((Button)sender).Tag;
            senderLbl.Content = string.Format("{0}({1})", mail.getFromName(d), mail.getFromAddress(d));
            timeLbl.Content = mail.getTime(d);
            themeLbl.Content = mail.getSubject(d);
            bodyLbl.Content = mail.getBody(d);
        }
    }
}
