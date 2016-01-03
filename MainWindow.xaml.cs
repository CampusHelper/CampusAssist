using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        System.Windows.Forms.NotifyIcon notify;
        public MainWindow(WebProcess _web)
        {
            InitializeComponent();
            web = _web;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* 一以下最好开新的线程处理否则会阻塞 */
            string mainPage = web.doRedict();           // 首次进入需要重定向一次
            string[] info = HtmlParse.getInfo(mainPage);
            nameLbl.Content = info[0];
            ipLbl.Content = info[1];
            string html = web.getDocument("http://applicationidc.ecnu.edu.cn/ecnuidc/sso/ssoemailchh.jsp", Encoding.Default);
            info = HtmlParse.getEmail(html);
            mailCntLbl.Content = info[0];
            unreadCntLbl.Content = info[1];
            /* 以上 */
            dateLbl.Content = DateTime.Now.ToLongDateString().ToString() + " " + weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            notify = new System.Windows.Forms.NotifyIcon();
            notify.Icon = new System.Drawing.Icon("icon.ico");
            notify.Click += new EventHandler(reSize);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                string page;
                switch (tabControl.SelectedIndex)
                {
                    // 教务通知
                    case 1:
                        // 抓整个网页已经封装宰了web.getDocument中 直接调用即可获取整个网页
                        page = web.getDocument("http://www.jwc.ecnu.edu.cn/",Encoding.UTF8);

                        // 用已经实现好的HtmlParse中的相关函数获取需要的信息, 请参考HtmlParse
                        string[] info = HtmlParse.getAnnounce(page);

                        // 动态向announcementGrid添加元素
                        for(int i = 0; i < info.Length; i++)
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

                    // 校园卡信息
                    case 6:
                        page = web.getDocument("http://portal.ecnu.edu.cn/eapdomain/neudcp/sso/sso_ecard_xxcx.jsp", Encoding.Default);
                        string[] bal = HtmlParse.getBalance(page);
                        balanceLbl.Content = bal[1];
                        statusLbl.Content = bal[0];

                        page = web.getDocument("http://www.ecard.ecnu.edu.cn/Ecard/cqmoney.aspx", Encoding.Default);
                        string[] consume = HtmlParse.getConsume(page);
                        for (int i = 0; i < consume.Length; i++)
                        {
                            string[] cur = consume[i].Split(' ');
                            costGrid.RowDefinitions.Add(new RowDefinition());
                            for (int j = 0; j < 6; j++)
                            {
                                Label lbl = new Label();
                                lbl.FontSize = 12;
                                lbl.Content = cur[j];
                                costGrid.Children.Add(lbl);
                                Grid.SetRow(lbl, i+1);
                                Grid.SetColumn(lbl, j);
                            }

                            
                        }

                        break;
                    default:
                        // 需要将不用的清除否则会重叠绘制
                        announcementGrid.Children.RemoveRange(0, announcementGrid.Children.Count);
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

        private void reSize(object sender,EventArgs e)
        {
            notify.Visible = false;
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }
    }
}
