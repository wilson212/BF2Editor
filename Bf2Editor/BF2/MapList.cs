using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BF2Editor
{
    public class MapList
    {
        /// <summary>
        /// Our regex expression to parse the maplines in the maplist.con
        /// </summary>
        const string Expression = @"^maplist.append[\s|\t]+([""]*)(?<Mapname>[a-z0-9_]+)([""]*)[\s|\t]+([""]*)gpm_(?<Gamemode>[a-z]+)([""]*)[\s|\t]+(?<Size>[0-9]+)";

        /// <summary>
        /// The map list entries in this maplist
        /// </summary>
        public List<MapListEntry> Entries { get; protected set; }

        /// <summary>
        /// Creates a new Maplist Container
        /// </summary>
        public MapList()
        {
            Entries = new List<MapListEntry>();
        }

        /// <summary>
        /// Creates a new maplist container with the specified maplist.con lines
        /// </summary>
        /// <param name="FileLines"></param>
        public MapList(string[] FileLines)
        {
            Entries = new List<MapListEntry>();
            foreach (string line in FileLines)
                AddFromString(line);
        }

        /// <summary>
        /// Creates a new maplist container from specified maplist.con file
        /// </summary>
        /// <param name="FilePath">The full filepath to the maplist.con file</param>
        public MapList(string FilePath)
        {
            // Attempt to open the file, and parse its contents
            using (FileStream Stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader Reader = new StreamReader(Stream))
            {
                Entries = new List<MapListEntry>();
                while (!Reader.EndOfStream)
                {
                    AddFromString(Reader.ReadLine());
                }
            }
        }
        
        /// <summary>
        /// Parses a line in con file format, and adds the map to the entry list if its valid
        /// </summary>
        /// <param name="Line">The con file formated string to parse</param>
        /// <returns>Returns whether the line was successfully parsed</returns>
        public bool AddFromString(string Line)
        {
            // Parse the 1st line of the con file
            Match M = Regex.Match(Line, Expression, RegexOptions.IgnoreCase);
            if (M.Success)
            {
                MapListEntry Entry = new MapListEntry();
                Entry.MapName = M.Groups["Mapname"].Value;
                Entry.GameMode = M.Groups["Gamemode"].Value;
                Entry.MapSize = Int32.Parse(M.Groups["Size"].Value);
                Entries.Add(Entry);
            }

            return M.Success;
        }

        /// <summary>
        /// Saves the current maplist to a Con file
        /// </summary>
        /// <param name="FilePath">The full path to the con file. If it does not exist, one will be created</param>
        public void SaveToFile(string FilePath)
        {
            using(FileStream Stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                // Clean out the file
                Stream.SetLength(0);

                // Append new data
                using(StreamWriter Writer = new StreamWriter(Stream))
                {
                    foreach (MapListEntry Entry in Entries)
                        Writer.WriteLine(Entry.ToString());
                }
            }
        }
    }
}
