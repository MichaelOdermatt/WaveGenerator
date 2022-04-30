using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    int xSize = 100;
    int zSize = 100;
    float HeightMultiplier = 1f;

    Vector4 Wave1 = new Vector4(1, 1, 0.1f, 150);
    Vector4 Wave2 = new Vector4(0, 1, 0.15f,50);
    Vector4 Wave3 = new Vector4(1, 1.3f, 0.03f, 100);

    Mesh waterMesh;
    Vector3[] Verticies;
    Vector3[] InitialVerticies;
    Vector2[] UVs;
    int[] Triangles;
    float[,] NoiseMap;

    void Start()
    {
        NoiseMap = NoiseGenerator.GenerateNoiseMap(xSize + 1, zSize + 1, 234, 5, 3, 0.5f, 2.5f, new Vector2(0, 0));

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
        Vector2 NoiseMapOffset = Wave1 + Wave2 + Wave3;
        NoiseMapOffset.Normalize();
        NoiseMapOffset *= Time.time;
        NoiseMap = NoiseGenerator.GenerateNoiseMap(xSize + 1, zSize + 1, 234, 5, 3, 0.5f, 2.5f, NoiseMapOffset * -1);

        for(int i = 0; i < Verticies.Length; i++)
        {
            Vector3 newPoint = InitialVerticies[i];

            newPoint += GerstnerWave(Wave1, InitialVerticies[i]);
            newPoint += GerstnerWave(Wave2, InitialVerticies[i]);
            newPoint += GerstnerWave(Wave3, InitialVerticies[i]);

            newPoint.y += NoiseMap[(int)InitialVerticies[i].x, (int)InitialVerticies[i].z] * HeightMultiplier;

            Verticies[i] = newPoint;
        }

        waterMesh.vertices = Verticies;
        waterMesh.RecalculateNormals();
    }

    private Vector3 GerstnerWave(Vector4 wave, Vector3 vertex)
    {
        Vector2 waveDirection = new Vector2(wave.x, wave.y);
        waveDirection.Normalize();

        float steepness = wave.z;
        float waveLength = wave.w;
        float k = 2 * Mathf.PI / waveLength;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float c = Mathf.Sqrt(gravity / k);
        float dot = Vector2.Dot(waveDirection, new Vector2(vertex.x, vertex.z));
        float f = k * (dot - c * Time.time);
        float a = steepness / k;

        Vector3 newPosition = Vector3.zero;

        newPosition.x = waveDirection.x * (a * Mathf.Cos(f));
        newPosition.y = a * Mathf.Sin(f);
        newPosition.z = waveDirection.y * (a  * Mathf.Cos(f));

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
// Gernstner Waves https://catlikecoding.com/unity/tutorials/flow/waves/
