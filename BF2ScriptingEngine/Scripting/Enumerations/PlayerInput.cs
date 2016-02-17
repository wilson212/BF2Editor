using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents a Player Input Key
    /// </summary>
    /// <remarks>
    /// Order was taken from the bf2editor by Dice
    /// </remarks>
    public enum PlayerInput
    {
        PIYaw,
        PIPitch,
        PIRoll,
        PIThrottle,

        PIMouseLookX, 
        PIMouseLookY,
        PICameraX,
        PICameraY,

        PIFire,
        PIAction,
        PIUse,
        PIMouseLook,
        PIAltSprint,
        PISprint,

        PIWeaponSelect1,
        PIWeaponSelect2,
        PIWeaponSelect3,
        PIWeaponSelect4,
        PIWeaponSelect5,
        PIWeaponSelect6,
        PIWeaponSelect7,
        PIWeaponSelect8,
        PIWeaponSelect9,

        PIPositionSelect1,
        PIPositionSelect2,
        PIPositionSelect3,
        PIPositionSelect4,
        PIPositionSelect5,
        PIPositionSelect6,
        PIPositionSelect7,
        PIPositionSelect8,

        PIAltFire,
        PIReload,

        PISelectFunc,
        PIDrop,
        PIToggleCameraMode,
        PIToggleCamera,
        PILie,
        PICrouch,

        PICameraMode1,
        PICameraMode2,
        PICameraMode3,
        PICameraMode4,

        PISelectPrimWeapon,
        PISelectSecWeapon,
        PIToggleWeapon,
        PIToggleFireRate,
        PIFlareFire,

        PIRadio1,
        PIRadio2,
        PIRadio3,
        PIRadio4,
        PIRadio5,
        PIRadio6,
        PIRadio7,
        PIRadio8,

        PIScreenShot,
        PIToolTip,

        PISayAll,
        PISayTeam,

        PINextItem,
        PIPrevItem,

        PICommunication,
        PIShowScoreBoard,

        PINone,
    }
}
