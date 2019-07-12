using System;
using System.Windows.Forms;
using WindowsExplorer.Controllers;
using WindowsExplorer.Infrastructure;
using WindowsExplorer.Views;

namespace WindowsExplorer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Explorer(new ControllerExplorer(new Repository())));
        }
    }
}
