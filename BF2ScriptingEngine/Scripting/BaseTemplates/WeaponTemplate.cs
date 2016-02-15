using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Some property decritptions are copied from Korben @ battlefield Singleplayer
    /// </remarks>
    /// <seealso cref="http://www.battlefieldsingleplayer.com/forum/index.php?showtopic=8308"/>
    public class WeaponTemplate : ConFileObject
    {
        /// <summary>
        /// Indicates whether this weapon shoots directly at someone, or indrectly
        /// </summary>
        /// <remarks>
        /// Used for weapons that fire in the 45-90 deg range from forward, eg bombs 
        /// from a plane. It is used in bf2, eg the f15 bombs. - Korben
        /// </remarks>
        [PropertyName("indirect")]
        public ObjectProperty<bool> Indirect { get; set; }

        /// <summary>
        /// Is the projectile lobbed in an arc, like a grenade. 
        /// </summary>
        [PropertyName("isThrown")]
        public ObjectProperty<bool> IsThrown { get; set; }

        /// <summary>
        /// Gets or Sets the minimum range that the Bot will use this weapon
        /// </summary>
        [PropertyName("minRange")]
        public ObjectProperty<double> MinRange { get; set; }

        /// <summary>
        /// Gets or Sets the maximum range that the Bot will use this weapon
        /// </summary>
        [PropertyName("maxRange")]
        public ObjectProperty<double> MaxRange { get; set; }

        /// <summary>
        /// This variable tells the Bot at which range point he should be when using this weapon
        /// </summary>
        [PropertyName("optimalRangePercentage")]
        public ObjectProperty<int> OptimalRangePercentage { get; set; }

        /// <summary>
        /// Gets or Sets the pose (standing/crouching/prone) that the bot will fire 
        /// from once inside the optimal range.
        /// </summary>
        [PropertyName("setFiringPose")]
        public ObjectProperty<FiringPose> FiringPose { get; set; }

        /// <summary>
        /// Depreciated (from BF1942/BFV)
        /// </summary>
        /// <remarks>Hidden, as it is the "set" was removed in some BF2 files.</remarks>
        [PropertyName("firingPose")]
        protected ObjectProperty<FiringPose> SetFiringPose { get; set; }

        /// <summary>
        /// Tells the bot at what value of deviation he may make the first shot of the weapon.
        /// </summary>
        /// <remarks>
        /// I assume the default value is 5, same as the .deviation variable. The subtle difference 
        /// between the two variables is .deviation starts at the first action of aiming while 
        /// .allowedDeviation is the at the instant of the first shot. It only makes sense for 
        /// this variable's value to be the same or lower than .deviation variable. - Korben
        /// </remarks>
        /// <seealso cref="http://www.battlefieldsingleplayer.com/forum/index.php?showtopic=8308&view=findpost&p=101317"/>
        [PropertyName("allowedDeviation")]
        public ObjectProperty<double> AllowedDeviation { get; set; }

        /// <summary>
        /// This variable sets the initial value of deviation the first 
        /// time the bot chooses that particular weapon and begins aiming.
        /// </summary>
        /// <remarks>
        /// The default value is 5, which calculates to 5 meters / 100 meters or 5% deviation.
        /// Deviation ultimatly reflects on the bots starting accuracy with the weapon.
        /// </remarks>
        [PropertyName("deviation")]
        public ObjectProperty<double> Deviation { get; set; }

        /// <summary>
        /// For Bots, This variable sets the amount of time it takes for the weapon deviation to go 
        /// from the value set in the <see cref="Deviation"/> variable to a value of zero deviation.
        /// </summary>
        /// <remarks>
        /// For bots, weapon deviation decreases over time. The default value is 10, 
        /// which calculates to 10 seconds.
        /// </remarks>
        [PropertyName("deviationCorrectionTime")]
        public ObjectProperty<int> DeviationCorrectionTime { get; set; }

        /// <summary>
        /// Gets or Sets the numerical button to activate this weapon?
        /// </summary>
        [PropertyName("weaponActivate")]
        public ObjectProperty<PlayerInput> WeaponActivate { get; set; }

        /// <summary>
        /// Gets or Sets the firing button for this weapon
        /// </summary>
        [PropertyName("weaponFire")]
        public ObjectProperty<PlayerInput> WeaponFire { get; set; }

        /// <summary>
        /// Gets or Sets the Weights assigned to chance of bots selecting this type target
        /// </summary>
        [PropertyName("setStrength")]
        public ObjectProperty<Dictionary<StrengthType, double>> Strengths { get; set; }

        /// <summary>
        /// For bots only: Specifies the rate that the AI will fire the weapon at.
        /// This value does not affect human players.
        /// </summary>
        [PropertyName("fireRate")]
        public ObjectProperty<int> FireRate { get; set; }

        /// <summary>
        /// The range at which another bot will "hear" the firing of this particular weapon
        /// </summary>
        /// <remarks>
        /// Bots have two primary senses - sight and sound - to detect other bots. If a bot has his 
        /// back turned to you, he obviously can't see you, but if you fire your weapon 
        /// (at another bot for example) he may hear that firing and detect you. It is a spherical range, 
        /// i.e. works in all directions forward/behind/above/below. - Korben
        /// </remarks>
        [PropertyName("setSoundSphereRadius")]
        public ObjectProperty<double> SoundSphereRadius { get; set; }

        /// <summary>
        /// When a projectile impacts whatever, this variable tells the bots (foe or friendly) 
        /// the effective damage zone, and the bots if they detect this projectile will attempt to evade 
        /// out of that radius (grenades for example).
        /// </summary>
        [PropertyName("setExplosionRadius")]
        public ObjectProperty<double> ExplosionRadius { get; set; }

        /// <summary>
        /// Will the projectile size fit through a chain-link fence? so the bot will/won't 
        /// fire that weapon at you through the fence.
        /// </summary>
        [PropertyName("setFiresThroughTransparent")]
        public ObjectProperty<bool> FiresThroughTransparent { get; set; }

        /// <summary>
        /// Creates a new isntance of WeaponTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public WeaponTemplate(string Name, Token Token) : base(Name, Token) { }

        /// <summary>
        /// Creates a new instance of WeaponTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static WeaponTemplate Create(Token token)
        {
            return new WeaponTemplate(token.TokenArgs.Arguments.Last(), token);
        }
    }
}
