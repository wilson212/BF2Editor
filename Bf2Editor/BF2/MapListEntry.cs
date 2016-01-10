using System;

namespace BF2Editor
{
    public class MapListEntry
    {
        /// <summary>
        /// Gets or Sets the map name of this entry
        /// </summary>
        public string MapName;

        /// <summary>
        /// Gets or Sets the Gamemode for this map entry
        /// </summary>
        public string GameMode;

        /// <summary>
        /// Gets or Sets the Map size of this entry (16,32,64,128)
        /// </summary>
        public int MapSize;

        /// <summary>
        /// Creates a new instance of MapListEntry
        /// </summary>
        /// <param name="MapName">The Map Name for this entry</param>
        /// <param name="GameMode">The Gamemode for this entry</param>
        /// <param name="MapSize">The Map size of this entry (16,32,64,128)</param>
        public MapListEntry(string MapName = "", string GameMode = "", int MapSize = 16)
        {
            this.MapName = MapName;
            this.GameMode = GameMode;
            this.MapSize = MapSize;
        }

        /// <summary>
        /// Returns this MapListEntry in Con File Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("mapList.append {0} gpm_{1} {2}", MapName, GameMode, MapSize);
        }
    }
}
