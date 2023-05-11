using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicGrid : MonoBehaviour
{
    public GameObject pointPrefab;
    public float pointDistance = 1f;
    public int gridSize = 10;
    private Vector3[,,] pointPositions;

    // Start is called before the first frame update
    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        pointPositions = new Vector3[gridSize, gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 position = new Vector3(x * pointDistance, y * pointDistance, z * pointDistance);
                    GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
                    point.transform.parent = transform;

                    pointPositions[x, y, z] = position;
                }
            }
        }
    }

    public Vector3[,,] GetPointPositions()
    {
        return pointPositions;
    }
}