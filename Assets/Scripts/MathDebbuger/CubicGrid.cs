using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicGrid : MonoBehaviour
{
    public GameObject pointPrefab;
    public float pointDistance = 1f;
    public int gridSize = 10;
    private Vector3[,,] pointPositions = new Vector3[0, 0, 0];
    [SerializeField] private bool draw = true;

    // Start is called before the first frame update
    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        pointPositions = new Vector3[gridSize, gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    Vector3 position = new Vector3(i * pointDistance, j * pointDistance, k * pointDistance);

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

    public Vector3[,,] GetPointPositions()
    {
        return pointPositions;
    }

    public List<Vector3> CheckCubeCollision(Vector3 upPlane, Vector3 downPlane, Vector3 rightPlane, Vector3 leftPlane, Vector3 frontPlane, Vector3 backPlane)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    Vector3 position = pointPositions[i, j, k];
                    bool insideCube = true;

                    if (Vector3.Dot(position - upPlane, Vector3.up) < 0)
                    {
                        insideCube = false;
                    }
                    if (Vector3.Dot(position - downPlane, Vector3.down) < 0)
                    {
                        insideCube = false;
                    }
                    if (Vector3.Dot(position - rightPlane, Vector3.right) < 0)
                    {
                        insideCube = false;
                    }
                    if (Vector3.Dot(position - leftPlane, Vector3.left) < 0)
                    {
                        insideCube = false;
                    }
                    if (Vector3.Dot(position - frontPlane, Vector3.forward) < 0)
                    {
                        insideCube = false;
                    }
                    if (Vector3.Dot(position - backPlane, Vector3.back) < 0)
                    {
                        insideCube = false;
                    }

                    if (insideCube)
                    {
                        points.Add(position);
                    }
                }
            }
        }

        return points;
    }
}