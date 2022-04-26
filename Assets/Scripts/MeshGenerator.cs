using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    int xSize = 10;
    int zSize = 10;
    float waveLength = 8;
    float amplitude = 0.5f;

    Vector4 Wave1 = new Vector4(1, 0, 0.5f, 10);

    Mesh waterMesh;
    Vector3[] Verticies;
    Vector3[] InitialVerticies;
    Vector2[] UVs;
    int[] Triangles;

    void Start()
    {
        InitialVerticies = createVerticies();
        Verticies = (Vector3[])InitialVerticies.Clone();
        Triangles = createTriangles();

        waterMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = waterMesh;
        waterMesh.vertices = Verticies;
        waterMesh.uv = UVs;
        waterMesh.triangles = Triangles;
        waterMesh.RecalculateNormals();
    }

    private void Update()
    {
        for(int i = 0; i < Verticies.Length; i++)
        {
            Verticies[i] = GerstnerWave(Wave1, InitialVerticies[i]);
        }

        waterMesh.vertices = Verticies;
        waterMesh.RecalculateNormals();
    }

    private Vector3 GerstnerWave(Vector4 wave, Vector3 vertex)
    {
        Vector3 newPosition = Vector3.zero;

        Vector2 waveDirection = new Vector2(wave.x, wave.z);

        float k = 2 * Mathf.PI / waveLength;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float c = Mathf.Sqrt(gravity / k);
        Vector3 d = waveDirection.normalized;
        float dot = Vector2.Dot(d, new Vector2(vertex.x, vertex.z));
        float f = k * (dot - c * Time.time);

        newPosition.x = vertex.x + (amplitude * Mathf.Cos(f));
        newPosition.y = amplitude * Mathf.Sin(f);
        newPosition.z = vertex.z + (amplitude * Mathf.Cos(f));

        return newPosition;
    }

    private Vector3[] createVerticies()
    {
        Vector3[] verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                verticies[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        return verticies;
    }

    private int[] createTriangles()
    {
        int[] triangels = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangels[tris + 0] = vert;
                triangels[tris + 1] = vert + xSize + 1;
                triangels[tris + 2] = vert + 1;
                triangels[tris + 3] = vert + 1;
                triangels[tris + 4] = vert + xSize + 1;
                triangels[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }

        return triangels;
    }
}

// Sources:
// https://catlikecoding.com/unity/tutorials/flow/waves/
