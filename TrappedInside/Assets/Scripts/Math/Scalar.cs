using UnityEngine;

namespace TI
{
    public static class Scalar
    {
        /// <summary>
        /// Lerps from <paramref name="value"/> towards <paramref name="target"/>
        /// at most <paramref name="step"/> amount.
        /// </summary>
        public static float LerpTowards(float value, float target, float step)
        {
            return Mathf.Abs(target - value) <= step
                ? target
                : value + step * Mathf.Sign(target - value);
        }
    }
}
