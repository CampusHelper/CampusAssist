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
        public MainWindow(WebProcess _web)
        {
            InitializeComponent();
            web = _web;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string mainPage = web.doRedict();
            string[] info = HtmlParse.getInfo(mainPage);
            nameLbl.Content = info[0];
            ipLbl.Content = info[1];
            string html = web.getDocument("http://applicationidc.ecnu.edu.cn/ecnuidc/sso/ssoemailchh.jsp", Encoding.Default);
            info = HtmlParse.getEmail(html);
            mailCntLbl.Content = info[0];
            unreadCntLbl.Content = info[1];
            dateLbl.Content = DateTime.Now.ToLongDateString().ToString() + " " + weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                switch (tabControl.SelectedIndex)
                {
                    // 教务通知
                    case 1:
                        string page = web.getDocument("http://www.jwc.ecnu.edu.cn/",Encoding.UTF8);
                        string[] info = HtmlParse.getAnnounce(page);
                        for(int i = 0; i < info.Length; i++)
                        {
                            Label lbl = new Label();
                            string[] cur = info[i].Split(' ');
                            lbl.Content = cur[0];
                            RowDefinition rd = new RowDefinition();
                            announcementGrid.RowDefinitions.Add(rd);
                            announcementGrid.Children.Add(lbl);
                            Grid.SetRow(lbl, i);
                            Grid.SetColumn(lbl, 0);
                            lbl = new Label();
                            lbl.Content = cur[1];
                            announcementGrid.Children.Add(lbl);
                            Grid.SetRow(lbl, i);
                            Grid.SetColumn(lbl, 1);
                        }
                        break;
                }
            }
        }
    }
}
