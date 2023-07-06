using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public Vector3 eulerAngles
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
        // Calculate the vector part of the quaternion multiplied by the vector
        Vec3 vectorPart = new Vec3(rotation.x, rotation.y, rotation.z);
        Vec3 uv = Vec3.Cross(vectorPart, point);

        // Calculate the rotated vector by adding the vector part and twice the cross product of the vector part and the point
        return point + 2.0f * (rotation.w * uv + vectorPart * Vec3.Dot(vectorPart, point));
    }

    public static MyQuaternion operator *(MyQuaternion lhs, MyQuaternion rhs)
    {
        // Calculate the w component of the result quaternion
        float w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;

        // Calculate the x, y, and z components of the result quaternion
        float x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
        float y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z;
        float z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x;

        // Create and return a new quaternion using the calculated components
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
            // If the vectors are opposite, rotate about an arbitrary axis
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
        // Create a rotation that aligns the forward vector with the target direction
        MyQuaternion rotation = MyQuaternion.LookRotation(view);

        // Set the components of this quaternion to the components of the calculated rotation
        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void SetLookRotation(Vec3 view, Vec3 up)
    {
        // Create a rotation that aligns the forward vector with the target direction and the upwards vector with the target upwards direction
        MyQuaternion rotation = MyQuaternion.LookRotation(view, up);

        // Set the components of this quaternion to the components of the calculated rotation
        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void ToAngleAxis(out float angle, out Vec3 axis)
    {
        // Calculate the sine of half the angle
        float sinHalfAngle = Mathf.Sqrt(1.0f - w * w);

        // If the quaternion represents a rotation other than zero, calculate the angle and axis
        if (sinHalfAngle > 0.0001f)
        {
            angle = 2.0f * Mathf.Acos(w) * Mathf.Rad2Deg;
            axis = new Vec3(x, y, z) / sinHalfAngle;
        }
        else
        {
            // If the quaternion represents a zero rotation, return an angle of 0 and an arbitrary axis
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

        // Clamp the dot product to the range of [-1, 1] to avoid rounding errors when taking the arccosine
        dot = Mathf.Clamp(dot, -1.0f, 1.0f);

        // Calculate the angle between the two rotations using the arccosine of the dot product
        float angle = Mathf.Acos(dot) * 2.0f * Mathf.Rad2Deg;

        return angle;
    }

    public static MyQuaternion AngleAxis(float angle, Vec3 axis)
    {
        // Convert the angle to radians
        float radians = angle * Mathf.Deg2Rad;

        // Calculate the sine and cosine of half the angle
        float halfCos = Mathf.Cos(radians * 0.5f);
        float halfSin = Mathf.Sin(radians * 0.5f);

        // Create a quaternion using the half angle and axis
        MyQuaternion rotation = new MyQuaternion(
            axis.x * halfSin,
            axis.y * halfSin,
            axis.z * halfSin,
            halfCos);

        return rotation;
    }

    public static MyQuaternion AxisAngle(Vec3 axis, float angle)
    {
        // Normalize the axis vector
        axis.Normalize();

        // Calculate half the angle
        float halfAngle = angle * 0.5f;

        // Calculate the sin and cos of half the angle
        float sinHalfAngle = Mathf.Sin(halfAngle);
        float cosHalfAngle = Mathf.Cos(halfAngle);

        // Create a new quaternion using the axis and the sin/cos of half the angle
        return new MyQuaternion(axis.x * sinHalfAngle, axis.y * sinHalfAngle, axis.z * sinHalfAngle, cosHalfAngle);
    }

    public static float Dot(MyQuaternion a, MyQuaternion b)
    {
        // Calculate the dot product of the two quaternions
        float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        return dot;
    }

    public static MyQuaternion Euler(Vec3 euler)
    {
        // Convert the angles to radians
        float rx = euler.x * Mathf.Deg2Rad;
        float ry = euler.y * Mathf.Deg2Rad;
        float rz = euler.z * Mathf.Deg2Rad;

        // Calculate the sine and cosine of half the angles
        float cx = Mathf.Cos(rx * 0.5f);
        float cy = Mathf.Cos(ry * 0.5f);
        float cz = Mathf.Cos(rz * 0.5f);
        float sx = Mathf.Sin(rx * 0.5f);
        float sy = Mathf.Sin(ry * 0.5f);
        float sz = Mathf.Sin(rz * 0.5f);

        // Calculate the components of the quaternion
        float w = cx * cy * cz + sx * sy * sz;
        float x = sx * cy * cz - cx * sy * sz;
        float y = cx * sy * cz + cy * sx * sz;
        float z = cx * cy * sz - sx * sy * cz;

        // Create a quaternion using the components
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
        // Calculate the cross product between the two vectors
        Vec3 cross = Vec3.Cross(fromDirection, toDirection);

        // Calculate the dot product between the two vectors
        float dot = Vec3.Dot(fromDirection, toDirection);

        // If the vectors are parallel, return the identity quaternion
        if (dot > 0.99999f)
        {
            return MyQuaternion.identity;
        }
        else if (dot < -0.99999f)
        {
            // If the vectors are opposite, rotate about an arbitrary axis
            Vec3 axis = Vec3.Cross(Vec3.Up, fromDirection);
            if (axis.magnitude < 0.01f)
            {
                axis = Vec3.Cross(Vec3.Right, fromDirection);
            }
            axis.Normalize();
            return AngleAxis(180.0f, axis);
        }
        else
        {
            // Calculate the angle of rotation and the axis of rotation
            float angle = Mathf.Acos(dot);
            Vec3 axis = cross.normalized;

            // Create a quaternion using the angle and axis
            return AngleAxis(angle * Mathf.Rad2Deg, axis);
        }
    }

    public static MyQuaternion Inverse(MyQuaternion rotation)
    {
        // Invert the x, y, and z components of the quaternion
        MyQuaternion inverse = new MyQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);

        return inverse;
    }

    public static MyQuaternion Lerp(MyQuaternion a, MyQuaternion b, float t)
    {
        t = Mathf.Clamp01(t);
        float invT = 1f - t;

        float w = a.w * invT + b.w * t;
        float x = a.x * invT + b.x * t;
        float y = a.y * invT + b.y * t;
        float z = a.z * invT + b.z * t;

        return new MyQuaternion(x, y, z, w);
    }

    public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        float invT = 1f - t;

        float w = a.w * invT + b.w * t;
        float x = a.x * invT + b.x * t;
        float y = a.y * invT + b.y * t;
        float z = a.z * invT + b.z * t;

        return new MyQuaternion(x, y, z, w);
    }

    public static MyQuaternion LookRotation(Vec3 forward, Vec3 upwards)
    {
        MyQuaternion rotation = MyQuaternion.identity;

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
        // Call the overload of LookRotation that takes an upwards vector with the default upwards vector
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
        float angle = MyQuaternion.Angle(from, to);
        float t = Mathf.Min(1.0f, maxDegreesDelta / angle);
        MyQuaternion result = MyQuaternion.Slerp(from, to, t);
        return result;
    }

    public static MyQuaternion Slerp(MyQuaternion a, MyQuaternion b, float t)
    {
        float dot = MyQuaternion.Dot(a, b);

        // Adjusting the sign of one quaternion if necessary
        if (dot < 0)
        {
            b = new MyQuaternion(-b.x, -b.y, -b.z, -b.w);
            dot = -dot;
        }

        // Checking if the quaternions are close to each other and returning the result
        if (dot > 0.9995f)
        {
            return MyQuaternion.LerpUnclamped(a, b, t);
        }

        // Calculating the angle between the quaternions
        float theta = Mathf.Acos(dot);
        float invSinTheta = 1f / Mathf.Sin(theta);

        // Calculating the weights for the interpolation
        float weightA = Mathf.Sin((1 - t) * theta) * invSinTheta;
        float weightB = Mathf.Sin(t * theta) * invSinTheta;

        // Interpolating between the quaternions using the calculated weights
        return new MyQuaternion(
            weightA * a.x + weightB * b.x,
            weightA * a.y + weightB * b.y,
            weightA * a.z + weightB * b.z,
            weightA * a.w + weightB * b.w);
    }

    public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        // Calculating the dot product between both quaternions
        float dot = MyQuaternion.Dot(a, b);

        // Adjusting the sign of one quaternion if necessary
        if (dot < 0)
        {
            b = new MyQuaternion(-b.x, -b.y, -b.z, -b.w);
            dot = -dot;
        }

        // Calculating the angle between the quaternions
        float theta = Mathf.Acos(dot);
        float invSinTheta = 1f / Mathf.Sin(theta);

        // Calculating the weights for the interpolation
        float weightA = Mathf.Sin((1 - t) * theta) * invSinTheta;
        float weightB = Mathf.Sin(t * theta) * invSinTheta;

        // Interpolating between the quaternions using the calculated weights
        return new MyQuaternion(
            weightA * a.x + weightB * b.x,
            weightA * a.y + weightB * b.y,
            weightA * a.z + weightB * b.z,
            weightA * a.w + weightB * b.w
        );
    }

    public bool Equals(MyQuaternion other)
    {
        throw new NotImplementedException();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        throw new NotImplementedException();
    }
    #endregion

}
