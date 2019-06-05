using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Timer timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();

            while (true)
            {
                Application.DoEvents();
                Console.WriteLine("B");

                Console.Read();

            }

        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("A");
        }
    }

}

