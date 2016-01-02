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
            string html = web.getDocument("http://applicationidc.ecnu.edu.cn/ecnuidc/sso/ssoemailchh.jsp", Encoding.UTF8);
            info = HtmlParse.getEmail(html);
            mailCntLbl.Content = info[0];
            unreadCntLbl.Content = info[1];
        }
    }
}
