namespace Custom.Math
{
    /// <summary>
    /// Structure   :   "Vector4"
    /// 
    /// Purpose     :   A representation of a 3d point, a 3d vector, a 3d normal, or a quaternion.
    /// 
    /// The Level-5 formats seem to like storing their 3d data in vector4's.
    /// I'm unsure as to why they would want to do this, since the w(and sometimes z)-component goes unused most of the time, wasting space and processing power.
    /// That said, there might be a hardware reason or a byte offset issue at play I'm unaware of. That would explain a lot of the padding.
    /// Regardless, it doesn't really matter in hindsight, I'm just detailing what I observed. 
    /// Just keep this in mind, as a lot of the data you're likely to interact with in the MDS will be of Vector4 type.
    /// </summary>
    public struct Vector4
    {
        //////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        //////////////////////////////////////////////////////////////////////////////////////////
        public float x;
        public float y;
        public float z;
        public float w;


        //////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nX">The x component.</param>
        /// <param name="nY">The y component.</param>
        /// <param name="nZ">The z component.</param>
        /// <param name="nW">The w component.</param>
        public Vector4(float nX, float nY, float nZ, float nW)
        {
            x = nX;
            y = nY;
            z = nZ;
            w = nW;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nX">The x component.</param>
        /// <param name="nY">The y component.</param>
        /// <param name="nZ">The z component.</param>
        public Vector4(float nX, float nY, float nZ)
        {
            x = nX;
            y = nY;
            z = nZ;
            w = 0.0f;
        }

        /// <summary>
        /// Returns
        /// </summary>
        /// <returns>The string equivalent of this class.</returns>
        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }

        /// <summary>
        /// Operator multiply.
        /// </summary>
        /// <param name="tVector">The input vector.</param>
        /// <param name="fValue">The float multiplier.</param>
        /// <returns>A multiplied vector4.</returns>
        public static Vector4 operator *(Vector4 tVector, float fValue)
        {
            return new Vector4(tVector.x * fValue, tVector.y * fValue, tVector.z * fValue, tVector.w * fValue);
        }

        /// <summary>
        /// Returns a number with the magnitude of first argument and sign of second argument.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>A number with the magnitude of first argument and sign of second argument.</returns>
        static float CopySign(float a, float b)
        {
            if(float.IsNaN(a))
                return a * System.Math.Sign(b);
            return System.Math.Abs(a) * System.Math.Sign(b);
        }

        /// <summary>
        /// Converts the quaternion vector4 into euler angles.
        /// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <returns>The euler angles that represent this quaternion.</returns>
        public Vector4 ToEulersFromQuaternion()
        {
            Vector4 q = this;
            Vector4 euler = new Vector4();

            // roll (x-axis rotation)
            float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
            float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
            euler.x = (float)System.Math.Atan2((double)sinr_cosp, (double)cosr_cosp);

            // pitch (y-axis rotation)
            float sinp = 2 * (q.w * q.y - q.z * q.x);
            if (System.Math.Abs(sinp) >= 1)
                euler.y = CopySign((float)System.Math.PI / 2.0f, sinp); // use 90 degrees if out of range
            else
                euler.y = (float)System.Math.Asin(sinp);

            // yaw (z-axis rotation)
            float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
            float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
            euler.z = (float)System.Math.Atan2(siny_cosp, cosy_cosp);

            return euler;
        }

        /// <summary>
        /// Linear-interpolates from a to b with t.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The time along the line formed by a-b.</param>
        /// <returns>The interpolation of t along a-b.</returns>
        public static Vector4 LerpVector(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(
                MathHelpers.Lerp(a.x, b.x, t),
                MathHelpers.Lerp(a.y, b.y, t),
                MathHelpers.Lerp(a.z, b.z, t),
                MathHelpers.Lerp(a.w, b.w, t)
                );
        }

        /// <summary>
        /// Returns the dot product between the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// Negates the Vector4.
        /// </summary>
        /// <returns>The negation of this Vector4.</returns>
        public Vector4 Negate()
        {
            return new Vector4(-x, -y, -z, -w);
        }

        /// <summary>
        /// Linear-interpolates from a to b with t. This is the quaternion version.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The time along the line formed by a-b.</param>
        /// <returns>The interpolation of t along a-b.</returns>
        public static Vector4 LerpQuaternion(Vector4 a, Vector4 b, float t)
        {
            //https://stackoverflow.com/questions/46156903/how-to-lerp-between-two-quaternions
            // negate second quat if dot product is negative
            float l2 = Dot(a, b);
            if (l2 < 0.0f)
            {
                b = b.Negate();
            }

            Vector4 c;
            // c = a + t(b - a)  -->   c = a - t(a - b)
            // the latter is slightly better on x64
            c.x = a.x - t * (a.x - b.x);
            c.y = a.y - t * (a.y - b.y);
            c.z = a.z - t * (a.z - b.z);
            c.w = a.w - t * (a.w - b.w);
            return c;
        }
    }
}