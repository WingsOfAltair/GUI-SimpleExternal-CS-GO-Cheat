using System;
using System.Diagnostics;
using System.Threading;
using Smurf.GlobalOffensive.SDK;
using System.Windows.Forms;

namespace Smurf.GlobalOffensive
{
    internal static class Program
    {
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimpleExternalUI());
        }
    }
}