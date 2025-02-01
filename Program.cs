using System;
using System.Windows.Forms;

namespace OmniTools
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Crée l'instance du MainForm et la stocke dans Logger
            MainForm mainForm = new MainForm();
            Logger.MainFormInstance = mainForm;

            Application.Run(mainForm);
        }
    }
}
