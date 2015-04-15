using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TDS_MetaSolve
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            License lic = new License();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
