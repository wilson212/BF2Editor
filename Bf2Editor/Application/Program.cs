using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using BF2Editor.Logging;
using System.Threading;

namespace BF2Editor
{
    static class Program
    {
        public static readonly string RootPath = Application.StartupPath;

        public static LogWriter ErrorLog { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create a new log writer
            ErrorLog = new LogWriter(Path.Combine(RootPath, "Error.log"));

            // Enable visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Only allow 1 instance of this app
            bool createdNew = false;
            using (Mutex mutex = new Mutex(true, "b5d780ec-1570-4c95-a770-a7bec899d6bc", out createdNew))
            {
                if (!createdNew)
                {
                    // Alert the user
                    MessageBox.Show(
                        "The BF2Editor is already running. Only one instance of this application can run at a time.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    return;
                }

                // Run the application
                Application.Run(new MainForm());
            }
        }
    }
}
