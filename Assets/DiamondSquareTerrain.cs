

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiamondSquareTerrain : MonoBehaviour {
    public int detailLevel = 6;
    int detail;
    public float mSize = 5;
    public float mHeight = 2;
    public float heightDecay = 0.5f;

    Vector3[] vertices;
    float[] heightMap;
    int mVertCount;

	public Shader shader;
	public PointLight pointLight;

    public Color mountainColor = new Vector4(0.1f, 0.8f, 0.1f, 1.0f);
    public Color sandColor = new Vector4(0.3f, 0.3f, 0.1f, 1.0f);
    public Color snowColor = new Vector4(1f, 1f, 1f, 1.0f);

    void Start () {
        MeshFilter terrain = this.gameObject.AddComponent<MeshFilter>();
        terrain.mesh = this.CreateTerrain();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        shader = Shader.Find("xin/TerrainShader");
		renderer.material.shader = shader;
        renderer.material.SetColor("_MountainColor", mountainColor);
        renderer.material.SetColor("_SandColor", sandColor);
        renderer.material.SetColor("_SnowColor", snowColor);

        //add mesh collider
        MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
        //collider.gameObject = this.gameObject;


        Mesh mesh = GetComponent<MeshFilter>().mesh;
        

    }
	void Update()
	{
		// Get renderer component (in order to pass params to shader)
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader

        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
		renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
	}

    Mesh CreateTerrain() {
        detail = (int)Mathf.Pow(2, detailLevel);
        mVertCount = (detail + 1) * (detail + 1);
        vertices = new Vector3[mVertCount];
        Vector2[] uvs = new Vector2[mVertCount];
        List<int> triangles = new List<int>();
        
        //float halfSize = mSize * 0.5f;

        //size of each small grid, equals mountain size / number of segments
        float gridSize =mSize/ detail; ;


        Mesh mesh = new Mesh();
        mesh.name = "Mesh";

        //5 segments will have 6 vertices 
        int offset = detail + 1;
        for (int i = 0;i<=detail;i++) {
            for (int j = 0; j <= detail; j++) {
                vertices[i * offset + j] = new Vector3(i * gridSize, 0.0f, j*gridSize);
                //Debug.Log(-halfSize + j * gridSize);
                uvs[i * (detail + 1) + j] = new Vector2((float)i/detail, (float)j/detail);

                //stop at end of the border
                if (i == detail || j == detail) {
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
                int v3 = (i + 1) * offset + j+1;
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


        heightMap = new float[vertices.Length];
        HeightMap();
        //maping heightmap onto vertices
        int index = 0;
        while (index < vertices.Length) {
            vertices[index].y = heightMap[index];
            index++;
        }

        mesh.vertices = vertices;
        //mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;

         
    }
    void HeightMap() {
        

        //number of grids start from 0
        int verticesCount = vertices.Length - 1;
        heightMap[0] = restrictHeight(RandomHeight(mHeight));
        heightMap[detail] = restrictHeight(RandomHeight(mHeight));
        heightMap[verticesCount] = restrictHeight(RandomHeight(mHeight));
        heightMap[verticesCount - detail] = restrictHeight(RandomHeight(mHeight));
        int steps = (int)Mathf.Log(detail, 2);
        int numSquares = 1;
        int squareSize = detail;
        for (int i = 0; i < steps; i++)
        {
            int row = 0;
            for (int j = 0; j < numSquares; j++)
            {
                int col = 0;
                for (int k = 0; k < numSquares; k++)
                {

                    DiamondSquare(row, col, squareSize, mHeight);
                    col += squareSize;

                }

                row += squareSize;

            }
            numSquares *= 2;
            squareSize /= 2;
            mHeight *= heightDecay;//decay

        }
        
    }
    

    void DiamondSquare(int row, int col, int gridSize, float maxHeight)
    {

        int offset = detail + 1;
        /*
         c1 #### v1 #### c2
         #                #
         #                #
         v4      m       v2
         #                #
         #                #
         c4 #### v3 #### c3

         */
        int halfGridSize = (int)(gridSize * 0.5f);
        int topLeft = row * offset + col;
        int botLeft = (row + gridSize) * offset + col;

        //index for four corners
        int c1 = row * offset + col;
        int c2 = row * offset + col + gridSize;
        int c3 = (row + gridSize) * offset + col + gridSize;
        int c4 = (row + gridSize) * offset + col;

        int middle = (row + halfGridSize) * offset + col + halfGridSize;
        int v1 = row * offset + col + halfGridSize;
        int v2 = middle + halfGridSize;
        int v3 = (row + gridSize) * offset + col + halfGridSize;
        int v4 = middle - halfGridSize;

        //middle = avg of four corners plus a random value
        heightMap[middle] =  restrictHeight (RandomAverage(new int[] {c1,c2,c3,c4}) + RandomHeight(maxHeight)   );
        heightMap[v1] =  restrictHeight(RandomAverage(new[]{ c1, c2, middle }) + RandomHeight(maxHeight));
        heightMap[v2] = restrictHeight( RandomAverage(new[]{ middle, c2, c3 }) + RandomHeight(maxHeight));
        heightMap[v3] = restrictHeight(RandomAverage(new[]{ middle, c3, c4 }) + RandomHeight(maxHeight));
        heightMap[v4] =  restrictHeight(RandomAverage(new[]{ c1, middle, c4 }) + RandomHeight(maxHeight) );


        
    }

    float RandomHeight(float maxHeight) {
        return Random.Range(-maxHeight, maxHeight);
    }
    float RandomAverage(int[] points) {
        float sum = 0.0f;
        foreach (int v in points) {
            sum += heightMap[v];
        } 
        return sum/points.Length;
    }
    //prevent underground 
    float restrictHeight(float h){
        return h < 0 ? 0 : h;
    }


}

