using CustomMath;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class MyMeshCollider : MonoBehaviour
{
    struct Face
    {
        public Vec3[] vertices;
        public Plane plane;

        public Face(Vec3[] vertices)
        {
            this.vertices = vertices;
            plane = new Plane(vertices[0], vertices[1], vertices[2]);
        }
    }

    CubicGrid grid;
    public float size = 1f;
    public int id;

    Mesh mesh;
    public List<Vec3> points = new List<Vec3>();
    List<Face> faces = new List<Face>();

    Transform prevTrans;

    private void Start()
    {
        prevTrans = transform;
    }

    private void Update()
    {
        if (prevTrans != transform)
            PositionUpdated();

        grid = FindObjectOfType<CubicGrid>();
        mesh = GetComponent<MeshFilter>().sharedMesh;

        GetFaces();
        CheckPointsInside();

        DrawPoint(points, size);
    }

    private void PositionUpdated()
    {
        prevTrans.position = transform.position;
    }

    private void GetFaces()
    {
        int[] vertexIndex = mesh.triangles;

        faces.Clear();

        for (int i = 0; i < vertexIndex.Length; i += 3)
        {
            Vec3 vertex1 = transform.TransformPoint(mesh.vertices[vertexIndex[i]]);
            Vec3 vertex2 = transform.TransformPoint(mesh.vertices[vertexIndex[i + 1]]);
            Vec3 vertex3 = transform.TransformPoint(mesh.vertices[vertexIndex[i + 2]]);

            Face face = new Face(new[] { vertex1, vertex2, vertex3 });

            if (!faces.Contains(face))
                faces.Add(face);

        }
    }

    public void CollisionStay()
    {
        Debug.Log("Colliding " + id);

        
    }

    private void CheckPointsInside()
    {
        Vec3[,,] positions = grid.GetPointPositions();

        points.Clear();

        for (int i = 0; i < positions.GetLength(0); i++)
        {
            for (int j = 0; j < positions.GetLength(1); j++)
            {
                for (int k = 0; k < positions.GetLength(2); k++)
                {
                    if (PointInMesh(positions[i, j, k]))
                    {
                        points.Add(positions[i, j, k]);
                    }
                }
            }
        }
    }

    private bool PointInMesh(Vec3 point)
    {
        Ray ray = new Ray(point, Vec3.Forward);
        float hit;
        int count = 0;

        foreach (var face in faces)
        {
            if (face.plane.Raycast(ray, out hit))
            {
                Vec3 collisionPoint = ray.GetPoint(hit);

                if (CheckFacesPoint(face.vertices[0], face.vertices[1], face.vertices[2], collisionPoint))
                {
                    count++;
                }
            }

        }

        return count % 2 != 0;

    }

    private bool CheckFacesPoint(Vec3 corner, Vec3 corner2, Vec3 corner3, Vec3 point)
    {
        float faceArea = TriangleArea(corner, corner2, corner3);
        float area1 = TriangleArea(corner, corner2, point);
        float area2 = TriangleArea(corner, corner3, point);
        float area3 = TriangleArea(corner2, corner3, point);

        return Mathf.Approximately(faceArea, (area1 + area2 + area3));

    }

    private static float TriangleArea(Vec3 v1, Vec3 v2, Vec3 v3)
    {
        float a = Vec3.Distance(v1, v2);
        float b = Vec3.Distance(v2, v3);
        float c = Vec3.Distance(v3, v1);

        float semiperimeter = (a + b + c) / 2f;

        float areaSquared = semiperimeter * (semiperimeter - a) * (semiperimeter - b) * (semiperimeter - c);

        // Check if the triangle is degenerate 
        if (areaSquared <= 0f)
        {
            return 0f;
        }

        float area = Mathf.Sqrt(areaSquared);

        return area;
    }

    private void DrawPoint(List<Vec3> points, float size)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Debug.DrawLine(points[i], points[i] + Vec3.Up, UnityEngine.Color.red);
        }
    }

}