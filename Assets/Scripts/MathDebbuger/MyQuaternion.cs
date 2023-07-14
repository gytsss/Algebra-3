using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public struct MyQuaternion : IEquatable<MyQuaternion>, IFormattable
{
    #region StaticProperties
    public static MyQuaternion identity => new MyQuaternion(0, 0, 0, 1);
    #endregion

    #region Properties
    public float x;
    public float y;
    public float z;
    public float w;
    public Vec3 eulerAngles
    {
        get
        {
            float pitch = Mathf.Atan2(2 * (w * x + y * z), 1 - 2 * (x * x + y * y)) * Mathf.Rad2Deg;
            float yaw = Mathf.Atan2(2 * (w * y - z * x), 1 - 2 * (y * y + x * x)) * Mathf.Rad2Deg;
            float roll = Mathf.Asin(2 * (w * z + x * y)) * Mathf.Rad2Deg;
            return new Vec3(pitch, yaw, roll);
        }
    }
    public Quaternion normalized
    {
        get
        {
            Quaternion q = new Quaternion(x, y, z, w);
            float mag = Mathf.Sqrt(w * w + x * x + y * y + z * z);
            q.x /= mag;
            q.y /= mag;
            q.z /= mag;
            q.w /= mag;
            return q;
        }
    }
    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                case 3:
                    return w;
                default:
                    throw new IndexOutOfRangeException("Quaternion index out of range: " + index);
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Quaternion index out of range: " + index);
            }
        }
    }
    #endregion

    #region Constructor
    public MyQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    #endregion

    #region Operators
    public static Vec3 operator *(MyQuaternion rotation, Vec3 point)
    {
        Vec3 vectorPart = new Vec3(rotation.x, rotation.y, rotation.z);
        Vec3 v = Vec3.Cross(vectorPart, point);
         
        return point + 2.0f * (rotation.w * v + vectorPart * Vec3.Dot(vectorPart, point));
    }

    public static MyQuaternion operator *(MyQuaternion lhs, MyQuaternion rhs)
    {
        float w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;

        float x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
        float y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z;
        float z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x;

        return new MyQuaternion(x, y, z, w);
    }

    public static bool operator ==(MyQuaternion lhs, MyQuaternion rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;

    public static bool operator !=(MyQuaternion lhs, MyQuaternion rhs) => !(lhs == rhs);
    #endregion

    #region PublicMethods
    public void Set(float newX, float newY, float newZ, float newW)
    {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
    }

    public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        Vec3 cross = Vec3.Cross(fromDirection, toDirection);
        float dot = Vec3.Dot(fromDirection, toDirection);

        if (dot < -0.999999f)
        {
            Vec3 axis = Vec3.Up;
            if (Mathf.Abs(fromDirection.x) < 0.1f && Mathf.Abs(fromDirection.z) < 0.1f)
            {
                axis = Vec3.Cross(fromDirection, Vec3.Right).normalized;
            }
            else
            {
                axis = Vec3.Cross(fromDirection, Vec3.Up).normalized;
            }
            AxisAngle(axis, 180.0f);
        }
        else
        {
            float angle = Mathf.Acos(Mathf.Clamp(dot, -1.0f, 1.0f));
            AxisAngle(cross.normalized, angle * Mathf.Rad2Deg);
        }
    }

    public void SetLookRotation(Vec3 view)
    {
        MyQuaternion rotation = LookRotation(view);

        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void SetLookRotation(Vec3 view, Vec3 up)
    {
        MyQuaternion rotation = LookRotation(view, up);

        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void ToAngleAxis(out float angle, out Vec3 axis)
    {
        angle = 2.0f * Mathf.Acos(w) * Mathf.Rad2Deg;
        float mag = Mathf.Sqrt(1.0f - w * w);

        if (mag > 0.0001f)
        {
            axis = new Vec3(x, y, z) / mag;
        }
        else
        {
            angle = 0.0f;
            axis = Vec3.Up;
        }
    }

    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
    }
    #endregion

    #region Functions  
    public static float Angle(MyQuaternion a, MyQuaternion b)
    {
        float dot = Dot(a, b);
        dot = Mathf.Clamp(dot, -1.0f, 1.0f);
        float angle = Mathf.Acos(dot) * 2.0f * Mathf.Rad2Deg;
        return angle;
    }

    public static MyQuaternion AngleAxis(float angle, Vec3 axis)
    {
        float radians = angle * Mathf.Deg2Rad;

        float halfCos = Mathf.Cos(radians * 0.5f);
        float halfSin = Mathf.Sin(radians * 0.5f);

        MyQuaternion rotation = new MyQuaternion(
            axis.x * halfSin,
            axis.y * halfSin,
            axis.z * halfSin,
            halfCos);

        return rotation;
    }

    public static MyQuaternion AxisAngle(Vec3 axis, float angle)
    {
        axis.Normalize();

        float halfAngle = angle * 0.5f;

        float sinHalfAngle = Mathf.Sin(halfAngle);
        float cosHalfAngle = Mathf.Cos(halfAngle);

        return new MyQuaternion(axis.x * sinHalfAngle, axis.y * sinHalfAngle, axis.z * sinHalfAngle, cosHalfAngle);
    }

    public static float Dot(MyQuaternion a, MyQuaternion b)
    {
        float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        return dot;
    }

    public static MyQuaternion Euler(Vec3 euler)
    {
        float rx = euler.x * Mathf.Deg2Rad;
        float ry = euler.y * Mathf.Deg2Rad;
        float rz = euler.z * Mathf.Deg2Rad;

        float cx = Mathf.Cos(rx * 0.5f);
        float cy = Mathf.Cos(ry * 0.5f);
        float cz = Mathf.Cos(rz * 0.5f);
        float sx = Mathf.Sin(rx * 0.5f);
        float sy = Mathf.Sin(ry * 0.5f);
        float sz = Mathf.Sin(rz * 0.5f);

        float w = cx * cy * cz + sx * sy * sz;
        float x = sx * cy * cz - cx * sy * sz;
        float y = cx * sy * cz + cy * sx * sz;
        float z = cx * cy * sz - sx * sy * cz;

        MyQuaternion rotation = new MyQuaternion(x, y, z, w);

        return rotation;
    }

    public static MyQuaternion Euler(float x, float y, float z)
    {
        float xRad = x * Mathf.Deg2Rad * 0.5f;
        float yRad = y * Mathf.Deg2Rad * 0.5f;
        float zRad = z * Mathf.Deg2Rad * 0.5f;

        float sinX = Mathf.Sin(xRad);
        float cosX = Mathf.Cos(xRad);
        float sinY = Mathf.Sin(yRad);
        float cosY = Mathf.Cos(yRad);
        float sinZ = Mathf.Sin(zRad);
        float cosZ = Mathf.Cos(zRad);

        float w = cosX * cosY * cosZ + sinX * sinY * sinZ;
        float xValue = sinX * cosY * cosZ - cosX * sinY * sinZ;
        float yValue = cosX * sinY * cosZ + sinX * cosY * sinZ;
        float zValue = cosX * cosY * sinZ - sinX * sinY * cosZ;

        return new MyQuaternion(xValue, yValue, zValue, w);
    }

    public static MyQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        Vec3 axis = Vec3.Cross(fromDirection, toDirection);
        float angle = Vec3.Angle(fromDirection, toDirection);
        return AngleAxis(angle, axis.normalized);
    }

    public static MyQuaternion Inverse(MyQuaternion rotation)
    {
        return new MyQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
    }

    public static MyQuaternion Lerp(MyQuaternion a, MyQuaternion b, float t)
    {
        MyQuaternion result = identity;

        t = Mathf.Clamp01(t);
        float invT = 1f - t;

        result.w = a.w * invT + b.w * t;
        result.x = a.x * invT + b.x * t;
        result.y = a.y * invT + b.y * t;
        result.z = a.z * invT + b.z * t;

        Normalize(result);

        return result;
    }

    public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        MyQuaternion result = identity;

        float invT = 1f - t;

        if (Dot(a, b) >= 0f)
        {
            result.x = a.x * invT + b.x * t;
            result.y = a.y * invT + b.y * t;
            result.z = a.z * invT + b.z * t;
            result.w = a.w * invT + b.w * t;
        }
        else
        {
            result.x = a.x * invT - b.x * t;
            result.y = a.y * invT - b.y * t;
            result.z = a.z * invT - b.z * t;
            result.w = a.w * invT - b.w * t;
        }

        Normalize(result);

        return result;
    }

    public static MyQuaternion LookRotation(Vec3 forward, Vec3 upwards)
    {
        MyQuaternion rotation = identity;

        if (forward.magnitude > 0.0f)
        {
            Vec3 right = Vec3.Cross(upwards, forward).normalized;
            upwards = Vec3.Cross(forward, right).normalized;
            rotation = new MyQuaternion(upwards.x, upwards.y, upwards.z, right.x + forward.z);
        }

        return rotation;
    }

    public static MyQuaternion LookRotation(Vec3 forward)
    {
        return LookRotation(forward, Vec3.Up);
    }

    public static MyQuaternion Normalize(MyQuaternion q)
    {
        float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        float invMagnitude = 1f / magnitude;

        return new MyQuaternion(
            q.x * invMagnitude,
            q.y * invMagnitude,
            q.z * invMagnitude,
            q.w * invMagnitude
        );
    }

    public static MyQuaternion RotateTowards(MyQuaternion from, MyQuaternion to, float maxDegreesDelta)
    {
        float angle = Angle(from, to);
        float t = Mathf.Min(1.0f, maxDegreesDelta / angle);
        MyQuaternion result = Slerp(from, to, t);
        return result;
    }

    public static MyQuaternion Slerp(MyQuaternion a, MyQuaternion b, float t)
    {
        t = Mathf.Clamp01(t);

        return SlerpUnclamped(a, b, t);
    }

    public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        float dot = Dot(a, b);

        if (dot < 0)
        {
            b = new MyQuaternion(-b.x, -b.y, -b.z, b.w);
            dot = -dot;
        }

        if (dot > 0.9995f)
        {
            return LerpUnclamped(a, b, t);
        }

        float angle = Mathf.Acos(dot);
        float invSinAngle = 1f / Mathf.Sin(angle);

        float weightA = Mathf.Sin((1 - t) * angle) * invSinAngle;
        float weightB = Mathf.Sin(t * angle) * invSinAngle;

        return new MyQuaternion(
            weightA * a.x + weightB * b.x,
            weightA * a.y + weightB * b.y,
            weightA * a.z + weightB * b.z,
            weightA * a.w + weightB * b.w
        );
    }

    public bool Equals(MyQuaternion other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
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

        return string.Format("({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
    }
    #endregion

}
