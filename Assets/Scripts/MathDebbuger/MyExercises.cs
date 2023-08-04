using UnityEngine;
using CustomMath;
using MathDebbuger;

public class MyExercises : MonoBehaviour
{
    public enum Exercise
    {
        Uno,
        Dos,
        Tres,
        Cuatro,
        Cinco,
        Seis,
        Siete,
        Ocho,
        Nueve,
        Diez
    }

    public Exercise exercise = Exercise.Uno;
    public Color VectorColor = Color.red;
    public Vector3 a;
    public Vector3 b;

    private Vec3 vecA;
    private Vec3 vecB;
    private Vec3 vecC;
    private float time = 0;
    private const int tLimit = 5;

    void Start()
    {
        Vector3Debugger.AddVector(vecA, Color.cyan, "A");
        Vector3Debugger.EnableEditorView("A");
        Vector3Debugger.AddVector(vecB, Color.gray, "B");
        Vector3Debugger.EnableEditorView("B");
        Vector3Debugger.AddVector(vecC, VectorColor, "C");
        Vector3Debugger.EnableEditorView("C");
    }

    void Update()
    {
        vecA = new Vec3(a);
        vecB = new Vec3(b);

        switch (exercise)
        {
            case Exercise.Uno:

                vecC = vecA + vecB;

                break;
            case Exercise.Dos:

                vecC = vecB - vecA;

                break;
            case Exercise.Tres:

                vecC = new Vec3(vecA.x * vecB.x, vecA.y * vecB.y, vecA.z * vecB.z);

                break;
            case Exercise.Cuatro:

                vecC = Vec3.Cross(vecB, vecA);

                break;
            case Exercise.Cinco:

                time = time > 1 ? 0 : time + Time.deltaTime;

                vecC = Vec3.Lerp(vecA, vecB, time);

                break;
            case Exercise.Seis:

                vecC = Vec3.Max(vecA, vecB);

                break;
            case Exercise.Siete:

                vecC = Vec3.Project(vecA, vecB);

                break;
            case Exercise.Ocho:

                vecC = (vecA + vecB).normalized * Vec3.Distance(vecA, vecB);
                 
                break;
            case Exercise.Nueve:

                vecC = Vec3.Reflect(vecA, (vecB).normalized);

                break;
            case Exercise.Diez:

                time = time > tLimit ? 0 : time + Time.deltaTime;

                vecC = Vec3.LerpUnclamped(vecB, vecA, time);

                break;
            default:
                break;
        }

        Vector3Debugger.UpdatePosition("A", TransformVec3ToVector3(vecA));
        Vector3Debugger.UpdatePosition("B", TransformVec3ToVector3(vecB));
        Vector3Debugger.UpdatePosition("C", TransformVec3ToVector3(vecC));
    }

    Vector3 TransformVec3ToVector3(Vec3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }
}
