using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    int xSize = 10;
    int zSize = 10;
    float waveSpeed = 2;
    float waveLength = 10;
    float amplitude = 1f;

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
        Vector3 direction = new Vector3(1, 0, 1);
        Vector2 direction2d = new Vector2(direction.x, direction.z);

        for(int i = 0; i < Verticies.Length; i++)
        {
            Vector3 vert = Verticies[i];
            Vector3 initialVert = InitialVerticies[i];

            float k = 2 * Mathf.PI / waveLength;
            float c = Mathf.Sqrt(9.8f / k);
            Vector3 d = direction2d.normalized;
            float dot = Vector2.Dot(d, new Vector2(initialVert.x, initialVert.z));
            float f = k * (dot - c * Time.time);

            vert.x = initialVert.x + (amplitude * Mathf.Cos(f));
            vert.y = amplitude * Mathf.Sin(f);
            vert.z = initialVert.z + (amplitude * Mathf.Cos(f));

            Verticies[i] = vert;
        }

        waterMesh.vertices = Verticies;
        waterMesh.RecalculateNormals();
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
