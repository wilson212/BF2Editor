using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public class ControlInfo : AiTemplatePlugin
    {
        [PropertyName("driveTurnControl")]
        public ObjectProperty<PlayerInput> DriveTurnControl { get; set; }

        [PropertyName("driveThrottleControl")]
        public ObjectProperty<PlayerInput> DriveThrottleControl { get; set; }

        [PropertyName("aimHorizontalControl")]
        public ObjectProperty<PlayerInput> AimHorizontalControl { get; set; }

        [PropertyName("aimVerticalControl")]
        public ObjectProperty<PlayerInput> AimVerticalControl { get; set; }

        [PropertyName("lookHorizontalControl")]
        public ObjectProperty<PlayerInput> LookHorizontalControl { get; set; }

        [PropertyName("lookVerticalControl")]
        public ObjectProperty<PlayerInput> LookVerticalControl { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("driveRollControl")]
        public ObjectProperty<PlayerInput> DriveRollControl { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("drivePitchControl")]
        public ObjectProperty<PlayerInput> DrivePitchControl { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimRollControl")]
        public ObjectProperty<PlayerInput> AimRollControl { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimPitchControl")]
        public ObjectProperty<PlayerInput> AimPitchControl { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimThrottleControl")]
        public ObjectProperty<PlayerInput> AimThrottleControl { get; set; }

        [PropertyName("throttleSensitivity")]
        public ObjectProperty<decimal> ThrottleSensitivity { get; set; }

        [PropertyName("pitchSensitivity")]
        public ObjectProperty<decimal> PitchSensitivity { get; set; }

        [PropertyName("rollSensitivity")]
        public ObjectProperty<decimal> RollSensitivity { get; set; }

        [PropertyName("yawSensitivity")]
        public ObjectProperty<decimal> YawSensitivity { get; set; }

        [PropertyName("lookVerticalSensitivity")]
        public ObjectProperty<decimal> LookVerticalSensitivity { get; set; }

        [PropertyName("lookHorizontalSensitivity")]
        public ObjectProperty<decimal> LookHorizontalSensitivity { get; set; }

        [PropertyName("proportionalConstant")]
        public ObjectProperty<decimal> ProportionalConstant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// First found: USJEP_HMMWV (HmmwvPassengerCtrl)
        /// </remarks>
        [PropertyName("angleSpeedConstant")]
        public ObjectProperty<decimal> AngleSpeedConstant { get; set; }

        [PropertyName("derivativeConstant")]
        public ObjectProperty<decimal> DerivativeConstant { get; set; }

        [PropertyName("throttleLookAhead")]
        public ObjectProperty<decimal> ThrottleLookAhead { get; set; }

        [PropertyName("pitchLookAhead")]
        public ObjectProperty<decimal> PitchLookAhead { get; set; }

        [PropertyName("rollLookAhead")]
        public ObjectProperty<decimal> RollLookAhead { get; set; }

        [PropertyName("yawLookAhead")]
        public ObjectProperty<decimal> YawLookAhead { get; set; }

        [PropertyName("lookVerticalLookAhead")]
        public ObjectProperty<decimal> LookVerticalLookAhead { get; set; }

        [PropertyName("lookHorizontalLookAhead")]
        public ObjectProperty<decimal> LookHorizontalLookAhead { get; set; }

        [PropertyName("throttleScale")]
        public ObjectProperty<decimal> ThrottleScale { get; set; }

        [PropertyName("pitchScale")]
        public ObjectProperty<decimal> PitchScale { get; set; }

        [PropertyName("rollScale")]
        public ObjectProperty<decimal> RollScale { get; set; }

        [PropertyName("yawScale")]
        public ObjectProperty<decimal> YawScale { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("maxRollAngle")]
        public ObjectProperty<decimal> MaxRollAngle { get; set; }

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("maxClimbAngle")]
        public ObjectProperty<decimal> MaxClimbAngle { get; set; }

        [PropertyName("lookVerticalScale")]
        public ObjectProperty<double> LookVerticalScale { get; set; }

        [PropertyName("lookHorizontalScale")]
        public ObjectProperty<double> LookHorizontalScale { get; set; }

        /// <summary>
        /// Helicopters?
        /// </summary>
        [PropertyName("setCameraRelativeDofRotationOffsetDeg")]
        public ObjectProperty<int> CameraRelativeDofRotationOffsetDeg { get; set; }

        [PropertyName("setCameraRelativeMinRotationDeg")]
        public ObjectProperty<string> CameraRelativeMinRotationDeg { get; set; }

        [PropertyName("setCameraRelativeMaxRotationDeg")]
        public ObjectProperty<string> CameraRelativeMaxRotationDeg { get; set; }

        /// <summary>
        /// A list of all of this objects properties that are found within the con and AI files
        /// </summary>
        protected static Dictionary<string, FieldInfo> Fields = new Dictionary<string, FieldInfo>();

        /// <summary>
        /// Creates a new instance of Armament
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The parser token</param>
        public ControlInfo(string Name, Token Token) : base(Name, Token) { }
    }
}
