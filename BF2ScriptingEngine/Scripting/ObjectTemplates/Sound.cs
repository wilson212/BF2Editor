using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class Sound : ObjectTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        [PropertyName("soundFilename")]
        public ObjectProperty<string> SoundFilename { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("loopCount")]
        public ObjectProperty<int> LoopCount { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("is3dSound")]
        public ObjectProperty<bool> Is3dSound { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("stopType")]
        public ObjectProperty<int> StopType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("volume")]
        public ObjectProperty<double> Volume { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("pitch")]
        public ObjectProperty<double> Pitch { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("pan")]
        public ObjectProperty<double> Pan { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("reverbLevel")]
        public ObjectProperty<double> ReverbLevel { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("pitchEnvelope")]
        public ObjectProperty<string> PitchEnvelope { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("volumeEnvelope")]
        public ObjectProperty<string> VolumeEnvelope { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("minDistance")]
        public ObjectProperty<double> MinDistance { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("halfVolumeDistance")]
        public ObjectProperty<double> HalfVolumeDistance { get; internal set; }

        public Sound(string name, Token token) : base(name, token)
        {

        }        
    }
}
