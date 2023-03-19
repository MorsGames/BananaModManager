using System;
using System.Windows.Forms;

namespace BananaModManager
{
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0] == "-download")
            {
                string modURL = args[1];
                modURL = modURL.Remove(0, 17);
                Application.Run(new OneClickConfirmation(modURL));
            }
            else
            {
                //Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}