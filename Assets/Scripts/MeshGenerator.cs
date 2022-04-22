using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    int xSize = 10;
    int zSize = 10;
    float waveSpeed = 1;
    float waveLength = 10;
    float amplitude = 1f;

    Mesh waterMesh;
    Vector3[] Verticies;
    Vector2[] UVs;
    int[] Triangles;

    // fix
    float[] xs;

    void Start()
    {
        Verticies = createVerticies();
        Triangles = createTriangles();

        // fix
        xs = new float[Verticies.Length];
        for(int i = 0; i < xs.Length; i++)
        {
            xs[i] = Verticies[i].x;
        }

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
            Vector3 vert = Verticies[i];

            float k = 2 * Mathf.PI / waveLength;
            float f = k * (vert.x - waveSpeed * Time.time);
            vert.x = xs[i] + amplitude * Mathf.Cos(f);
            vert.y = amplitude * Mathf.Sin(f);

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
