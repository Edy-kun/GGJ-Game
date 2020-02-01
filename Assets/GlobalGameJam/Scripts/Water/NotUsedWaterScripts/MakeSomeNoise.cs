using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSomeNoise : MonoBehaviour
{
    public float power = 3;
    public float scale = 1;
    public float timeScale = 1;

    private float xOffset;
    private float yOffset;
    private MeshFilter mf;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        MakeNoise();
    }

    private void Update()
    {
        MakeNoise();
        xOffset += Time.deltaTime * timeScale;
        yOffset += Time.deltaTime * timeScale;
    }

    private void MakeNoise()
    {
        Vector3[] vertices = mf.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * power;
        }

        mf.mesh.vertices = vertices;
    }

    private float CalculateHeight (float x, float y){
        float xCord = x * scale * xOffset;
        float yCord = y * scale * yOffset;

        return Mathf.PerlinNoise(xCord, yCord);

    }
}
