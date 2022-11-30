namespace Custom.Math
{
    /// <summary>
    /// Structure   :   "Matrix"
    /// 
    /// Purpose     :   A representation of a 3-dimensional transformation.
    /// 
    /// The matrices are stored in row order, with translation along the bottom row, scale across the diagonal, and rotation in the top left 3x3.
    /// </summary>
    public class Matrix4x4
    {
        //////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        //////////////////////////////////////////////////////////////////////////////////////////
        public static readonly Matrix4x4 identity = new Matrix4x4();


        //////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        //////////////////////////////////////////////////////////////////////////////////////////
        float[] m_fMatrices = new float[16];


        //////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the list of matricies in the matrix.
        /// </summary>
        public float[] m
        {
            get { return m_fMatrices; }
        }

        /// <summary>
        /// Returns the translation of the matrix.
        /// </summary>
        public Vector4 translation
        {
            get { return new Vector4(m[12], m[13], m[14]); }
            set
            {
                m[12] = value.x;
                m[13] = value.y;
                m[14] = value.z;
            }
        }

        /// <summary>
        /// Returns the rotation of the matrix.
        /// </summary>
        public Vector4 rotation
        {
            get { return ToEulers(); }
        }

        /// <summary>
        /// Gets the scale of the matrix.
        /// </summary>
        public Vector4 scale
        {
            get { return new Vector4(m[0], m[5], m[10]); }
        }

        /// <summary>
        /// Gets whether or not this matrix is zero.
        /// </summary>
        public bool isZero
        {
            get
            {
                for (int i = 0; i < 16; i++)
                    if (m[i] != 0.0f)
                        return false;
                return true;
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        public Matrix4x4()
        {
            Identify();
        }

        /// <summary>
        /// Copy-Constructor.
        /// </summary>
        /// <param name="a">The other matrix.</param>
        public Matrix4x4(Matrix4x4 a)
        {
            for (int i = 0; i < 16; i++)
                m[i] = a.m[i];
        }

        /// <summary>
        /// Indexes the matrix on the x and y axis and returns the value at the position.
        /// </summary>
        /// <param name="x">The x axis.</param>
        /// <param name="y">The y axis.</param>
        /// <returns>The value of the matrix at this position.</returns>
        public float Get(int x, int y)
        {
            return m[y * 4 + x];
        }

        /// <summary>
        /// Zeroes out the matrix.
        /// </summary>
        public void Zero()
        {
            for (int i = 0; i < m.Length; i++)
                m[0] = 0.0f;
        }

        /// <summary>
        /// Sets the matrix to the identity matrix.
        /// </summary>
        public void Identify()
        {
            m[0] = 1.0f;
            m[1] = 0.0f;
            m[2] = 0.0f;
            m[3] = 0.0f;
            m[4] = 0.0f;
            m[5] = 1.0f;
            m[6] = 0.0f;
            m[7] = 0.0f;
            m[8] = 0.0f;
            m[9] = 0.0f;
            m[10] = 1.0f;
            m[11] = 0.0f;
            m[12] = 0.0f;
            m[13] = 0.0f;
            m[14] = 0.0f;
            m[15] = 1.0f;
        }

        /// <summary>
        /// Swaps the two values.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        static void Swap(ref float x, ref float y)
        {
            float fTemp = x;
            x = y;
            y = fTemp;
        }

        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        public void Transpose()
        {
            //x  1  2  3
            //4  x  6  7
            //8  9  x 11
            //12 13 14 x
            Swap(ref m[1], ref m[4]);
            Swap(ref m[2], ref m[8]);
            Swap(ref m[3], ref m[12]);
            Swap(ref m[6], ref m[9]);
            Swap(ref m[7], ref m[13]);
            Swap(ref m[11], ref m[14]);
        }

        /// <summary>
        /// Returns a transposition of this matrix.
        /// </summary>
        /// <returns>A transposed version of this matrix.</returns>
        public Matrix4x4 Transposed()
        {
            Matrix4x4 tMatrix = new Matrix4x4(this);
            tMatrix.Transpose();
            return tMatrix;
        }

        /// <summary>
        /// Returns the two by two matrix multiplication of the target values.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <param name="w">The w component.</param>
        /// <returns>The two by two matrix multiplication of the target values.</returns>
        static float TwoByTwo(float x, float y, float z, float w)
        {
            //xy
            //zw
            return ((x * w) - (y * z));
        }

        /// <summary>
        /// Returns the three by three matrix multiplication of the target values.
        /// </summary>
        /// <param name="a">The a component.</param>
        /// <param name="b">The b component.</param>
        /// <param name="c">The c component.</param>
        /// <param name="d">The d component.</param>
        /// <param name="e">The e component.</param>
        /// <param name="f">The f component.</param>
        /// <param name="g">The g component.</param>
        /// <param name="h">The h component.</param>
        /// <param name="i">The i component.</param>
        /// <returns>The three by three matrix multiplication of the target values.</returns>
        static float ThreeByThree(float a, float b, float c, float d, float e, float f, float g, float h, float i)
        {
            //a b c
            //d e f
            //g h i
            return ((a * TwoByTwo(e, f, h, i)) - (b * TwoByTwo(d, f, g, i)) + (c * TwoByTwo(d, e, g, h)));
        }

        /// <summary>
        /// Returns the determinant of the matrix.
        /// </summary>
        /// <returns>The determinant value.</returns>
        public float Determinant()
        {
            return (m[0] * ThreeByThree(m[5], m[6], m[7], m[9], m[10], m[11], m[13], m[14], m[15])) -
                (m[1] * ThreeByThree(m[4], m[6], m[7], m[8], m[10], m[11], m[12], m[14], m[15])) +
                (m[2] * ThreeByThree(m[4], m[5], m[7], m[8], m[9], m[11], m[12], m[13], m[15])) -
                (m[3] * ThreeByThree(m[4], m[5], m[6], m[8], m[9], m[10], m[12], m[13], m[14]));
        }

        /// <summary>
        /// Adds the second matrix to the calling matrix.
        /// </summary>
        /// <param name="b">The other matrix.</param>
        public void Add(Matrix4x4 b)
        {
            m[0] += b.m[0];
            m[1] += b.m[1];
            m[2] += b.m[2];
            m[3] += b.m[3];
            m[4] += b.m[4];
            m[5] += b.m[5];
            m[6] += b.m[6];
            m[7] += b.m[7];
            m[8] += b.m[8];
            m[9] += b.m[9];
            m[10] += b.m[10];
            m[11] += b.m[11];
            m[12] += b.m[12];
            m[13] += b.m[13];
            m[14] += b.m[14];
            m[15] += b.m[15];
        }

        /// <summary>
        /// Adds the first matrix to the second matrix.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 operator+(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4(a);
            result.Add(b);
            return result;
        }

        /// <summary>
        /// Subtracts the second matrix from the calling matrix.
        /// </summary>
        /// <param name="b">The other matrix.</param>
        public void Subtract(Matrix4x4 b)
        {
            m[0] -= b.m[0];
            m[1] -= b.m[1];
            m[2] -= b.m[2];
            m[3] -= b.m[3];
            m[4] -= b.m[4];
            m[5] -= b.m[5];
            m[6] -= b.m[6];
            m[7] -= b.m[7];
            m[8] -= b.m[8];
            m[9] -= b.m[9];
            m[10] -= b.m[10];
            m[11] -= b.m[11];
            m[12] -= b.m[12];
            m[13] -= b.m[13];
            m[14] -= b.m[14];
            m[15] -= b.m[15];
        }

        /// <summary>
        /// Subtracts the first matrix from the second matrix.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 operator -(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4(a);
            result.Subtract(b);
            return result;
        }

        /// <summary>
        /// Multiplies the matrix by the scalar.
        /// </summary>
        /// <param name="fScalar">The scalar to multiply by.</param>
        public void Multiply(float fScalar)
        {
            m[0] *= fScalar;
            m[1] *= fScalar;
            m[2] *= fScalar;
            m[3] *= fScalar;
            m[4] *= fScalar;
            m[5] *= fScalar;
            m[6] *= fScalar;
            m[7] *= fScalar;
            m[8] *= fScalar;
            m[9] *= fScalar;
            m[10] *= fScalar;
            m[11] *= fScalar;
            m[12] *= fScalar;
            m[13] *= fScalar;
            m[14] *= fScalar;
            m[15] *= fScalar;
        }

        /// <summary>
        /// Multiplies the first matrix by the scalar.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="scalar">The scalar multiplier.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 operator *(Matrix4x4 a, float scalar)
        {
            Matrix4x4 result = new Matrix4x4(a);
            result.Multiply(scalar);
            return result;
        }

        /// <summary>
        /// Multiplies the first matrix by the scalar.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4(a);
            result.MatrixMultiply(b);
            return result;
        }

        /// <summary>
        /// Divides the matrix by the scalar.
        /// </summary>
        /// <param name="fScalar">The scalar to divide by.</param>
        public void Divide(float fScalar)
        {
            m[0] /= fScalar;
            m[1] /= fScalar;
            m[2] /= fScalar;
            m[3] /= fScalar;
            m[4] /= fScalar;
            m[5] /= fScalar;
            m[6] /= fScalar;
            m[7] /= fScalar;
            m[8] /= fScalar;
            m[9] /= fScalar;
            m[10] /= fScalar;
            m[11] /= fScalar;
            m[12] /= fScalar;
            m[13] /= fScalar;
            m[14] /= fScalar;
            m[15] /= fScalar;
        }

        /// <summary>
        /// Divides the first matrix by the scalar.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="scalar">The scalar divider.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 operator /(Matrix4x4 a, float scalar)
        {
            Matrix4x4 result = new Matrix4x4(a);
            result.Divide(scalar);
            return result;
        }

        /// <summary>
        /// Negates the matrix.
        /// </summary>
        public void Negate()
        {
            m[0] = -m[0];
            m[1] = -m[1];
            m[2] = -m[2];
            m[3] = -m[3];
            m[4] = -m[4];
            m[5] = -m[5];
            m[6] = -m[6];
            m[7] = -m[7];
            m[8] = -m[8];
            m[9] = -m[9];
            m[10] = -m[10];
            m[11] = -m[11];
            m[12] = -m[12];
            m[13] = -m[13];
            m[14] = -m[14];
            m[15] = -m[15];
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        public void Invert()
        {
            float[] tempMat = new float[16];
            float det = 0.0f;

            tempMat[0] = m[5] * m[10] * m[15] -
                     m[5] * m[11] * m[14] -
                     m[9] * m[6] * m[15] +
                     m[9] * m[7] * m[14] +
                     m[13] * m[6] * m[11] -
                     m[13] * m[7] * m[10];

            tempMat[4] = -m[4] * m[10] * m[15] +
                      m[4] * m[11] * m[14] +
                      m[8] * m[6] * m[15] -
                      m[8] * m[7] * m[14] -
                      m[12] * m[6] * m[11] +
                      m[12] * m[7] * m[10];

            tempMat[8] = m[4] * m[9] * m[15] -
                     m[4] * m[11] * m[13] -
                     m[8] * m[5] * m[15] +
                     m[8] * m[7] * m[13] +
                     m[12] * m[5] * m[11] -
                     m[12] * m[7] * m[9];

            tempMat[12] = -m[4] * m[9] * m[14] +
                       m[4] * m[10] * m[13] +
                       m[8] * m[5] * m[14] -
                       m[8] * m[6] * m[13] -
                       m[12] * m[5] * m[10] +
                       m[12] * m[6] * m[9];

            tempMat[1] = -m[1] * m[10] * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * m[10];

            tempMat[5] = m[0] * m[10] * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * m[10];

            tempMat[9] = -m[0] * m[9] * m[15] +
                      m[0] * m[11] * m[13] +
                      m[8] * m[1] * m[15] -
                      m[8] * m[3] * m[13] -
                      m[12] * m[1] * m[11] +
                      m[12] * m[3] * m[9];

            tempMat[13] = m[0] * m[9] * m[14] -
                      m[0] * m[10] * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * m[10] -
                      m[12] * m[2] * m[9];

            tempMat[2] = m[1] * m[6] * m[15] -
                     m[1] * m[7] * m[14] -
                     m[5] * m[2] * m[15] +
                     m[5] * m[3] * m[14] +
                     m[13] * m[2] * m[7] -
                     m[13] * m[3] * m[6];

            tempMat[6] = -m[0] * m[6] * m[15] +
                      m[0] * m[7] * m[14] +
                      m[4] * m[2] * m[15] -
                      m[4] * m[3] * m[14] -
                      m[12] * m[2] * m[7] +
                      m[12] * m[3] * m[6];

            tempMat[10] = m[0] * m[5] * m[15] -
                      m[0] * m[7] * m[13] -
                      m[4] * m[1] * m[15] +
                      m[4] * m[3] * m[13] +
                      m[12] * m[1] * m[7] -
                      m[12] * m[3] * m[5];

            tempMat[14] = -m[0] * m[5] * m[14] +
                       m[0] * m[6] * m[13] +
                       m[4] * m[1] * m[14] -
                       m[4] * m[2] * m[13] -
                       m[12] * m[1] * m[6] +
                       m[12] * m[2] * m[5];

            tempMat[3] = -m[1] * m[6] * m[11] +
                      m[1] * m[7] * m[10] +
                      m[5] * m[2] * m[11] -
                      m[5] * m[3] * m[10] -
                      m[9] * m[2] * m[7] +
                      m[9] * m[3] * m[6];

            tempMat[7] = m[0] * m[6] * m[11] -
                     m[0] * m[7] * m[10] -
                     m[4] * m[2] * m[11] +
                     m[4] * m[3] * m[10] +
                     m[8] * m[2] * m[7] -
                     m[8] * m[3] * m[6];

            tempMat[11] = -m[0] * m[5] * m[11] +
                       m[0] * m[7] * m[9] +
                       m[4] * m[1] * m[11] -
                       m[4] * m[3] * m[9] -
                       m[8] * m[1] * m[7] +
                       m[8] * m[3] * m[5];

            tempMat[15] = m[0] * m[5] * m[10] -
                      m[0] * m[6] * m[9] -
                      m[4] * m[1] * m[10] +
                      m[4] * m[2] * m[9] +
                      m[8] * m[1] * m[6] -
                      m[8] * m[2] * m[5];

            det = m[0] * tempMat[0] + m[1] * tempMat[4] + m[2] * tempMat[8] + m[3] * tempMat[12];

            if (det == 0)
                return;

            det = 1.0f / det;
            for (int i = 0; i < 16; i++)
                m[i] = tempMat[i] * det;
        }

        /// <summary>
        /// Returns the inverse of the matrix.
        /// </summary>
        /// <returns>An inverse of the matrix.</returns>
        public Matrix4x4 Inversed()
        {
            Matrix4x4 output = new Matrix4x4(this);
            output.Invert();
            return output;
        }

        /// <summary>
        /// Performs an orthogonal inversion of the matrix.
        /// </summary>
        public void OrthogonalInvert()
        {
            //fill the empty spots with translation values.
            m[3] = -(m[0] * m[12] + m[1] * m[13] + m[2] * m[14]);
            m[7] = -(m[4] * m[12] + m[5] * m[13] + m[6] * m[14]);
            m[11] = -(m[8] * m[12] + m[9] * m[13] + m[10] * m[14]);

            //Transpose this matrix.
            Swap(ref m[1], ref m[4]);
            Swap(ref m[2], ref m[8]);
            Swap(ref m[3], ref m[12]);
            Swap(ref m[6], ref m[9]);
            Swap(ref m[7], ref m[13]);
            Swap(ref m[11], ref m[14]);

            //zero out the remaining empty spots.
            m[3] = 0.0f;
            m[7] = 0.0f;
            m[11] = 0.0f;
        }

        /// <summary>
        /// Multiplies the calling matrix by the matrix passed.
        /// </summary>
        /// <param name="tMat">The matrix to multiply by.</param>
        public void MatrixMultiply(Matrix4x4 tMat)
        {
            //|m m m m|   |0  1  2  3 |
            //|m m m m| * |4  5  6  7 |
            //|m m m m|   |8  9  10 11|
            //|m m m m|   |12 13 14 15|
            float[] fRes = new float[4];
            fRes[0] = m[0] * tMat.m[0] + m[1] * tMat.m[4] + m[2] * tMat.m[8] + m[3] * tMat.m[12];
            fRes[1] = m[0] * tMat.m[1] + m[1] * tMat.m[5] + m[2] * tMat.m[9] + m[3] * tMat.m[13];
            fRes[2] = m[0] * tMat.m[2] + m[1] * tMat.m[6] + m[2] * tMat.m[10] + m[3] * tMat.m[14];
            fRes[3] = m[0] * tMat.m[3] + m[1] * tMat.m[7] + m[2] * tMat.m[11] + m[3] * tMat.m[15];
            m[0] = fRes[0];
            m[1] = fRes[1];
            m[2] = fRes[2];
            m[3] = fRes[3];

            fRes[0] = m[4] * tMat.m[0] + m[5] * tMat.m[4] + m[6] * tMat.m[8] + m[7] * tMat.m[12];
            fRes[1] = m[4] * tMat.m[1] + m[5] * tMat.m[5] + m[6] * tMat.m[9] + m[7] * tMat.m[13];
            fRes[2] = m[4] * tMat.m[2] + m[5] * tMat.m[6] + m[6] * tMat.m[10] + m[7] * tMat.m[14];
            fRes[3] = m[4] * tMat.m[3] + m[5] * tMat.m[7] + m[6] * tMat.m[11] + m[7] * tMat.m[15];
            m[4] = fRes[0];
            m[5] = fRes[1];
            m[6] = fRes[2];
            m[7] = fRes[3];

            fRes[0] = m[8] * tMat.m[0] + m[9] * tMat.m[4] + m[10] * tMat.m[8] + m[11] * tMat.m[12];
            fRes[1] = m[8] * tMat.m[1] + m[9] * tMat.m[5] + m[10] * tMat.m[9] + m[11] * tMat.m[13];
            fRes[2] = m[8] * tMat.m[2] + m[9] * tMat.m[6] + m[10] * tMat.m[10] + m[11] * tMat.m[14];
            fRes[3] = m[8] * tMat.m[3] + m[9] * tMat.m[7] + m[10] * tMat.m[11] + m[11] * tMat.m[15];
            m[8] = fRes[0];
            m[9] = fRes[1];
            m[10] = fRes[2];
            m[11] = fRes[3];

            fRes[0] = m[12] * tMat.m[0] + m[13] * tMat.m[4] + m[14] * tMat.m[8] + m[15] * tMat.m[12];
            fRes[1] = m[12] * tMat.m[1] + m[13] * tMat.m[5] + m[14] * tMat.m[9] + m[15] * tMat.m[13];
            fRes[2] = m[12] * tMat.m[2] + m[13] * tMat.m[6] + m[14] * tMat.m[10] + m[15] * tMat.m[14];
            fRes[3] = m[12] * tMat.m[3] + m[13] * tMat.m[7] + m[14] * tMat.m[11] + m[15] * tMat.m[15];
            m[12] = fRes[0];
            m[13] = fRes[1];
            m[14] = fRes[2];
            m[15] = fRes[3];
        }

        /// <summary>
        /// Multiplies the first matrix by the second and returns it.
        /// </summary>
        /// <param name="tFirst">The first matrix.</param>
        /// <param name="tSecond">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x4 MatrixMultiply(Matrix4x4 tFirst, Matrix4x4 tSecond)
        {
            Matrix4x4 tReturn = new Matrix4x4(tFirst);
            tReturn.MatrixMultiply(tSecond);
            return tReturn;
        }

        /// <summary>
        /// Multiplies the left vector by the right matrix and returns the result.
        /// </summary>
        /// <param name="tLeftVector">The left vector.</param>
        /// <param name="tRightMatrix">The right matrix.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Vector4 VectorMultiply(Vector4 tLeftVector, Matrix4x4 tRightMatrix)
        {
            //		R	x	C
            //            |0  1  2  3 |
            //          * |4  5  6  7 | = 
            //            |8  9  10 11|
            //|x y z w|   |12 13 14 15|		|x y z w|
            //1x4			4x4			=	1x4
            Vector4 outVec = new Vector4();
            outVec.x = tLeftVector.x * tRightMatrix.m[0] + tLeftVector.y * tRightMatrix.m[4] + tLeftVector.z * tRightMatrix.m[8] + tLeftVector.w * tRightMatrix.m[12];
            outVec.y = tLeftVector.x * tRightMatrix.m[1] + tLeftVector.y * tRightMatrix.m[5] + tLeftVector.z * tRightMatrix.m[9] + tLeftVector.w * tRightMatrix.m[13];
            outVec.z = tLeftVector.x * tRightMatrix.m[2] + tLeftVector.y * tRightMatrix.m[6] + tLeftVector.z * tRightMatrix.m[10] + tLeftVector.w * tRightMatrix.m[14];
            outVec.w = tLeftVector.x * tRightMatrix.m[3] + tLeftVector.y * tRightMatrix.m[7] + tLeftVector.z * tRightMatrix.m[11] + tLeftVector.w * tRightMatrix.m[15];
            return outVec;
        }

        /// <summary>
        /// Generates a translation matrix.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>A translation matrix.</returns>
        public static Matrix4x4 GenerateTranslation(float x, float y, float z)
        {
            Matrix4x4 tOutput = new Matrix4x4();
            tOutput.m[0] = 1.0f;
            tOutput.m[1] = 0.0f;
            tOutput.m[2] = 0.0f;
            tOutput.m[3] = 0.0f;
            tOutput.m[4] = 0.0f;
            tOutput.m[5] = 1.0f;
            tOutput.m[6] = 0.0f;
            tOutput.m[7] = 0.0f;
            tOutput.m[8] = 0.0f;
            tOutput.m[9] = 0.0f;
            tOutput.m[10] = 1.0f;
            tOutput.m[11] = 0.0f;
            tOutput.m[12] = x;
            tOutput.m[13] = y;
            tOutput.m[14] = z;
            tOutput.m[15] = 1.0f;
            return tOutput;
        }

        /// <summary>
        /// Generates a rotation matrix on the x-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <returns>A rotation matrix on the x-axis.</returns>
        public static Matrix4x4 GenerateRotationX(float angle)
        {
            angle = MathHelpers.ToRadians(angle);

            //|1  0  0  0|
            //|0  c -s  0|
            //|0  s  c  0|
            //|0  0  0  1|
            Matrix4x4 tOutput = Matrix4x4.identity;

            tOutput.m[5] = (float)System.Math.Cos((double)angle);
            tOutput.m[9] = (float)System.Math.Sin((double)angle);
            tOutput.m[5] = MathHelpers.ClampToZero(tOutput.m[5]);
            tOutput.m[9] = MathHelpers.ClampToZero(tOutput.m[9]);
            tOutput.m[6] = -tOutput.m[9];
            tOutput.m[10] = tOutput.m[5];

            tOutput.m[0] = 1.0f;
            tOutput.m[1] = 0.0f;
            tOutput.m[2] = 0.0f;
            tOutput.m[3] = 0.0f;
            tOutput.m[4] = 0.0f;
            tOutput.m[7] = 0.0f;
            tOutput.m[8] = 0.0f;
            tOutput.m[11] = 0.0f;
            tOutput.m[12] = 0.0f;
            tOutput.m[13] = 0.0f;
            tOutput.m[14] = 0.0f;
            tOutput.m[15] = 1.0f;

            return tOutput;
        }

        /// <summary>
        /// Generates a rotation matrix on the y-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <returns>A rotation matrix on the y-axis.</returns>
        public static Matrix4x4 GenerateRotationY(float angle)
        {
            angle = MathHelpers.ToRadians(angle);

            //|c 0 s 0|
            //|0 1 0 0|
            //|-s0 c 0|
            //|0 0 0 1|
            Matrix4x4 tOutput = Matrix4x4.identity;

            tOutput.m[0] = (float)System.Math.Cos((double)angle);
            tOutput.m[2] = (float)System.Math.Sin((double)angle);
            tOutput.m[0] = MathHelpers.ClampToZero(tOutput.m[0]);
            tOutput.m[2] = MathHelpers.ClampToZero(tOutput.m[2]);
            tOutput.m[8] = -tOutput.m[2];
            tOutput.m[10] = tOutput.m[0];

            tOutput.m[1] = 0.0f;
            tOutput.m[3] = 0.0f;
            tOutput.m[4] = 0.0f;
            tOutput.m[5] = 1.0f;
            tOutput.m[6] = 0.0f;
            tOutput.m[7] = 0.0f;
            tOutput.m[9] = 0.0f;
            tOutput.m[11] = 0.0f;
            tOutput.m[12] = 0.0f;
            tOutput.m[13] = 0.0f;
            tOutput.m[14] = 0.0f;
            tOutput.m[15] = 1.0f;

            return tOutput;
        }

        /// <summary>
        /// Generates a rotation matrix on the z-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <returns>A rotation matrix on the z-axis.</returns>
        public static Matrix4x4 GenerateRotationZ(float angle)
        {
            angle = MathHelpers.ToRadians(angle);


            angle = MathHelpers.ToRadians(angle);

            //|c -s0 0|
            //|s c 0 0|
            //|0 0 1 0|
            //|0 0 0 1|
            Matrix4x4 tOutput = Matrix4x4.identity;

            tOutput.m[0] = (float)System.Math.Cos((double)angle);
            tOutput.m[4] = (float)System.Math.Sin((double)angle);
            tOutput.m[0] = MathHelpers.ClampToZero(tOutput.m[0]);
            tOutput.m[4] = MathHelpers.ClampToZero(tOutput.m[4]);
            tOutput.m[1] = -tOutput.m[4];
            tOutput.m[5] = tOutput.m[0];

            tOutput.m[2] = 0.0f;
            tOutput.m[3] = 0.0f;
            tOutput.m[6] = 0.0f;
            tOutput.m[7] = 0.0f;
            tOutput.m[8] = 0.0f;
            tOutput.m[9] = 0.0f;
            tOutput.m[10] = 1.0f;
            tOutput.m[11] = 0.0f;
            tOutput.m[12] = 0.0f;
            tOutput.m[13] = 0.0f;
            tOutput.m[14] = 0.0f;
            tOutput.m[15] = 1.0f;

            return tOutput;
        }

        /// <summary>
        /// Generates a scale matrix.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <returns>A scale matrix.</returns>
        public static Matrix4x4 GenerateScale(float x = 1.0f, float y = 1.0f, float z = 1.0f)
        {
            Matrix4x4 tOutput = Matrix4x4.identity;

            tOutput.m[0] = x;
            tOutput.m[1] = 0.0f;
            tOutput.m[2] = 0.0f;
            tOutput.m[3] = 0.0f;
            tOutput.m[4] = 0.0f;
            tOutput.m[5] = y;
            tOutput.m[6] = 0.0f;
            tOutput.m[7] = 0.0f;
            tOutput.m[8] = 0.0f;
            tOutput.m[9] = 0.0f;
            tOutput.m[10] = z;
            tOutput.m[11] = 0.0f;
            tOutput.m[12] = 0.0f;
            tOutput.m[13] = 0.0f;
            tOutput.m[14] = 0.0f;
            tOutput.m[15] = 1.0f;

            return tOutput;
        }

        /// <summary>
        /// Returns the string equivalent of this matrix.
        /// </summary>
        /// <returns>The string equivalent of this class.</returns>
        public override string ToString()
        {
            string szRet = "(";
            for (int i = 0; i < 15; i++)
            {
                szRet += m[i] + ", ";
            }
            szRet += m[15] + ")";
            return szRet;
        }

        /// <summary>
        /// Clones the matrix and returns it.
        /// </summary>
        /// <returns>The clone of the matrix.</returns>
        public Matrix4x4 Clone()
        {
            return new Matrix4x4(this);
        }

        /// <summary>
        /// Converts the incoming quaternion into a rotation matrix. Assuming XYZ orientation.
        /// </summary>
        /// <param name="tQuaternion">The quaternion to convert.</param>
        /// <returns>The resulting rotation matrix.</returns>
        public static Matrix4x4 FromQuaternion(Vector4 tQuaternion)
        {
            //https://automaticaddison.com/how-to-convert-a-quaternion-to-a-rotation-matrix/
            Matrix4x4 tResult = new Matrix4x4();

            float q0 = tQuaternion.w;
            float q1 = tQuaternion.x;
            float q2 = tQuaternion.y;
            float q3 = tQuaternion.z;

            tResult.m[0] = 2 * (q0 * q0 + q1 * q1) - 1;
            tResult.m[1] = 2 * (q1 * q2 - q0 * q3);
            tResult.m[2] = 2 * (q1 * q3 + q0 * q2);
            tResult.m[3] = 0;

            tResult.m[4] = 2 * (q1 * q2 + q0 * q3);
            tResult.m[5] = 2 * (q0 * q0 + q2 * q2) - 1;
            tResult.m[6] = 2 * (q2 * q3 - q0 * q1);
            tResult.m[7] = 0;

            tResult.m[8] = 2 * (q1 * q3 - q0 * q2);
            tResult.m[9] = 2 * (q2 * q3 + q0 * q1);
            tResult.m[10] = 2 * (q0 * q0 + q3 * q3) - 1;
            tResult.m[11] = 0;

            return tResult;
        }

        /// <summary>
        /// Converts the matrix's rotation to a quaternion.
        /// </summary>
        /// <returns>The quaternion equivalent of this matrix's rotation.</returns>
        public Vector4 ToQuaternion()
        {
            //https://answers.unity.com/questions/11363/converting-matrix4x4-to-quaternion-vector3.html
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Vector4 q = new Vector4();
            q.w = (float)System.Math.Sqrt(System.Math.Max(0, 1 + Get(0, 0) + Get(1, 1) + Get(2, 2))) / 2;
            q.x = (float)System.Math.Sqrt(System.Math.Max(0, 1 + Get(0, 0) - Get(1, 1) - Get(2, 2))) / 2;
            q.y = (float)System.Math.Sqrt(System.Math.Max(0, 1 - Get(0, 0) + Get(1, 1) - Get(2, 2))) / 2;
            q.z = (float)System.Math.Sqrt(System.Math.Max(0, 1 - Get(0, 0) - Get(1, 1) + Get(2, 2))) / 2;
            q.x *= System.Math.Sign(q.x * (Get(2, 1) - Get(1, 2)));
            q.y *= System.Math.Sign(q.y * (Get(0, 2) - Get(2, 0)));
            q.z *= System.Math.Sign(q.z * (Get(1, 0) - Get(0, 1)));
            return q;
        }

        /// <summary>
        /// Converts the matrix into euler angles.
        /// </summary>
        /// <returns>A series of euler angles representing the matrix.</returns>
        public Vector4 ToEulers()
        {
            return ToQuaternion().ToEulersFromQuaternion();
        }
    }
}