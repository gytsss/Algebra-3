using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    MyMeshCollider[] colliders;

    private void Update()
    {
        colliders = FindObjectsOfType<MyMeshCollider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].id = i;
        }

        ComparePoints();
    }

    private void ComparePoints()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = i; j < colliders.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }

                for (int k = 0; k < colliders[i].points.Count; k++)
                {
                    if (colliders[j].points.Contains(colliders[i].points[k]))
                    {
                        colliders[i].CollisionStay();
                        colliders[j].CollisionStay();
                    }
                }
            }
        }
    }
}
