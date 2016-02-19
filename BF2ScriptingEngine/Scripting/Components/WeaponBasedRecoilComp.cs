using System;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting.Components
{
    public class WeaponBasedRecoilComp : ConFileObject, IComponent
    {
        /// <summary>
        /// 
        /// </summary>
        [PropertyName("recoilSize")]
        public ObjectProperty<decimal> RecoilSize { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("cameraRecoilSpeed")]
        public ObjectProperty<int> CameraRecoilSpeed { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("cameraRecoilSize")]
        public ObjectProperty<double> CameraRecoilSize { get; internal set; }

        public WeaponBasedRecoilComp(string name, Token token) 
            : base("WeaponBasedRecoilComp", token)
        {

        }
    }
}
