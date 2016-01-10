using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class Mobile : AiTemplatePlugin
    {
        /// <summary>
        /// Air vehicles?
        /// </summary>
        [PropertyName("vehicleNumber")]
        public ObjectProperty<int> VehicleNumber;


        [PropertyName("setPathfindingMap")]
        public ObjectProperty<string> PathfindingMap;

        /// <summary>
        /// Defines the templates Max Speed.
        /// </summary>
        [PropertyName("maxSpeed")]
        public ObjectProperty<double> MaxSpeed;

        /// <summary>
        /// range is 0.1 to 135.0; most commonly 5.0
        /// </summary>
        [PropertyName("turnRadius")]
        public ObjectProperty<double> TurnRadius;

        /// <summary>
        /// range is 20.0 to 50.0; most commonly 50.0
        /// </summary>
        [PropertyName("coverSearchRadius")]
        public ObjectProperty<double> CoverSearchRadius;

        /// <summary>
        /// Range is -1.5 to 1.7; most commonly 0.9
        /// </summary>
        [PropertyName("lodHeight")]
        public ObjectProperty<double> LodHeight;

        /// <summary>
        /// Indicates whether the vehicle is turnable
        /// </summary>
        [PropertyName("isTurnable")]
        public ObjectProperty<bool> IsTurnable;

        /// <summary>
        /// Airplanes?
        /// </summary>
        [PropertyName("setSoundSphereRadius")]
        protected ObjectProperty<double[]> SoundSphereRadius;

        /// <summary>
        /// Airplanes?
        /// </summary>
        [PropertyName("setHearingProbability")]
        protected ObjectProperty<double[]> HearingProbability;

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>/
        /// <param name="Token">The parser token</param>
        public Mobile(string Name, Token Token) : base(Name, Token) { }
    }
}
