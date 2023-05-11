using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MyPlane : MonoBehaviour
{
    public float _distance;
    public Vector3 _normal;
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

    public Vector3 normal
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


    public MyPlane(Vector3 inNormal, float distance) 
    {
        normal = inNormal;
        this.distance = distance;
    }
    public MyPlane(Vector3 inNormal, Vector3 inPoint) 
    {
        normal = inNormal;
        distance = -Vector3.Dot(inNormal, inPoint);
    }

    public MyPlane(Vector3 a, Vector3 b, Vector3 c) 
    {
        normal = Vector3.Cross(b - a, c - a).normalized;
        distance = -Vector3.Dot(normal, a);
    }

    public Vector3 ClosestPointOnPlane(Vector3 point)
    {
        float distanceToPoint = GetDistanceToPoint(point);
        return point - normal * distanceToPoint;
    }

    public void Flip()
    {
        normal *= -1f;
        distance *= -1f;
    }

    public float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(normal, point) + distance;
    }

    public bool GetSide(Vector3 point)
    {
        return Vector3.Dot(normal, point) + distance > 0;
    }

    public bool Raycast(Ray ray, out float enter)
    {
        enter = 0f;
        float rayDistance;
        if (new MyPlane(normal, transform.position).Raycast(ray, out rayDistance))
        {
            Vector3 planePoint = ray.origin + ray.direction * rayDistance;
            Vector3 rayEndPoint = planePoint - normal * GetDistanceToPoint(ray.origin);
            if (Vector3.Dot(rayEndPoint - ray.origin, ray.direction) >= 0f)
            {
                enter = rayDistance;
                return true;
            }
        }
        return false;
    }

    public bool SameSide(Vector3 inPt0, Vector3 inPt1)
    {
        float d0 = GetDistanceToPoint(inPt0);
        float d1 = GetDistanceToPoint(inPt1);
        return d0 >= 0f && d1 >= 0f || d0 < 0f && d1 < 0f;
    }

    public void Set3Points(Vector3 a, Vector3 b, Vector3 c) 
    {
        normal = Vector3.Cross(b - a, c - a).normalized;
        distance = -Vector3.Dot(normal, a);
    }

    public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint) 
    {
        normal = inNormal;
        distance = -Vector3.Dot(inNormal, inPoint);
    }

    public static MyPlane Translate(MyPlane plane, Vector3 translation)
    {
        plane.distance += Vector3.Dot(plane.normal, translation);
        return plane;
    }
}
