using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BF2Editor
{
    public class BF2Map
    {
        /// <summary>
        /// Returns the Map's full name, or title
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Returns the Maps folder name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Returns the Maps ROOT directory
        /// </summary>
        public string RootPath { get; protected set; }

        /// <summary>
        /// A dictionary, of each "GameMode" => List("Supported Map Sizes")
        /// </summary>
        public Dictionary<string, List<string>> GameModes { get; protected set; }

        /// <summary>
        /// Constructs a new instance of a BF2Map object
        /// </summary>
        /// <param name="Name">The folder name of the map</param>
        /// <param name="LevelsPath">The full root path to the mods levels folder</param>
        public BF2Map(string Name, string LevelsPath)
        {
            // Define internals
            this.Name = Name;
            this.RootPath = Path.Combine(LevelsPath, Name);
            this.GameModes = new Dictionary<string, List<string>>();

            // Make sure the Descriptor file is located
            string DescFile = Path.Combine(RootPath, "Info", Name + ".desc");
            if (!File.Exists(DescFile))
                throw new InvalidMapException("Map does not contain a descriptor file");

            // Load the map description file
            try
            {
                XmlDocument Doc = new XmlDocument();
                Doc.Load(DescFile);

                // Get a list of supported modes, and add them to the GameModes and Mode Sizes variables
                XmlNodeList Modes = Doc.GetElementsByTagName("mode");
                if(Modes.Count == 0)
                    throw new InvalidMapException("Map descriptor file does not contain any game mode descriptions");

                // Loop though each game mode, and get the supported map sizes
                foreach (XmlNode m in Modes)
                {
                    string mode = m.Attributes["type"].InnerText;
                    List<string> temp = new List<string>();
                    foreach (XmlNode c in m.ChildNodes)
                        temp.Add(c.Attributes["players"].InnerText);

                    GameModes.Add(mode, temp);
                }

                // Get map name
                XmlNode Node = Doc.SelectSingleNode("/map/name");
                if (Node == null)
                    throw new InvalidMapException("Map descriptor file does not contain a valid name element");

                this.Title = Node.InnerText.Trim();
            }
            catch (Exception e)
            {
                throw new InvalidMapException("There was an error loading the map descriptor file", e);
            }
        }

        /// <summary>
        /// Returns the maps Title
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Title;
        }

    }
}
