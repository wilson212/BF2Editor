using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Contains a list of networkable informations to be 
    /// used on objects that need to be synced over a network
    /// </summary>
    public enum NetworkableInfo
    {
        BasicInfo,
        CameraFireArmNetworkable,
        ControlPointInfo,
        DestroyableObjectInfo,
        GhostAlwaysWOPrediction,
        GhostAlwaysWithPrediction,
        HandFireArmsInfo,
        KitInfo,
        LaserTargetInfo,
        NoInfo,
        ProjectileInfo,
        SpringInfo
    }
}
