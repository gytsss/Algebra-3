using CustomMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicGrid : MonoBehaviour
{
    public GameObject pointPrefab;
    public float pointDistance = 1f;
    public int gridSize = 10;
    private Vec3[,,] pointPositions = new Vec3[0, 0, 0];
    [SerializeField] private bool draw = true;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        pointPositions = new Vec3[gridSize, gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    Vec3 position = new Vec3(i * pointDistance, j * pointDistance, k * pointDistance);

                    if (draw)
                    {
                        GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
                        point.transform.parent = transform;
                    }

                    pointPositions[i, j, k] = position;
                }
            }
        }
    }

    public Vec3[,,] GetPointPositions()
    {
        return pointPositions;
    }
}