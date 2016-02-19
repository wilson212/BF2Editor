namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Documentaion provided by bfmods.com
    /// </summary>
    /// <seealso cref="http://bfmods.com/mdt/scripting/CRD.html"/>
    public enum CrdType
    {
        /// <summary>
        /// Do not vary the argument at all, just return the first number.
        /// </summary>
        /// <example>
        /// ObjectTemplate.TimeToLive CRD_NONE/240/0/0
        ///     
        /// For the exppack, the time to live before it blows up by itself is always 240 seconds. 
        /// This sort of CRD can be directly replaced by the number itself, e.g. on the ships the 
        /// landing craft TimeToLive property is set to 30, even though this property's argument 
        /// is normally a CRD.
        /// </example>
        CRD_NONE,

        /// <summary>
        /// Uniform random distribution, between the two numbers specified.
        /// </summary>
        /// <example>
        /// ObjectTemplate.TimeToLive CRD_UNIFORM/2/4/0
        ///     
        /// The time to live before it blows up by itself is between 2 and 4 seconds.
        /// </example>
        CRD_UNIFORM,

        /// <summary>
        /// Exponential drop off, first number specifies the mean (average).
        /// </summary>
        /// <remarks>
        /// The exponential distribution basically gives a distribution curve that drops off 
        /// sharply and then levels out. It is computed by -mean*ln( random(0,1) ). In other words, 
        /// a random number between 0 and 1 (not including 0 or 1) is generated, the natural 
        /// logarithm of this number is computed, and the result is multiplied by the negative of 
        /// the mean (the average), the second argument (and first number) passed in.
        /// 
        /// For example, if the random number generated was 0.1, the natural log is -2.3, so since the 
        /// mean was -20, the final number returned would be -(-20)*-2.3 = -46. Since the symmetric flag, 
        /// the last argument, is set to 1 (true), there is a 50/50 chance that this result will be 
        /// negated, i.e. be returned as 46 instead of -46.
        /// </remarks>
        /// <example>
        /// ObjectTemplate.initRotation CRD_EXPONENTIAL/0/180/1
        /// 
        /// This example actually always returns a value of 0, since that is the mean specified. 
        /// The "180" argument is ignored.
        /// </example>
        CRD_EXPONENTIAL,

        /// <summary>
        /// Normal distribution, the first number is the mean, the second the variance.
        /// </summary>
        CRD_NORMAL
    }
}
