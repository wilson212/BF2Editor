using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class Mobile : AiTemplatePlugin
    {
        /// <summary>
        /// Air vehicles?
        /// </summary>
        [PropertyName("vehicleNumber")]
        public ObjectProperty<int> VehicleNumber { get; set; }


        [PropertyName("setPathfindingMap")]
        public ObjectProperty<string> PathfindingMap { get; set; }

        /// <summary>
        /// Defines the templates Max Speed.
        /// </summary>
        [PropertyName("maxSpeed")]
        public ObjectProperty<double> MaxSpeed { get; set; }

        /// <summary>
        /// range is 0.1 to 135.0; most commonly 5.0
        /// </summary>
        [PropertyName("turnRadius")]
        public ObjectProperty<double> TurnRadius { get; set; }

        /// <summary>
        /// range is 20.0 to 50.0; most commonly 50.0
        /// </summary>
        [PropertyName("coverSearchRadius")]
        public ObjectProperty<double> CoverSearchRadius { get; set; }

        /// <summary>
        /// Range is -1.5 to 1.7; most commonly 0.9
        /// </summary>
        [PropertyName("lodHeight")]
        public ObjectProperty<double> LodHeight { get; set; }

        /// <summary>
        /// Indicates whether the vehicle is turnable
        /// </summary>
        [PropertyName("isTurnable")]
        public ObjectProperty<bool> IsTurnable { get; set; }

        /// <summary>
        /// Airplanes?
        /// </summary>
        [PropertyName("setSoundSphereRadius")]
        protected ObjectProperty<double, double> SoundSphereRadius { get; set; }

        /// <summary>
        /// Airplanes?
        /// </summary>
        [PropertyName("setHearingProbability")]
        protected ObjectProperty<double, double> HearingProbability { get; set; }

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>/
        /// <param name="Token">The parser token</param>
        public Mobile(string Name, Token Token) : base(Name, Token) { }
    }
}
