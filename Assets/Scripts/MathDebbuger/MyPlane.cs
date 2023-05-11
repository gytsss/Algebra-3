using CustomMath;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MyPlane : MonoBehaviour
{
    public float _distance;
    public Vec3 _normal;
    public MyPlane _flipped;

    public float distance
    {
        get
        {
            return _distance;
        }
        set
        {
            _distance = value;
        }
    }

    public Vec3 normal
    {
        get
        {
            return _normal;
        }
        set
        {
            _normal = value.normalized;
        }
    }


    public MyPlane(Vec3 inNormal, float distance)
    {
        normal = inNormal;
        this.distance = distance;
    }
    public MyPlane(Vec3 inNormal, Vec3 inPoint)
    {
        normal = inNormal;
        distance = -Vec3.Dot(inNormal, inPoint);
    }

    public MyPlane(Vec3 a, Vec3 b, Vec3 c)
    {
        normal = Vec3.Cross(b - a, c - a).normalized;
        distance = -Vec3.Dot(normal, a);
    }

    public Vec3 ClosestPointOnPlane(Vec3 point)
    {
        float distanceToPoint = GetDistanceToPoint(point);
        return point - normal * distanceToPoint;
    }

    public void Flip()
    {
        normal *= -1f;
        distance *= -1f;
    }

    public float GetDistanceToPoint(Vec3 point)
    {
        return Vec3.Dot(normal, point) + distance;
    }

    public bool GetSide(Vec3 point)
    {
        return Vec3.Dot(normal, point) + distance > 0;
    }

    public bool Raycast(Ray ray, out float enter)
    {
        enter = 0f;
        float rayDistance;
        if (new MyPlane(normal, transform.position).Raycast(ray, out rayDistance))
        {
            Vec3 planePoint = ray.origin + ray.direction * rayDistance;
            Vec3 rayEndPoint = planePoint - normal * GetDistanceToPoint(ray.origin);
            if (Vec3.Dot(rayEndPoint - ray.origin, ray.direction) >= 0f)
            {
                enter = rayDistance;
                return true;
            }
        }
        return false;
    }

    public bool SameSide(Vec3 inPt0, Vec3 inPt1)
    {
        float d0 = GetDistanceToPoint(inPt0);
        float d1 = GetDistanceToPoint(inPt1);
        return d0 >= 0f && d1 >= 0f || d0 < 0f && d1 < 0f;
    }

    public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
    {
        normal = Vec3.Cross(b - a, c - a).normalized;
        distance = -Vec3.Dot(normal, a);
    }

    public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
    {
        normal = inNormal;
        distance = -Vec3.Dot(inNormal, inPoint);
    }

    public void Translate(float distance)
    {
        this.distance += distance;
    }
}
