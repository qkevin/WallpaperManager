using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args == null || !args.Any())
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            else
            {
                var startWithoutGUI = bool.Parse(args[0]);
                App app = new App();
                app.StartWithoutGUI = startWithoutGUI;
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
