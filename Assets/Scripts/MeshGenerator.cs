using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public float Scale = 5;
    public int Octaves = 4;
    public float Persistance = 0.5f;
    public float Lacunarity = 3.5f;

    int xSize = 100;
    int zSize = 100;
    public float HeightMultiplier = 1f;

    public Vector4[] Waves = new Vector4[]
    {
        // x and y represent wave direction in the x and z direction respectively,
        // z represents steepness, and w represents wavelength
        new Vector4(1, 1, 0.2f, 50),
        new Vector4(0, 1, 0.15f,25),
        new Vector4(1, 1.3f, 0.15f, 15),
    };

    Mesh waterMesh;
    Vector3[] InitialVerticies;
    Vector2[] UVs;
    int[] Triangles;
    float[,] NoiseMap;

    void Start()
    {
        InitialVerticies = createVerticies();
        Triangles = createTriangles();

        waterMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = waterMesh;
        waterMesh.vertices = InitialVerticies;
        waterMesh.uv = UVs;
        waterMesh.triangles = Triangles;
        waterMesh.RecalculateNormals();
    }

    private void Update()
    {
        Vector2 NoiseMapOffset = getWaveDirection();
        // multiply by -1 to reverse the direction of the offset to match waves
        NoiseMapOffset *= Time.time * -1;
        NoiseMap = NoiseGenerator.GenerateNoiseMap(
            xSize + 1, zSize + 1, 234, Scale, 
            Octaves, Persistance, Lacunarity, NoiseMapOffset);

        var newVerticies = (Vector3[])InitialVerticies.Clone();

        for(int i = 0; i < InitialVerticies.Length; i++)
        {
            newVerticies[i] = applyGerstnerWaveAndNoise(InitialVerticies[i], Waves);
        }

        waterMesh.vertices = newVerticies;
        waterMesh.RecalculateNormals();
    }

    private Vector3 applyGerstnerWaveAndNoise(Vector3 Vertex, Vector4[] waves)
    {
        Vector3 newPoint = Vertex;

        foreach (var wave in waves)
        {
            newPoint += GerstnerWave(wave, Vertex);
        }

        newPoint.y += NoiseMap[(int)Vertex.x, (int)Vertex.z] * HeightMultiplier;

        return newPoint;
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

    private Vector2 getWaveDirection()
    {
        Vector2 sum = Vector2.zero;

        foreach (Vector2 wave in Waves)
        {
            sum += wave;
        }

        sum.Normalize();
        return sum;
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
