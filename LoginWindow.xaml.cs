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
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void onExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void onLogin(object sender, RoutedEventArgs e)
        {
            System.Net.CookieContainer ck = new System.Net.CookieContainer();
            System.Collections.ArrayList l = WebReauest.GetHtmlData("http://www.baidu.com", ck);
            for(int i = 0; i < l.Count; i++)
            {
                MessageBox.Show(l[i].ToString());
            }
            
            MainWindow wd = new MainWindow();
            wd.Show();
            Close();
            
        }

        private void onEmail(object sender, RoutedEventArgs e)
        {
            Email form = new Email();
            form.Show();
        }
    }
}
