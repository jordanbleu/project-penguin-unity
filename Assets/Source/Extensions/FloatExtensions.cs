namespace Source.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Returns true if this is within '<paramref name="units"/>' of '<paramref name="ofValue"/>' 
        /// (plus or minus)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <param name="ofValue"></param>
        /// <returns></returns>
        public static bool IsWithin(this float value, float units, float ofValue)
        {
            var min = ofValue - units;
            var max = ofValue + units;
            return (value >= min) && (value <= max);
        }
        
        /// <summary>
        /// Returns a value that is +/-<paramref name="stabilizationRate"/> towards <paramref name="stabilizationRate"/> from
        /// <paramref name="value"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stabilizationRate"></param>
        /// <param name="finalValue"></param>
        /// <returns></returns>
        public static float Stabilize(this float value, float stabilizationRate, float finalValue)
        {
            float newValue = value;

            if (value.IsWithin(stabilizationRate, finalValue))
            {
                newValue = finalValue;
            }
            else if (value > finalValue)
            {
                newValue -= stabilizationRate;
            }
            else if (value < finalValue)
            {
                newValue += stabilizationRate;
            }

            return newValue;
        }
    }
}