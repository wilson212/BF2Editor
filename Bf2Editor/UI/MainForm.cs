using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Editor.Properties;
using BF2Editor.UI;
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;
using Ionic.Zip;

namespace BF2Editor
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Our current selected log output tab
        /// </summary>
        protected LogEntryType LogPage = LogEntryType.Info;

        /// <summary>
        /// Our current executing process
        /// </summary>
        public static readonly Process myProcess = Process.GetCurrentProcess();

        /// <summary>
        /// The current selected mod or null
        /// </summary>
        public static BF2Mod SelectedMod;

        public MainForm()
        {
            // Create form ui controls
            InitializeComponent();

            // Update Settings if we need to
            if (!Settings.Default.SettingsUpdated)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpdated = true;
                Settings.Default.Save();
            }

            // Start off by using the last working directory
            if (!String.IsNullOrWhiteSpace(Settings.Default.LastUsedPath))
            {
                try
                {
                    LoadBf2Directory(Settings.Default.LastUsedPath);
                }
                catch { } // Supress
            }
        }

        #region ServerObjects

        /// <summary>
        /// Sets the working for Battlefield 2
        /// </summary>
        /// <param name="folderPath"></param>
        private void LoadBf2Directory(string folderPath)
        {
            // Try and load battlefield 2 mods
            BF2Client.SetInstallPath(folderPath);

            // check for ESAI
            bool esai = Directory.Exists(Path.Combine(folderPath, "mods", "bf2", "ESAI"));

            // Enable buttons on the form
            ChangeModButton.Enabled = true;
            LoadObjectsButton.Enabled = true;

            // Set texts
            DirectoryLabel.Text = folderPath;
            EsaiLabel.Text = (esai) ? "Yes" : "No";

            // Selected Mod
            if (!String.IsNullOrWhiteSpace(Settings.Default.LastSelectedMod))
            {
                SelectedMod = BF2Client.Mods
                    .Where(x => x.Name == Settings.Default.LastSelectedMod)
                    .FirstOrDefault();

                // If mod is null, we changed paths maybe?
                if (SelectedMod != null)
                {
                    SelectedModLabel.Text = $"{SelectedMod.Title} ({SelectedMod.Name})";
                }
            }
        }

        /// <summary>
        /// Loads every .ai file and parses it for use within this form
        /// </summary>
        protected async void ParseTemplates()
        {
            // Clear nodesand Globals!
            treeView1.Nodes.Clear();
            ObjectManager.ReleaseAll();

            try
            {

                // Load Kits
                LoadKitTemplate();

                // Load Vehicle templates
                await LoadTemplates("Vehicles");

                // Load Weapon templates
                await LoadTemplates("Weapons");

                // Create a new log entry with parsing details
                LogEntry entry = new LogEntry()
                {
                    Type = LogEntryType.Info,
                    Message = String.Format(
                    "========== Loaded Objects: {0} succeeded, {2} failed, {1} skipped ==========",
                        ObjectManager.ObjectsCount, Logger.Warnings.Count, Logger.Errors.Count
                    )
                };

                // Append an empry entry (line break), and the Final message
                Logger.Messages.Add(new LogEntry());
                Logger.Messages.Add(entry);

                // Update Loaded objects
                ObjectsLoadedLabel.Text = ObjectManager.ObjectsCount.ToString();
                ErrorsStripButton.Enabled = Logger.Errors.Count > 0;
                ErrorsStripButton.Text = Logger.Errors.Count + " Errors";
                WarningsStripButton.Enabled = Logger.Warnings.Count > 0;
                WarningsStripButton.Text = Logger.Warnings.Count + " Warnings";

                // Finally
                LogPage = LogEntryType.Error; // Force redraw
                MessagesStripButton_Click(this, EventArgs.Empty);

                // Refresh this process
                myProcess.Refresh();

                // DO garabge collection first, to get the most accurate memory size
                GC.Collect();

                // Update label
                MemoryUsageLabel.Text = String.Format(
                    new FileSizeFormatProvider(),
                    "{0:fs1}", myProcess.PrivateMemorySize64
                );
            }
            catch { }
        }

        /// <summary>
        /// Loads all of the *.ai files located in the SubDir name, into
        /// the ObjectManager
        /// </summary>
        /// <param name="templateType">The folder name of .ai files to load</param>
        protected Task LoadTemplates(string templateType)
        {
            return Task.Run(async() =>
            {
                string path = Path.Combine(Program.RootPath, "Temp", "Server Objects", SelectedMod.Name, templateType);
                TreeNode topNode = new TreeNode(templateType);

                // Make sure our tempalte directory exists
                if (!Directory.Exists(path))
                    return;

                // Now loop through each object type
                foreach (string dir in Directory.EnumerateDirectories(path))
                {
                    string dirName = dir.Remove(0, path.Length + 1);

                    // Skip common folder
                    if (dirName.ToLowerInvariant() == "common")
                        continue;

                    TreeNode dirNode = new TreeNode(dirName);
                    foreach (string subdir in Directory.EnumerateDirectories(dir))
                    {
                        string subdirName = subdir.Remove(0, dir.Length + 1);

                        // Skip common folder
                        if (subdirName.ToLowerInvariant() == "common")
                            continue;

                        // Skip dirs that dont have an ai folder
                        if (!Directory.Exists(Path.Combine(subdir, "ai")))
                            continue;

                        TreeNode subNode = new TreeNode(subdirName);
                        Dictionary<AiFileType, ConFile> files = new Dictionary<AiFileType, ConFile>(2);
                        ConFile cFile;

                        // Load the Objects.ai file if we have one
                        string file = Path.Combine(subdir, "ai", "Objects.ai");
                        if (File.Exists(file))
                        {
                            cFile = await ScriptEngine.LoadFileAsync(file, true);
                            if (cFile == null)
                                continue;

                            files.Add(AiFileType.Object, cFile);
                        }

                        // Load the Weapons.ai file if we have one
                        file = Path.Combine(subdir, "ai", "Weapons.ai");
                        if (File.Exists(file))
                        {
                            cFile = await ScriptEngine.LoadFileAsync(file, true);
                            if (cFile == null)
                                continue;

                            files.Add(AiFileType.Weapon, cFile);
                        }

                        // Add to tree view
                        if (files.Count > 0)
                        {
                            subNode.Tag = files;
                            dirNode.Nodes.Add(subNode);
                        }
                    }

                    if (dirNode.Nodes.Count > 0)
                        topNode.Nodes.Add(dirNode);

                }

                // Cross-Thread
                Invoke((MethodInvoker)delegate
                {
                    if (topNode.Nodes.Count > 0)
                        treeView1.Nodes.Add(topNode);
                });
            });
        }

        /// <summary>
        /// Loads the KIT object template 
        /// </summary>
        protected void LoadKitTemplate()
        {
            string path = Path.Combine(
                Program.RootPath, "Temp", "Server Objects", 
                SelectedMod.Name, "Kits", "ai", "Objects.ai"
            );

            // Xpack doesnt have a kits folder!
            if (File.Exists(path))
            {
                /*
                TreeNode topNode = new TreeNode("Kits");
                Dictionary<AiFileType, AiFile> files = new Dictionary<AiFileType, AiFile>();
                AiFile file = new AiFile(path, AiFileType.Kit);
                ObjectManager.RegisterFileObjects(file);
                files.Add(AiFileType.Kit, file);
                topNode.Tag = files;
                treeView1.Nodes.Add(topNode);
                */
            }
        }
        
        /// <summary>
        /// Currently, this method just extracts the contents of the Server_Objects.zip,
        /// into the "/Temp/Server Objects/{mod}" folder for processing.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Task ExtractObjects(string path)
        {
            return Task.Run(async () =>
            {
                // Empty the directory and delete it
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    await Task.Delay(250);
                }

                // Create a new directory
                Directory.CreateDirectory(path);

                // Load zip
                //TaskForm.UpdateStatus("Extracting Zip Contents...");
                string file = Path.Combine(SelectedMod.RootPath, "Objects_Server.zip");
                using (ZipFile Zip = ZipFile.Read(file))
                {
                    // Extract just the files we need
                    ZipEntry[] Entries = Zip.Entries.Where(
                        x => (
                            x.FileName.StartsWith("Vehicles", StringComparison.InvariantCultureIgnoreCase) ||
                            x.FileName.StartsWith("Weapons", StringComparison.InvariantCultureIgnoreCase) ||
                            x.FileName.StartsWith("Kits", StringComparison.InvariantCultureIgnoreCase)
                        ) && x.FileName.EndsWith(".ai", StringComparison.InvariantCultureIgnoreCase)
                    ).ToArray();

                    // Extract entries
                    foreach (ZipEntry entry in Entries)
                    {
                        // Extract the entry
                        entry.Extract(path, ExtractExistingFileAction.Throw);

                        // Get rid of read only attributes!
                        FileInfo F = new FileInfo(Path.Combine(path, entry.FileName));
                        F.Attributes = FileAttributes.Normal;
                    }
                }
            });
        }

        #endregion

        #region Message Window

        /// <summary>
        /// Fills the Output window with all the entries suppiled.
        /// </summary>
        /// <param name="Entries"></param>
        private void FillOutput(List<LogEntry> Entries)
        {
            listView2.Invoke((MethodInvoker)delegate
            {
                // Clear old messages
                listView2.BeginUpdate();
                listView2.Items.Clear();

                // Loop through each entry, and add it to the Message view
                foreach (LogEntry entry in Entries)
                {
                    string file = (entry.File == null) ? "" : entry.File.FilePath;
                    string line = (entry.Line == 0) ? "" : entry.Line.ToString();

                    ListViewItem listViewItem = new ListViewItem(new[] { entry.Message, file, line });
                    listViewItem.Tag = entry;
                    listViewItem.UseItemStyleForSubItems = true;
                    listViewItem.ForeColor = GetEntryFontColor(entry.Type);
                    listView2.Items.Add(listViewItem);
                }

                // Set scroll to the bottom
                listView2.TopItem = listView2.Items.Cast<ListViewItem>().LastOrDefault();

                // Perform update
                listView2.EndUpdate();
            });
        }

        private Color GetEntryFontColor(LogEntryType logEntryType)
        {
            if (LogPage != LogEntryType.Info || logEntryType == LogEntryType.Info) 
                return Color.Black;

            return (logEntryType == LogEntryType.Error) ? Color.DarkRed : Color.DarkOrange;
        }

        private void MessagesStripButton_Click(object sender, EventArgs e)
        {
            // Return here if we didnt change the tabbing
            if (LogPage == LogEntryType.Info) return;

            // Set new page and fill the output windo
            LogPage = LogEntryType.Info;
            FillOutput(Logger.Messages);
        }

        private void ErrorsStripButton_Click(object sender, EventArgs e)
        {
            // Return here if we didnt change the tabbing
            if (LogPage == LogEntryType.Error) return;

            // Set new page and fill the output windo
            LogPage = LogEntryType.Error;
            FillOutput(Logger.Errors);
        }

        private void WarningsStripButton_Click(object sender, EventArgs e)
        {
            // Return here if we didnt change the tabbing
            if (LogPage == LogEntryType.Warning) return;

            // Set new page and fill the output windo
            LogPage = LogEntryType.Warning;
            FillOutput(Logger.Warnings);
        }

        #endregion

        #region Events

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            // Here we load a new bf2 directory
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Title = "Please select your Battlefield 2 client or server executable";
            Dialog.Filter = "Battlefield 2 Game or Server|*.exe";

            // Set the initial search directory if we found an install path via registry
            if (!String.IsNullOrWhiteSpace(Settings.Default.LastUsedPath))
                Dialog.InitialDirectory = Settings.Default.LastUsedPath;

            // Show Dialog
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(Dialog.FileName);
                bool samePath = folderPath.Equals(
                    Settings.Default.LastUsedPath, 
                    StringComparison.InvariantCultureIgnoreCase
                );

                // Save our last used directory
                Settings.Default.LastUsedPath = folderPath;
                if (!samePath) // If we aren't using the same path, also reset that
                    Settings.Default.LastSelectedMod = null;
                Settings.Default.Save();

                // Load the BF2 directory
                try
                {
                    LoadBf2Directory(folderPath);
                }
                catch (Exception ex)
                {
                    // Warn the user!
                    MessageBox.Show("Failed to load the selected Battlefield 2 directory!"
                        + Environment.NewLine.Repeat(2)
                        + ex.Message,
                        "Failed to load Battlfield 2", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                }
            }
        }

        private void SaveChangesMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RestoreMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CreatePackageMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ChangeModButton_Click(object sender, EventArgs e)
        {
            using (SelectModForm f = new SelectModForm())
            {
                f.ShowDialog(this);
                BF2Mod mod = f.GetSelectedMod();
                if (mod != null)
                {
                    SelectedMod = mod;
                    SelectedModLabel.Text = $"{SelectedMod.Title} ({SelectedMod.Name})";
                    Settings.Default.LastSelectedMod = SelectedMod.Name;
                    Settings.Default.Save();
                }
            }
        }

        private async void LoadObjectsButton_Click(object sender, EventArgs e)
        {
            // Make sure we have a mod selected
            if (SelectedMod == null)
            {
                MessageBox.Show("Please select a Battlefield 2 mod first!",
                    "No Mod Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                // Check if the temp files are already exist
                string path = Path.Combine(Program.RootPath, "Temp", "Server Objects", SelectedMod.Name);
                if (Directory.Exists(path))
                {
                    // Ask the user if we are reloading or not
                    DialogResult Res = MessageBox.Show(
                        String.Format(
                            "We have detected that the temporary files from: \"{0}\" are already extracted, Would you like to continue editing these files?"
                            + "{1}{1}By clicking no, these files will be removed and loaded again from the Mod isntallation directory.",
                            SelectedMod.Name, Environment.NewLine
                        ),
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );

                    // Form cancelled?
                    if (Res != DialogResult.Yes && Res != DialogResult.No)
                        return;

                    // Load new files
                    if (Res == DialogResult.No)
                        await ExtractObjects(path);
                }
                else
                {
                    await ExtractObjects(path);
                }

                // Parse the templates
                ParseTemplates();
            }
            catch
            {

            }
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView2.SelectedItems[0];
            if (item.Index > -1)
            {

            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Refresh this process
            myProcess.Refresh();

            // DO garabge collection first, to get the most accurate memory size
            GC.Collect();

            // Update label
            MemoryUsageLabel.Text = String.Format(
                new FileSizeFormatProvider(), 
                "{0:fs1}", myProcess.PrivateMemorySize64
            );
        }

        /// <summary>
        /// Testing...
        /// </summary>
        private async void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var obj = ObjectManager.GetObject<AiTemplate>("Ahe_Ah1z");
            string formated = obj.File.ToFileFormat();
            Clipboard.SetText(formated);
            
            try {
                string path = @"D:\Programming\C#\Projects\Bf2Editor\Bf2Editor\bin\Debug\Temp\Server Objects\bf2\Kits\US";
                ConFile file = await ScriptEngine.LoadFileAsync(Path.Combine(path, "us_kits.con"));
                formated = file.ToFileFormat();
                Clipboard.SetText(formated);

                ConFile file2 = await ScriptEngine.LoadFileAsync(Path.Combine(path, "US_Specops.con"));
                formated = file2.ToFileFormat();
                Clipboard.SetText(formated);
            }
            catch
            {

            }

            var c = Logger.Errors;
        }

        #endregion
    }
}
