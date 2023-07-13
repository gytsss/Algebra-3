using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct MyMatrix4x4 : IEquatable<MyMatrix4x4>, IFormattable
{
    #region StaticProperties
    public static MyMatrix4x4 zero
    {
        get
        {
            return new MyMatrix4x4
            {
                m00 = 0f,
                m01 = 0f,
                m02 = 0f,
                m03 = 0f,
                m10 = 0f,
                m11 = 0f,
                m12 = 0f,
                m13 = 0f,
                m20 = 0f,
                m21 = 0f,
                m22 = 0f,
                m23 = 0f,
                m30 = 0f,
                m31 = 0f,
                m32 = 0f,
                m33 = 0f
            };
        }
    }

    public static MyMatrix4x4 identity
    {
        get
        {
            return new MyMatrix4x4
            {
                m00 = 1f,
                m01 = 0f,
                m02 = 0f,
                m03 = 0f,
                m10 = 0f,
                m11 = 1f,
                m12 = 0f,
                m13 = 0f,
                m20 = 0f,
                m21 = 0f,
                m22 = 1f,
                m23 = 0f,
                m30 = 0f,
                m31 = 0f,
                m32 = 0f,
                m33 = 1f
            };
        }
    }

    #endregion

    #region Properties
    public float m00;
    public float m01;
    public float m02;
    public float m03;

    public float m10;
    public float m11;
    public float m12;
    public float m13;

    public float m20;
    public float m21;
    public float m22;
    public float m23;

    public float m30;
    public float m31;
    public float m32;
    public float m33;

    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => m00,
                1 => m10,
                2 => m20,
                3 => m30,
                4 => m01,
                5 => m11,
                6 => m21,
                7 => m31,
                8 => m02,
                9 => m12,
                10 => m22,
                11 => m32,
                12 => m03,
                13 => m13,
                14 => m23,
                15 => m33,
                _ => throw new IndexOutOfRangeException("Invalid matrix index!"),
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    m00 = value;
                    break;
                case 1:
                    m10 = value;
                    break;
                case 2:
                    m20 = value;
                    break;
                case 3:
                    m30 = value;
                    break;
                case 4:
                    m01 = value;
                    break;
                case 5:
                    m11 = value;
                    break;
                case 6:
                    m21 = value;
                    break;
                case 7:
                    m31 = value;
                    break;
                case 8:
                    m02 = value;
                    break;
                case 9:
                    m12 = value;
                    break;
                case 10:
                    m22 = value;
                    break;
                case 11:
                    m32 = value;
                    break;
                case 12:
                    m03 = value;
                    break;
                case 13:
                    m13 = value;
                    break;
                case 14:
                    m23 = value;
                    break;
                case 15:
                    m33 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid matrix index!");
            }
        }
    }

    public float this[int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return this[row + column * 4];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            this[row + column * 4] = value;
        }
    }


    public bool isIdentity
    {
        get
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (row == col)
                    {
                        if (this[row, col] != 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (this[row, col] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }

    public float determinant
    {
        get
        {
            return this[0, 0] * (
            this[1, 1] * (this[2, 2] * this[3, 3] - this[2, 3] * this[3, 2]) -
            this[1, 2] * (this[2, 1] * this[3, 3] - this[2, 3] * this[3, 1]) +
            this[1, 3] * (this[2, 1] * this[3, 2] - this[2, 2] * this[3, 1])
        )
        -
        this[0, 1] * (
            this[1, 0] * (this[2, 2] * this[3, 3] - this[2, 3] * this[3, 2]) -
            this[1, 2] * (this[2, 0] * this[3, 3] - this[2, 3] * this[3, 0]) +
            this[1, 3] * (this[2, 0] * this[3, 2] - this[2, 2] * this[3, 0])
        ) +
        this[0, 2] * (
            this[1, 0] * (this[2, 1] * this[3, 3] - this[2, 3] * this[3, 1]) -
            this[1, 1] * (this[2, 0] * this[3, 3] - this[2, 3] * this[3, 0]) +
            this[1, 3] * (this[2, 0] * this[3, 1] - this[2, 1] * this[3, 0])
        ) -
        this[0, 3] * (
            this[1, 0] * (this[2, 1] * this[3, 2] - this[2, 2] * this[3, 1]) -
            this[1, 1] * (this[2, 0] * this[3, 2] - this[2, 2] * this[3, 0]) +
            this[1, 2] * (this[2, 0] * this[3, 1] - this[2, 1] * this[3, 0])
        ); ;
        }
    }

    public MyMatrix4x4 transpose
    {
        get
        {
            MyMatrix4x4 result = new MyMatrix4x4();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = this[j, i];
                }
            }

            return result;
        }
    }

    public Vector3 lossyScale => new(GetColumn(0).magnitude, GetColumn(1).magnitude, GetColumn(2).magnitude);


    #endregion

    #region Constructor
    public MyMatrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
    {
        m00 = column0.x;
        m01 = column1.x;
        m02 = column2.x;
        m03 = column3.x;
        m10 = column0.y;
        m11 = column1.y;
        m12 = column2.y;
        m13 = column3.y;
        m20 = column0.z;
        m21 = column1.z;
        m22 = column2.z;
        m23 = column3.z;
        m30 = column0.w;
        m31 = column1.w;
        m32 = column2.w;
        m33 = column3.w;

    }
    #endregion

    #region Operators
    public static MyMatrix4x4 operator *(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        MyMatrix4x4 result = zero;
        result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
        result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
        result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
        result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
        result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
        result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
        result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
        result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
        result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
        result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
        result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
        result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
        result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
        result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
        result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
        result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
        return result;
    }

    public static Vector4 operator *(MyMatrix4x4 lhs, Vector4 vector)
    {
        Vector4 result = new Vector4();

        result.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
        result.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
        result.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
        result.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;

        return result;
    }

    public static bool operator ==(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (lhs[i, j] != rhs[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool operator !=(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        return !(lhs == rhs);
    }

    #endregion

    #region StaticMethods

    public static MyMatrix4x4 Rotate(MyQuaternion q)
    {
        MyMatrix4x4 rotationMatrix = identity;

        float xx = q.x * q.x;
        float xy = q.x * q.y;
        float xz = q.x * q.z;
        float xw = q.x * q.w;

        float yy = q.y * q.y;
        float yz = q.y * q.z;
        float yw = q.y * q.w;

        float zz = q.z * q.z;
        float zw = q.z * q.w;

        rotationMatrix[0, 0] = 1 - 2 * (yy + zz);
        rotationMatrix[0, 1] = 2 * (xy - zw);
        rotationMatrix[0, 2] = 2 * (xz + yw);

        rotationMatrix[1, 0] = 2 * (xy + zw);
        rotationMatrix[1, 1] = 1 - 2 * (xx + zz);
        rotationMatrix[1, 2] = 2 * (yz - xw);

        rotationMatrix[2, 0] = 2 * (xz - yw);
        rotationMatrix[2, 1] = 2 * (yz + xw);
        rotationMatrix[2, 2] = 1 - 2 * (xx + yy);

        return rotationMatrix;
    }

    public static MyMatrix4x4 Scale(Vec3 vector)
    {
        MyMatrix4x4 scaleMatrix = identity;

        scaleMatrix[0, 0] = vector.x;
        scaleMatrix[1, 1] = vector.y;
        scaleMatrix[2, 2] = vector.z;

        return scaleMatrix;
    }

    public static MyMatrix4x4 Translate(Vec3 vector)
    {
        MyMatrix4x4 translationMatrix = identity;

        translationMatrix[0, 3] = vector.x;
        translationMatrix[1, 3] = vector.y;
        translationMatrix[2, 3] = vector.z;

        return translationMatrix;
    }

    public static MyMatrix4x4 TRS(Vec3 pos, MyQuaternion q, Vec3 s)
    {
        MyMatrix4x4 translationMatrix = Translate(pos);

        MyMatrix4x4 rotationMatrix = Rotate(q);

        MyMatrix4x4 scaleMatrix = Scale(s);
        MyMatrix4x4 resultMatrix = translationMatrix * rotationMatrix * scaleMatrix;

        return resultMatrix;
    }

    #endregion

    #region Methods

    public static float Determinant(Matrix4x4 m)
    {
        return m.determinant;
    }

    public MyMatrix4x4 Transpose(Matrix4x4 m)
    {
        MyMatrix4x4 transposeMatrix = new MyMatrix4x4();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                transposeMatrix[j, i] = m[i, j];
            }
        }

        return transposeMatrix;
    }

    public Vector4 GetColumn(int index)
    {
        if (index < 0 || index >= 4)
        {
            throw new IndexOutOfRangeException("Index out of range");
        }

        Vector4 columnVector = new Vector4();

        for (int i = 0; i < 4; i++)
        {
            columnVector[i] = this[i, index];
        }

        return columnVector;
    }

    public Vec3 GetPosition()
    {
        return new Vec3(m03, m13, m23);
    }

    public Vector4 GetRow(int index)
    {
        if (index < 0 || index >= 4)
        {
            throw new IndexOutOfRangeException("Index out of range");
        }

        Vector4 rowVector = new Vector4();

        for (int i = 0; i < 4; i++)
        {
            rowVector[i] = this[index, i];
        }

        return rowVector;
    }

    public Vec3 MultiplyPoint(Vec3 point)
    {
        Vec3 result = new Vec3();

        result.x = this[0, 0] * point.x + this[0, 1] * point.y + this[0, 2] * point.z + this[0, 3];
        result.y = this[1, 0] * point.x + this[1, 1] * point.y + this[1, 2] * point.z + this[1, 3];
        result.z = this[2, 0] * point.x + this[2, 1] * point.y + this[2, 2] * point.z + this[2, 3];

        float length = result.magnitude;

        result.x /= length;
        result.y /= length;
        result.z /= length;

        return result;
    }

    public Vec3 MultiplyPoint3x4(Vec3 point)
    {
        Vec3 result = new Vec3();

        result.x = this[0, 0] * point.x + this[0, 1] * point.y + this[0, 2] * point.z + this[0, 3];
        result.y = this[1, 0] * point.x + this[1, 1] * point.y + this[1, 2] * point.z + this[1, 3];
        result.z = this[2, 0] * point.x + this[2, 1] * point.y + this[2, 2] * point.z + this[2, 3];

        return result;
    }

    public Vec3 MultiplyVector(Vec3 vector)
    {
        Vec3 result = Vec3.Zero;

        result.x = m00 * vector.x + m01 * vector.y + m02 * vector.z;
        result.y = m10 * vector.x + m11 * vector.y + m12 * vector.z;
        result.z = m20 * vector.x + m21 * vector.y + m22 * vector.z;

        return result;
    }

    public void SetColumn(int index, Vector4 column)
    {
        this[0, index] = column.x;
        this[1, index] = column.y;
        this[2, index] = column.z;
        this[3, index] = column.w;
    }

    public void SetRow(int index, Vector4 row)
    {
        this[index, 0] = row.x;
        this[index, 1] = row.y;
        this[index, 2] = row.z;
        this[index, 3] = row.w;
    }

    public void SetTRS(Vec3 pos, MyQuaternion q, Vec3 s)
    {
        this = TRS(pos, q, s);
    }

    public bool ValidTRS()
    {
        if (m30 != 0 || m31 != 0 || m32 != 0 || m33 != 1)
        {
            return false;
        }

        // Rotation: The upper-left 3x3 submatrix should be an orthogonal matrix
        Vec3 column0 = new Vec3(m00, m10, m20);
        Vec3 column1 = new Vec3(m01, m11, m21);
        Vec3 column2 = new Vec3(m02, m12, m22);

        if (!Mathf.Approximately(Vec3.Dot(column0, column1), 0) ||
            !Mathf.Approximately(Vec3.Dot(column0, column2), 0) ||
            !Mathf.Approximately(Vec3.Dot(column1, column2), 0))
        {
            return false;
        }

        if (m00 < 0 || m11 < 0 || m22 < 0)
        {
            return false;
        }

        return true;
    }

    public bool Equals(MyMatrix4x4 other)
    {
        return GetColumn(0).Equals(other.GetColumn(0)) && GetColumn(1).Equals(other.GetColumn(1)) && GetColumn(2).Equals(other.GetColumn(2)) && GetColumn(3).Equals(other.GetColumn(3));
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            format = "F5";
        }

        if (formatProvider == null)
        {
            formatProvider = CultureInfo.InvariantCulture.NumberFormat;
        }

        return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n", m00.ToString(format, formatProvider), m01.ToString(format, formatProvider), m02.ToString(format, formatProvider), m03.ToString(format, formatProvider), m10.ToString(format, formatProvider), m11.ToString(format, formatProvider), m12.ToString(format, formatProvider), m13.ToString(format, formatProvider), m20.ToString(format, formatProvider), m21.ToString(format, formatProvider), m22.ToString(format, formatProvider), m23.ToString(format, formatProvider), m30.ToString(format, formatProvider), m31.ToString(format, formatProvider), m32.ToString(format, formatProvider), m33.ToString(format, formatProvider));

    }
    #endregion



}
