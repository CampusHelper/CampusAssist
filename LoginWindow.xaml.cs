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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CampusAssist
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private WebProcess web;
        public LoginWindow()
        {
            InitializeComponent();
            try
            {
                web = new WebProcess(ref captchaImg);
            }catch
            {
                MessageBox.Show("网络故障.");
                Close();
            }
            
        }

        private void onExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void onLogin(object sender, RoutedEventArgs e)
        {
            if (web.login(userID.Text, password.Password, captcha.Text))
            {
                MainWindow wd = new MainWindow(web);
                wd.Show();
                Close();
            }
            else
            {
                MessageBox.Show("登录失败","失败");
            }

        }

        private void onEmail(object sender, RoutedEventArgs e)
        {
            Email form = new Email();
            form.Show();
        }
    }
}
