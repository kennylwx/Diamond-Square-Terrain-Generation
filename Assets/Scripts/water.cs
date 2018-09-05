using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class water : MonoBehaviour 
{
    public int detailLevel = 6;
    int detail;
    public float mSize = 500; 
    public float waveHeight = 1.0f;
    public float waterLevel = 40f;

    Vector3[] vertices;

    int mVertCount;

    void Start()
    {
        MeshFilter terrain = this.gameObject.AddComponent<MeshFilter>();
        terrain.mesh = this.CreateWater();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();

        renderer.material.shader = Shader.Find("xin/wave");
        renderer.material.SetColor("_Color", new Vector4(0.12678f, 0.5359051f, 0.7264151f, 0.8f));
        //add mesh collider
        MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();

    }
    Mesh CreateWater()
    {
        detail = (int)Mathf.Pow(2, detailLevel);
        mVertCount = (detail + 1) * (detail + 1);
        vertices = new Vector3[mVertCount];
        List<int> triangles = new List<int>();

        //size of each small grid, equals mountain size / number of segments
        float gridSize = mSize / detail; ;

        Mesh mesh = new Mesh();
        mesh.name = "Water";

        //5 segments will have 6 vertices 
        int offset = detail + 1;
        for (int i = 0; i <= detail; i++)
        {
            for (int j = 0; j <= detail; j++)
            {
                float rndHeight = waterLevel + Random.Range(-waveHeight, waveHeight);
                vertices[i * offset + j] = new Vector3(i * gridSize, rndHeight, j * gridSize);
                //stop at end of the border
                if (i == detail || j == detail)
                {
                    continue;
                }

                /*
                 v1 ######## v2
                 # #          #
                 #   #        #
                 #      #     #
                 #         #  #
                 v4 ######## v3
                */
                int v1 = i * offset + j;
                int v2 = i * offset + j + 1;
                int v3 = (i + 1) * offset + j + 1;
                int v4 = (i + 1) * offset + j;

                //top right triangle
                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);


                //bottom left triangle
                triangles.Add(v1);
                triangles.Add(v3);
                triangles.Add(v4);
            }
        }


        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        return mesh;

    }

}