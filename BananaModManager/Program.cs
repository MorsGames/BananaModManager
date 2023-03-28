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
                string[] modInfo = args[1].Split(',');
                string downloadURL = modInfo[0].Remove(0,17);
                string modID = modInfo[1];
                Application.Run(new OneClickConfirmation(downloadURL, modID));
            }
            if(args.Length > 0 && args[0] == "--update")
            {
                try
                {
                    Update.DoUpdate();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                
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