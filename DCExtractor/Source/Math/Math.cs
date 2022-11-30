namespace Custom.Math
{
    /// <summary>
    /// Class   :   "MathHelpers"
    /// 
    /// Purpose :   provides helpful math functions for use with other custom math classes.
    /// </summary>
    public static class MathHelpers
    {
        //////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        //////////////////////////////////////////////////////////////////////////////////////////
	    public const float Epsilon = 0.00001f;


        //////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks to see if the value is close enough to zero that it can be clamped.
        /// </summary>
        /// <param name="fValue">The input value.</param>
        /// <param name="fClamp">The clamping value to check against.</param>
        /// <returns>The value, which may have been clamped to zero.</returns>
        public static float ClampToZero(float fValue, float fClamp = Epsilon)
        {
            if (System.Math.Abs(fValue) < fClamp)
                return 0.0f;
            return fValue;
        }

        /// <summary>
        /// Checks to see if the value is close enough to zero that it can be clamped.
        /// </summary>
        /// <param name="dValue">The input value.</param>
        /// <param name="fClamp">The clamping value to check against.</param>
        /// <returns>The value, which may have been clamped to zero.</returns>
        public static double ClampToZero(double dValue, double fClamp = Epsilon)
        {
            if (System.Math.Abs(dValue) < fClamp)
                return 0.0f;
            return dValue;
        }

        /// <summary>
        /// Returns if the two values are close enough to be considered equal.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>Whether or not the two values are close enough to be considered equal.</returns>
        public static bool CloseToEqual(float a, float b)
        {
            return (a - b < Epsilon && a - b > -Epsilon);
        }

        /// <summary>
        /// Returns if the value is close enough to zero.
        /// </summary>
        /// <param name="a">The value.</param>
        /// <returns>Whether or not the value is close to zero.</returns>
        public static bool IsZero(float a)
        {
            return a > -Epsilon && a < Epsilon;
        }

        /// <summary>
        /// Converts the incoming degrees to radians.
        /// </summary>
        /// <param name="dDegrees">The degrees to convert.</param>
        /// <returns>A radian angle.</returns>
        public static double ToRadians(double dDegrees)
        {
            return dDegrees * System.Math.PI / 180.0;
        }

        /// <summary>
        /// Converts the incoming degrees to radians.
        /// </summary>
        /// <param name="fDegrees">The degrees to convert.</param>
        /// <returns>A radian angle.</returns>
        public static float ToRadians(float fDegrees)
        {
            return (float)((double)fDegrees * System.Math.PI / 180.0);
        }

        /// <summary>
        /// Converts the incoming radians to degrees.
        /// </summary>
        /// <param name="dRadians">The radians to convert.</param>
        /// <returns>A degrees angle.</returns>
        public static double ToDegrees(double dRadians)
        {
            return dRadians * 180.0 / System.Math.PI;
        }

        /// <summary>
        /// Converts the incoming radians to degrees.
        /// </summary>
        /// <param name="dRadians">The radians to convert.</param>
        /// <returns>A degrees angle.</returns>
        public static float ToDegrees(float dRadians)
        {
            return (float)((double)dRadians * 180.0 / System.Math.PI);
        }

        /// <summary>
        /// Linear-interpolates from a to b with t.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The time along the line formed by a-b.</param>
        /// <returns>The interpolation of t along a-b.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return (1.0f - t) * a + t * b;
        }
    }
}
