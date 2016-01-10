using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace BF2Editor
{
    public class BF2Mod
    {
        /// <summary>
        /// Returns the Mod's full name
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Returns the mods folder name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Returns the mods Root folder path
        /// </summary>
        public string RootPath { get; protected set; }

        /// <summary>
        /// Returns the mods level's folder
        /// </summary>
        public string LevelsPath { get; protected set; }

        /// <summary>
        /// Containst a list of map folders found in the levels folder
        /// </summary>
        public List<string> Levels;

        /// <summary>
        /// Once a Map is loaded into an object, it is stored
        /// here to prevent additional loading
        /// </summary>
        protected Dictionary<string, BF2Map> LoadedLevels;

        /// <summary>
        /// The full path to the maplist.con file
        /// </summary>
        public string MaplistFilePath { get; protected set; }

        /// <summary>
        /// Gets or Sets the contents of the maplist.con file.
        /// </summary>
        public MapList MapList { get; protected set; }

        /// <summary>
        /// Constructs a new BF2Mod object
        /// </summary>
        /// <param name="ModsPath">The full path to the Mods folder</param>
        /// <param name="ModName">THe mod's folder name</param>
        public BF2Mod(string ModsPath, string ModName)
        {
            // Set internal vars
            this.Name = ModName;
            this.RootPath = Path.Combine(ModsPath, ModName);
            this.LevelsPath = Path.Combine(RootPath, "levels");
            this.MaplistFilePath = Path.Combine(RootPath, "settings", "maplist.con");
            string DescFile = Path.Combine(ModsPath, ModName, "mod.desc");

            // Make sure we have a mod description file
            if (!File.Exists(DescFile))
            {
                Program.ErrorLog.Write("NOTICE: Mod \"" + ModName + "\" Does not contain mod.desc file");
                throw new InvalidModException("Mod \"" + ModName + "\" does not contain a mod.desc file");
            }

            // Make sure we have a levels directory
            if (!Directory.Exists(LevelsPath))
            {
                Program.ErrorLog.Write("NOTICE: Mod \"" + ModName + "\" Does not contain a Levels folder");
                throw new InvalidModException("Mod \"" + ModName + "\" does not contain a levels folder");
            }

            // Make sure we have a maplist!
            if (!File.Exists(MaplistFilePath))
            {
                Program.ErrorLog.Write("NOTICE: Mod \"" + ModName + "\" Does not contain a maplist.con file");
                throw new InvalidModException("Mod \"" + ModName + "\" does not contain a maplist.con file");
            }

            // Get the actual name of the mod
            try
            {
                XmlDocument Desc = new XmlDocument();
                Desc.Load(DescFile);
                XmlNodeList Node = Desc.GetElementsByTagName("title");
                string Name = Node[0].InnerText.Trim();
                if (Name == "MODDESC_BF2_TITLE")
                    this.Title = "Battlefield 2";
                else if (Name == "MODDESC_XP_TITLE")
                    this.Title = "Battlefield 2: Special Forces";
                else
                    this.Title = Name;
            }
            catch(Exception E)
            {
                throw new InvalidModException(
                    "Mod \"" + ModName + "\" contains an invalid Mod.desc file: "
                    + Environment.NewLine + "   " + E.Message, E
                );
            }

            // Load the levels
            Levels = new List<string>(from dir in Directory.GetDirectories(LevelsPath) select dir.Substring(LevelsPath.Length + 1));
            LoadedLevels = new Dictionary<string, BF2Map>();

            // Load the maplist
            MapList = new MapList(MaplistFilePath);
        }

        /// <summary>
        /// Fetches the BF2 map into an object. If the map has already been loaded
        /// into an object previously, that object will be returned instead.
        /// </summary>
        /// <param name="Name">The map FOLDER name</param>
        /// <returns></returns>
        public BF2Map LoadMap(string Name)
        {
            // Check 2 things, 1, does the map exist, and 2, if we have loaded it already
            if (!Levels.Contains(Name))
                throw new ArgumentException("Level Not Found");
            else if (LoadedLevels.ContainsKey(Name))
                return LoadedLevels[Name];

            // Create a new instance of the map, and store it for later
            BF2Map Map = new BF2Map(Name, LevelsPath);
            LoadedLevels.Add(Name, Map);
            return Map;
        }

        /// <summary>
        /// Returns the Mod's Title
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Title;
        }
    }
}
