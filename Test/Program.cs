using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Test
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var url = "https://static-cdn.jtvnw.net/badges/v1/bd444ec6-8f34-4bf9-91f4-af1e3428d80f/3";
            var uri = new Uri(url);
            Console.WriteLine(uri.LocalPath);
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("A");
        }
    }

}

