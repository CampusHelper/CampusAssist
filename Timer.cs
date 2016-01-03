using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CampusAssist
{
    class Timer
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        int t=0;
       public Timer()
    {
        
        timer.Interval = TimeSpan.FromMilliseconds(3000);
        timer.Tick += new EventHandler(send);  //你的事件
        timer.Start();
        MessageBox.Show("hello1");
    }
       public void send(object sender, System.EventArgs e)
       {
           t++;
           if (t % 5 == 0)
           {
               MessageBox.Show("hello");
           }
        }

    }
}
