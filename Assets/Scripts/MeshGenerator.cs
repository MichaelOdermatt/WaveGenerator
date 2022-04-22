using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    int xSize = 10;
    int zSize = 10;
    float waveSpeed = 1;
    float waveLength = 10;
    float amplitude = 0.75f;

    Mesh waterMesh;
    Vector3[] Verticies;
    Vector2[] UVs;
    int[] Triangles;

    void Start()
    {
        Verticies = createVerticies();
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
            float k = 2 * Mathf.PI / waveLength;
            float y = amplitude * Mathf.Sin(
                k * (Verticies[i].x - waveSpeed * Time.time));

            Verticies[i] = new Vector3(
                Verticies[i].x,
                y,
                Verticies[i].z);
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
