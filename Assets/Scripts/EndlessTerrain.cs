using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public float scale = 50f;
    public float amplitude = 5f;

    public float meshScale = 50f;

    public MeshFilter chunkPrefab;
    public MeshFilter[] chunks;
    MeshCollider[] chunkColliders;
    int chunkCount = 6;

    public float trainSpeed = 2f;

    float currentChunkZ = 0f;

    float offsetX;
    float offsetZ;

    float bumpTime = 0f;
    float bumpScale = 0.1f;

    void Start() {
        offsetX = Random.Range(-10000000f, 10000000f);
        offsetZ = Random.Range(-10000000f, 10000000f);

        chunks = new MeshFilter[6];
        chunkColliders = new MeshCollider[6];
        for (int i = 0; i < chunkCount; i++) {
            chunks[i] = Instantiate(chunkPrefab, new Vector3(-15f * meshScale, -3f, -300f + i * 9f * meshScale), Quaternion.identity);
            chunks[i].mesh = GenerateMesh(30, 10, offsetX, i * 9f + offsetZ);
            chunkColliders[i] = chunks[i].GetComponent<MeshCollider>();
            chunkColliders[i].sharedMesh = chunks[i].mesh;
            
            ShuffleTrees(chunks[i].transform);
        }
        currentChunkZ = 9f * chunkCount;

        //meshFilter.mesh = GenerateMesh(15, 10, 0, 0);
    }

    void ShuffleTrees(Transform chunk) {
        for (int j = 0; j < chunk.childCount; j++) {
            Transform tree = chunk.GetChild(j);
            
            if (tree.gameObject.layer != 8) {
                continue;
            }

            RaycastHit hit;
            Vector3 spawn;
            do {
                do {
                    spawn = new Vector3(Random.Range(-150f, 150f), 50f, chunk.position.z + Random.Range(0f, 9f * meshScale));
                } while (Mathf.Abs(spawn.x) < 15f);
            } while (!Physics.Raycast(spawn, Vector3.down, out hit, 100f, 1<<7, QueryTriggerInteraction.Ignore));
            
            tree.position = hit.point;

            if (spawn.x < 0) {
                tree.rotation = Quaternion.LookRotation(Vector3.right);
            }
            else {
                tree.rotation = Quaternion.LookRotation(Vector3.left);
            }

            tree.transform.localScale = Vector3.one * Random.Range(1.5f, 3f);
        }
    }

    void Update() {
        chunks[0].transform.Translate(-Vector3.forward * trainSpeed * Time.deltaTime);

        /*
        bumpTime += Time.deltaTime;
        float bumpHeight = Mathf.PerlinNoise(bumpTime / bumpScale, 0f);

        
        chunks[0].transform.position = new Vector3(chunks[0].transform.position.x, -3f + bumpHeight, chunks[0].transform.position.z);
        */

        for (int i = 1; i < chunkCount; i++) {
            chunks[i].transform.position = chunks[i-1].transform.position + Vector3.forward * 9f * meshScale;
        }

        if (chunks[0].transform.position.z < -350f) {
            MeshFilter movingChunk = chunks[0];
            for (int i = 0; i < chunkCount - 1; i++) {
                chunks[i] = chunks[i+1];
            }
            chunks[chunkCount-1] = movingChunk;
            movingChunk.transform.position = chunks[chunkCount - 2].transform.position + Vector3.forward * 9f * meshScale;
            movingChunk.mesh = GenerateMesh(30, 10, offsetX, currentChunkZ + offsetZ);
            movingChunk.GetComponent<MeshCollider>().sharedMesh = movingChunk.mesh;
            currentChunkZ += 9f;
            ShuffleTrees(movingChunk.transform);
        }
    }

    Mesh GenerateMesh(int width, int length, float chunkX, float chunkZ) {
        Vector3[] vertices = new Vector3[width * length];

        for (int z = 0; z < length; z++) {
            for (int x = 0; x < width; x++) {
                float height = Mathf.PerlinNoise((x + chunkX) / scale, (z + chunkZ) / scale) * amplitude;

                height *= (Mathf.Abs(15f - x) / 7.5f);

                vertices[z * width + x] = new Vector3(x * meshScale, height, z * meshScale);
            }
        }

        int[] triangles = new int[(width - 1) * (length - 1) * 6];
        int index = 0;

        for (int z = 0; z < length - 1; z++) {
            for (int x = 0; x < width - 1; x++) {
                triangles[index] = z * width + x;
                triangles[index+1] = (z+1) * width + x;
                triangles[index+2] = (z+1) * width + x+1;
                triangles[index+3] = z * width + x;
                triangles[index+4] = (z+1) * width + x+1;
                triangles[index+5] = z * width + x+1;

                index += 6;
            }
        }

        Vector2[] uvs = new Vector2[width * length];
        for (int i = 0; i < width * length; i++) {
            uvs[i] = new Vector2(0, 0);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
