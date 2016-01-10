using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BF2Editor
{
    /// <summary>
    /// This class represents the current selected BF2 client installation.
    /// </summary>
    public static class BF2Client
    {
        /// <summary>
        /// Contains the BF2 Servers root path
        /// </summary>
        public static string RootPath { get; private set; }

        /// <summary>
        /// The bf2 server python folder path
        /// </summary>
        public static string PythonPath { get; private set; }

        /// <summary>
        /// Contains a list of all the found (and valid) mod folders located in the "mods" directory
        /// </summary>
        /// <remarks>
        ///     Only mods that are considered valid make it into this list. In order for a mod
        ///     to be considered valid, it must contain a populated "mod.desc" file, a "settings/maplist.con" 
        ///     file, and a levels folder.
        /// </remarks>
        public static List<BF2Mod> Mods { get; private set; }

        /// <summary>
        /// While loading the server's found mods, if there was any errors, the messages
        /// will be stored in this list
        /// </summary>
        public static List<string> ModLoadErrors { get; private set; }

        /// <summary>
        /// Returns whether the server is currently running
        /// </summary>
        public static bool IsRunning
        {
            get { return ClientProcess != null; }
        }

        /// <summary>
        /// The active (or null) server process
        /// </summary>
        private static Process ClientProcess;

        public delegate void ClientChangedEvent();

        /// <summary>
        /// An event thats fired if the Bf2 server path is changed
        /// </summary>
        public static event ClientChangedEvent PathChanged;

        /// <summary>
        /// An event fired when the BF2 Server is started
        /// </summary>
        public static event ClientChangedEvent Started;

        /// <summary>
        /// An event fired when the BF2 server exits
        /// </summary>
        public static event ClientChangedEvent Exited;

        /// <summary>
        /// Loads a battlefield 2 server into this object for use.
        /// </summary>
        /// <param name="Bf2Path">The full root path to the server's executable file</param>
        public static void SetInstallPath(string Bf2Path)
        {
            // Make sure we have a valid server path
            if (!File.Exists(Path.Combine(Bf2Path, "bf2.exe")) && !File.Exists(Path.Combine(Bf2Path, "bf2_w32ded.exe")))
            {
                throw new ArgumentException("Invalid BF2 installation path");
            }

            // Make sure we actually changed server paths before processing
            if (!String.IsNullOrEmpty(RootPath) && (new Uri(Bf2Path)) == (new Uri(RootPath)))
            {
                // Same path is selected, just return
                return;
            }

            // Temporary variables
            string Modpath = Path.Combine(Bf2Path, "mods");
            List<BF2Mod> TempMods = new List<BF2Mod>();

            // Make sure the server has the required folders
            if (!Directory.Exists(Modpath))
            {
                // Write trace
                throw new Exception("Unable to locate the 'mods' folder. Please make sure you have selected a valid "
                    + "battlefield 2 installation path before proceeding.");

            }

            // Load all found mods, discarding invalid mods
            ModLoadErrors = new List<string>();
            IEnumerable<string> ModList = from dir in Directory.GetDirectories(Modpath) select dir.Substring(Modpath.Length + 1);
            foreach (string Name in ModList)
            {
                try
                {
                    // Create a new instance of the mod, and store it for later
                    BF2Mod Mod = new BF2Mod(Modpath, Name);
                    TempMods.Add(Mod);
                }
                catch (InvalidModException E)
                {
                    ModLoadErrors.Add(E.Message);
                    continue;
                }
                catch (Exception E)
                {
                    ModLoadErrors.Add(E.Message);
                }
            }

            // We need mods bro...
            if (TempMods.Count == 0)
            {
                throw new Exception("No valid battlefield 2 mods could be found in the Bf2 Server mods folder!");
            }

            // Define var values after we now know this server apears valid
            RootPath = Bf2Path;
            Mods = TempMods;

            // Fire change event
            if (PathChanged != null)
                PathChanged();
            
            // Recheck server process
            CheckServerProcess();
        }

        /// <summary>
        /// Assigns the Server Process if the process is running
        /// </summary>
        private static void CheckServerProcess()
        {
            try
            {
                Process[] processCollection = Process.GetProcessesByName("bf2");
                foreach (Process P in processCollection)
                {
                    if (Path.GetDirectoryName(P.MainModule.FileName) == RootPath)
                    {
                        // Hook into the proccess so we know when its running, and register a closing event
                        ClientProcess = P;
                        ClientProcess.EnableRaisingEvents = true;
                        ClientProcess.Exited += ServerProcess_Exited;

                        // Fire Event
                        if (Started != null)
                            Started();
                        break;
                    }
                }
            }
            catch { } // Who cares?
        }

        /// <summary>
        /// Starts the Battlefield 2 Server application
        /// </summary>
        /// <param name="Mod">The battlefield 2 mod that the server is to use</param>
        /// <param name="ExtraArgs">Any arguments to be past to the application on startup</param>
        public static void Start(BF2Mod Mod, string ExtraArgs)
        {
            // Make sure the server isnt running already
            if (IsRunning)
                throw new Exception("Battlefield 2 is already running!");

            // Make sure the mod is supported!
            if (!Mods.Contains(Mod))
                throw new Exception("The battlefield 2 mod cannot be located in the mods folder");

            // Start new BF2 proccess
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = String.Format(" +modPath mods/{0}", Mod.Name.ToLower());
            if (!String.IsNullOrEmpty(ExtraArgs))
                Info.Arguments += " " + ExtraArgs;

            // Start process. Set working directory so we dont get errors!
            Info.FileName = "bf2.exe";
            Info.WorkingDirectory = RootPath;
            ClientProcess = Process.Start(Info);

            // Hook into the proccess so we know when its running, and register a closing event
            ClientProcess.EnableRaisingEvents = true;
            ClientProcess.Exited += ServerProcess_Exited;

            // Call event
            if (Started != null)
                Started();

            while (!ClientProcess.WaitForInputIdle(50)) Application.DoEvents();
        }

        /// <summary>
        /// Kills the Bf2 Server process
        /// </summary>
        public static void Stop()
        {
            if (IsRunning) ClientProcess.Kill();
        }

        /// <summary>
        /// Event fired when the server closes down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ServerProcess_Exited(object sender, EventArgs e)
        {
            if (Exited != null)
                Exited();

            ClientProcess.Close();
            ClientProcess = null;
        }
    }
}
