using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class ControlInfo : AiTemplatePlugin
    {
        [PropertyName("driveTurnControl")]
        public ObjectProperty<PlayerInput> DriveTurnControl;

        [PropertyName("driveThrottleControl")]
        public ObjectProperty<PlayerInput> DriveThrottleControl;

        [PropertyName("aimHorizontalControl")]
        public ObjectProperty<PlayerInput> AimHorizontalControl;

        [PropertyName("aimVerticalControl")]
        public ObjectProperty<PlayerInput> AimVerticalControl;

        [PropertyName("lookHorizontalControl")]
        public ObjectProperty<PlayerInput> LookHorizontalControl;

        [PropertyName("lookVerticalControl")]
        public ObjectProperty<PlayerInput> LookVerticalControl;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("driveRollControl")]
        public ObjectProperty<PlayerInput> DriveRollControl;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("drivePitchControl")]
        public ObjectProperty<PlayerInput> DrivePitchControl;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimRollControl")]
        public ObjectProperty<PlayerInput> AimRollControl;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimPitchControl")]
        public ObjectProperty<PlayerInput> AimPitchControl;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("aimThrottleControl")]
        public ObjectProperty<PlayerInput> AimThrottleControl;

        [PropertyName("throttleSensitivity")]
        public ObjectProperty<decimal> ThrottleSensitivity;

        [PropertyName("pitchSensitivity")]
        public ObjectProperty<decimal> PitchSensitivity;

        [PropertyName("rollSensitivity")]
        public ObjectProperty<decimal> RollSensitivity;

        [PropertyName("yawSensitivity")]
        public ObjectProperty<decimal> YawSensitivity;

        [PropertyName("lookVerticalSensitivity")]
        public ObjectProperty<decimal> LookVerticalSensitivity;

        [PropertyName("lookHorizontalSensitivity")]
        public ObjectProperty<decimal> LookHorizontalSensitivity;

        [PropertyName("proportionalConstant")]
        public ObjectProperty<decimal> ProportionalConstant;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// First found: USJEP_HMMWV (HmmwvPassengerCtrl)
        /// </remarks>
        [PropertyName("angleSpeedConstant")]
        public ObjectProperty<decimal> AngleSpeedConstant;

        [PropertyName("derivativeConstant")]
        public ObjectProperty<decimal> DerivativeConstant;

        [PropertyName("throttleLookAhead")]
        public ObjectProperty<decimal> ThrottleLookAhead;

        [PropertyName("pitchLookAhead")]
        public ObjectProperty<decimal> PitchLookAhead;

        [PropertyName("rollLookAhead")]
        public ObjectProperty<decimal> RollLookAhead;

        [PropertyName("yawLookAhead")]
        public ObjectProperty<decimal> YawLookAhead;

        [PropertyName("lookVerticalLookAhead")]
        public ObjectProperty<decimal> LookVerticalLookAhead;

        [PropertyName("lookHorizontalLookAhead")]
        public ObjectProperty<decimal> LookHorizontalLookAhead;

        [PropertyName("throttleScale")]
        public ObjectProperty<decimal> ThrottleScale;

        [PropertyName("pitchScale")]
        public ObjectProperty<decimal> PitchScale;

        [PropertyName("rollScale")]
        public ObjectProperty<decimal> RollScale;

        [PropertyName("yawScale")]
        public ObjectProperty<decimal> YawScale;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("maxRollAngle")]
        public ObjectProperty<decimal> MaxRollAngle;

        /// <summary>
        /// Helicopters / Jets
        /// </summary>
        [PropertyName("maxClimbAngle")]
        public ObjectProperty<decimal> MaxClimbAngle;

        [PropertyName("lookVerticalScale")]
        public ObjectProperty<double> LookVerticalScale;

        [PropertyName("lookHorizontalScale")]
        public ObjectProperty<double> LookHorizontalScale;

        /// <summary>
        /// Helicopters?
        /// </summary>
        [PropertyName("setCameraRelativeDofRotationOffsetDeg")]
        public ObjectProperty<int> CameraRelativeDofRotationOffsetDeg;

        [PropertyName("setCameraRelativeMinRotationDeg")]
        public ObjectProperty<string> CameraRelativeMinRotationDeg;

        [PropertyName("setCameraRelativeMaxRotationDeg")]
        public ObjectProperty<string> CameraRelativeMaxRotationDeg;

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
