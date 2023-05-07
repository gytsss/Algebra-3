using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CustomMath
{
    public struct Vec3 : IEquatable<Vec3>
    {
        #region Variables
        public float x;
        public float y;
        public float z;

        public float sqrMagnitude { get { return x * x + y * y + z * z; } } //Es util porque es mas rapido que calcular la magnitud en si / calcular las longitudes
        public Vec3 normalized  //Vector con misma direccion pero con mag 1
        {
            get
            {
                float mag = this.magnitude;
                if (mag > Mathf.Epsilon)
                    return this / mag;
                else
                    return Zero;
            }
        }

        public float magnitude { get { return Mathf.Sqrt(sqrMagnitude); } } //Longitud del vector ultilizando pitagoras
        #endregion

        #region constants
        public const float epsilon = 1e-05f; //Pequeño numero que se utiliza para la comparacion de floats
        #endregion

        #region Default Values
        public static Vec3 Zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 One { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 Forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 Back { get { return new Vec3(0.0f, 0.0f, -1.0f); } }
        public static Vec3 Right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 Left { get { return new Vec3(-1.0f, 0.0f, 0.0f); } }
        public static Vec3 Up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 Down { get { return new Vec3(0.0f, -1.0f, 0.0f); } }
        
        //Se setea el valor mas grande que un float puede almacenar, se utiliza como valor maximo en algunos calculos
        public static Vec3 PositiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }

        //Se setea el valor mas chico que un float puede almacenar, se utiliza como valor minimo en algunos calculos
        public static Vec3 NegativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion                                                                                                                                                                               

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            this.x = v2.x;
            this.y = v2.y;
            this.z = 0.0f;
        }
        #endregion

        #region Operators
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < epsilon * epsilon;
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(-v3.x, -v3.y, -v3.z);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }

        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return v3 * scalar;
        }

        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        public static implicit operator Vector3(Vec3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        public static implicit operator Vector2(Vec3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }
        public static float Angle(Vec3 from, Vec3 to)  //Angulo entre 2 vectores
        {
            float cosTheta = Vec3.Dot(from.normalized, to.normalized);
            float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
            return theta;
        }

        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength) //Vector con una magnitud limitada, si es mayor a x magnitud
        {
            if (vector.magnitude > maxLength)
            {
                return vector.normalized * maxLength;
            }
            else
            {
                return vector;
            }
        }

        public static float Magnitude(Vec3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static Vec3 Cross(Vec3 a, Vec3 b) //Producto cruz entre los vectores
        {
            return new Vec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public static float Distance(Vec3 a, Vec3 b)
        {
            return (a - b).magnitude;
        }

        public static float Dot(Vec3 a, Vec3 b) //Producto punto entre los vectores
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3 Lerp(Vec3 a, Vec3 b, float t) //Vector interpolado entre otros 2
        {
            t = Mathf.Clamp01(t);
            return a + (b - a) * t;
        }

        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vec3 Max(Vec3 a, Vec3 b) //Vector que tiene valor maximo en cada una de las componentes
        {
            return new Vec3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Vec3 Min(Vec3 a, Vec3 b) //Vector que tiene valor minimo en cada una de las componentes
        {
            return new Vec3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public static float SqrMagnitude(Vec3 vector) //Magnitud al cuadrado
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        public static Vec3 Project(Vec3 vector, Vec3 onNormal) //Proyecta el vector en la direccion del vector normalizado
        {
            float sqrMag = Vec3.Dot(onNormal, onNormal);

            if (sqrMag < Mathf.Epsilon)
            {
                return Zero;
            }

            float dot = Vec3.Dot(vector, onNormal);
            return onNormal * dot / sqrMag;
        }

        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal) //Refleja un vector en una superficie con normal
        {
            return inDirection - 2.0f * Dot(inDirection, inNormal) * inNormal;
        }

        public void Set(float newX, float newY, float newZ) //Setear los valores de un vector
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public void Scale(Vec3 scale) //Escalar un vector
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public void Normalize() //Normaliza el vector, para tener una magnitud de 1, misma direc
        {
            float mag = Magnitude(this);

            if (mag > Mathf.Epsilon)
            {
                this /= mag;
            }
            else
            {
                this = Zero;
            }
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other) //Compara 2 vectores, true si son iguales
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode() //Devuelve un numero de hash unico basado en x,y,z
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }
}