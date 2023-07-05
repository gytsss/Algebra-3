using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct MyQuaternion 
{
    #region Variables
    public float x;
    public float y;
    public float z;
    public float w;

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

    public Vector3 eulerAngles
    {
        get
        {
            return Internal_MakePositive(QuaternionToEuler(this));
        }
        set
        {
            this = Euler(value);
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
    #endregion

    public static MyQuaternion identity => new MyQuaternion(0, 0, 0, 1);

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
    public static Vector3 operator *(MyQuaternion rotation, Vector3 point)
    {
        // Calculate the vector part of the quaternion multiplied by the vector
        Vector3 vectorPart = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 uv = Vector3.Cross(vectorPart, point);

        // Calculate the rotated vector by adding the vector part and twice the cross product of the vector part and the point
        return point + 2.0f * (rotation.w * uv + vectorPart * Vector3.Dot(vectorPart, point));
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

    public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        Vector3 cross = Vector3.Cross(fromDirection, toDirection);
        float dot = Vector3.Dot(fromDirection, toDirection);

        if (dot < -0.999999f)
        {
            // If the vectors are opposite, rotate about an arbitrary axis
            Vector3 axis = Vector3.up;
            if (Mathf.Abs(fromDirection.x) < 0.1f && Mathf.Abs(fromDirection.z) < 0.1f)
            {
                axis = Vector3.Cross(fromDirection, Vector3.right).normalized;
            }
            else
            {
                axis = Vector3.Cross(fromDirection, Vector3.up).normalized;
            }
            SetAxisAngle(axis, 180.0f);
        }
        else
        {
            float angle = Mathf.Acos(Mathf.Clamp(dot, -1.0f, 1.0f));
            SetAxisAngle(cross.normalized, angle * Mathf.Rad2Deg);
        }
    }

    public void SetLookRotation(Vector3 view)
    {
        // Create a rotation that aligns the forward vector with the target direction
        MyQuaternion rotation = MyQuaternion.LookRotation(view);

        // Set the components of this quaternion to the components of the calculated rotation
        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void SetLookRotation(Vector3 view, Vector3 up)
    {
        // Create a rotation that aligns the forward vector with the target direction and the upwards vector with the target upwards direction
        MyQuaternion rotation = MyQuaternion.LookRotation(view, up);

        // Set the components of this quaternion to the components of the calculated rotation
        x = rotation.x;
        y = rotation.y;
        z = rotation.z;
        w = rotation.w;
    }

    public void ToAngleAxis(out float angle, out Vector3 axis)
    {
        // Calculate the sine of half the angle
        float sinHalfAngle = Mathf.Sqrt(1.0f - w * w);

        // If the quaternion represents a rotation other than zero, calculate the angle and axis
        if (sinHalfAngle > 0.0001f)
        {
            angle = 2.0f * Mathf.Acos(w) * Mathf.Rad2Deg;
            axis = new Vector3(x, y, z) / sinHalfAngle;
        }
        else
        {
            // If the quaternion represents a zero rotation, return an angle of 0 and an arbitrary axis
            angle = 0.0f;
            axis = Vector3.up;
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

    public static MyQuaternion AngleAxis(float angle, Vector3 axis)
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

    public static float Dot(MyQuaternion a, MyQuaternion b)
    {
        // Calculate the dot product of the two quaternions
        float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        return dot;
    }

    public static MyQuaternion Euler(float x, float y, float z)
    {
        // Convert the angles to radians
        float rx = x * Mathf.Deg2Rad;
        float ry = y * Mathf.Deg2Rad;
        float rz = z * Mathf.Deg2Rad;

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

    public static MyQuaternion Euler(Vector3 euler)
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

    public static MyQuaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        // Calculate the cross product between the two vectors
        Vector3 cross = Vector3.Cross(fromDirection, toDirection);

        // Calculate the dot product between the two vectors
        float dot = Vector3.Dot(fromDirection, toDirection);

        // If the vectors are parallel, return the identity quaternion
        if (dot > 0.99999f)
        {
            return MyQuaternion.identity;
        }
        else if (dot < -0.99999f)
        {
            // If the vectors are opposite, rotate about an arbitrary axis
            Vector3 axis = Vector3.Cross(Vector3.up, fromDirection);
            if (axis.magnitude < 0.01f)
            {
                axis = Vector3.Cross(Vector3.right, fromDirection);
            }
            axis.Normalize();
            return AngleAxis(180.0f, axis);
        }
        else
        {
            // Calculate the angle of rotation and the axis of rotation
            float angle = Mathf.Acos(dot);
            Vector3 axis = cross.normalized;

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
        MyQuaternion result = new MyQuaternion(0.0f, 0.0f, 0.0f, 1.0f);

        // Interpolate between the two quaternions
        result.x = Mathf.Lerp(a.x, b.x, t);
        result.y = Mathf.Lerp(a.y, b.y, t);
        result.z = Mathf.Lerp(a.z, b.z, t);
        result.w = Mathf.Lerp(a.w, b.w, t);

        // Normalize the result quaternion
        result.Normalize();

        return result;
    }

    public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        MyQuaternion result = new MyQuaternion(0.0f, 0.0f, 0.0f, 1.0f);

        // Interpolate between the two quanternions
        result.x = Mathf.LerpUnclamped(a.x, b.x, t);
        result.y = Mathf.LerpUnclamped(a.y, b.y, t);
        result.z = Mathf.LerpUnclamped(a.z, b.z, t);
        result.w = Mathf.LerpUnclamped(a.w, b.w, t);

        // Normalize the result quaternion
        result.Normalize();

        return result;
    }

    public static MyQuaternion LookRotation(Vector3 forward, Vector3 upwards)
    {
        MyQuaternion rotation = MyQuaternion.identity;

        if (forward.magnitude > 0.0f)
        {
            Vector3 right = Vector3.Cross(upwards, forward).normalized;
            upwards = Vector3.Cross(forward, right).normalized;
            rotation = new MyQuaternion(upwards.x, upwards.y, upwards.z, right.x + forward.z);
        }

        return rotation;
    }

    public static MyQuaternion LookRotation(Vector3 forward)
    {
        // Call the overload of LookRotation that takes an upwards vector with the default upwards vector
        return LookRotation(forward, Vector3.up);
    }

    public static MyQuaternion Normalize(MyQuaternion q)
    {
        return q.normalized;
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
        // Calculate the cosine of the angle between the two quaternions
        float cosTheta = MyQuaternion.Dot(a, b);

        // If the quaternions are in opposite hemispheres, flip one of them
        if (cosTheta < 0.0f)
        {
            b = new MyQuaternion(-b.x, -b.y, -b.z, -b.w);
            cosTheta = -cosTheta;
        }

        // If the quaternions are very close, just return one of them
        if (cosTheta > 0.9995f)
        {
            return MyQuaternion.Lerp(a, b, t);
        }
        else
        {
            // Calculate the angle between the two quaternions
            float theta = Mathf.Acos(cosTheta);

            // Calculate the sin of the angle between the two quaternions
            float sinTheta = Mathf.Sin(theta);

            // Calculate the weightings for the two quaternions
            float w1 = Mathf.Sin((1.0f - t) * theta) / sinTheta;
            float w2 = Mathf.Sin(t * theta) / sinTheta;

            // Interpolate between the two quaternions using the weightings
            MyQuaternion result = new MyQuaternion(
                a.x * w1 + b.x * w2,
                a.y * w1 + b.y * w2,
                a.z * w1 + b.z * w2,
                a.w * w1 + b.w * w2);

            // Normalize the result quaternion
            result.Normalize();

            return result;
        }
    }

    public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        // Calculate the cosine of the angle between the two quaternions
        float cosTheta = MyQuaternion.Dot(a, b);

        // If the quaternions are in opposite hemispheres, flip one of them
        if (cosTheta < 0.0f)
        {
            b = new MyQuaternion(-b.x, -b.y, -b.z, -b.w);
            cosTheta = -cosTheta;
        }

        // Calculate the angle between the two quaternions
        float theta = Mathf.Acos(cosTheta);

        // Calculate the sin of the angle between the two quaternions
        float sinTheta = Mathf.Sin(theta);

        // Calculate the weightings for the two quaternions
        float w1 = Mathf.Sin((1.0f - t) * theta) / sinTheta;
        float w2 = Mathf.Sin(t * theta) / sinTheta;

        // Interpolate between the two quaternions using the weightings
        MyQuaternion result = new MyQuaternion(
            a.x * w1 + b.x * w2,
            a.y * w1 + b.y * w2,
            a.z * w1 + b.z * w2,
            a.w * w1 + b.w * w2);

        // Normalize the result quaternion
        result.Normalize();

        return result;
    }
    #endregion

}
