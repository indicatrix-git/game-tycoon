using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Splines;


public class IndicatrixAPI : MonoBehaviour
{
    public static IndicatrixAPI instance;

    string[,] txtTiles;

    GameObject snapSphere;
    GameObject snapFace;
    GameObject lineFace01, lineFace02, lineFace03, lineFace04;

    bool vertexMode = false;
    bool faceMode = false;

    SplineContainer sc;

    List<BezierKnot> splines;

    Vector3 snapPoint;

    LineRenderer lineRenderer01, lineRenderer02, lineRenderer03, lineRenderer04;

    int knotCounter = 0;

    Material lineFaceMat, snapFaceMatTex;
    Texture2D snapFaceTex;


    Mesh meshFace;




    private void Start()
    {
        instance = this;

        txtTiles = new string[256, 256];
        DefaultTxtTiles();

        snapSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        snapSphere.name = "snapSphere";
        SphereCollider sphereCollider = snapSphere.GetComponent<SphereCollider>();
        Destroy(sphereCollider);
        snapSphere.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        snapSphere.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        snapSphere.GetComponent<Renderer>().receiveShadows = false;
        snapSphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        lineFaceMat = Resources.Load("Materials/LineMat", typeof(Material)) as Material;
        snapFaceMatTex = Resources.Load("Materials/FaceMatTex", typeof(Material)) as Material;

        snapFaceTex = Resources.Load("Textures/tex01", typeof(Texture2D)) as Texture2D;

        lineFace01 = new GameObject("LineFace01");
        lineFace02 = new GameObject("LineFace02");
        lineFace03 = new GameObject("LineFace03");
        lineFace04 = new GameObject("LineFace04");
        lineFace01.name = "LineFace01";
        lineFace02.name = "LineFace02";
        lineFace03.name = "LineFace03";
        lineFace04.name = "LineFace04";

        lineRenderer01 = lineFace01.AddComponent<LineRenderer>();
        lineRenderer02 = lineFace02.AddComponent<LineRenderer>();
        lineRenderer03 = lineFace03.AddComponent<LineRenderer>();
        lineRenderer04 = lineFace04.AddComponent<LineRenderer>();
        lineRenderer01.material = lineFaceMat;
        lineRenderer02.material = lineFaceMat;
        lineRenderer03.material = lineFaceMat;
        lineRenderer04.material = lineFaceMat;

        lineRenderer01.startWidth = 0.1f;
        lineRenderer01.endWidth = 0.1f;
        lineRenderer01.positionCount = 2;

        lineRenderer02.startWidth = 0.1f;
        lineRenderer02.endWidth = 0.1f;
        lineRenderer02.positionCount = 2;

        lineRenderer03.startWidth = 0.1f;
        lineRenderer03.endWidth = 0.1f;
        lineRenderer03.positionCount = 2;

        lineRenderer04.startWidth = 0.1f;
        lineRenderer04.endWidth = 0.1f;
        lineRenderer04.positionCount = 2;


        snapFace = new GameObject("SnapFace");
        snapFace.name = "SnapFace";


        snapFace.AddComponent<MeshFilter>();
        snapFace.AddComponent<MeshRenderer>();

        MeshFilter meshFilter = snapFace.GetComponent<MeshFilter>();
        meshFace = new Mesh();
        meshFilter.mesh = meshFace;


        MeshRenderer meshRenderer = snapFace.GetComponent<MeshRenderer>();

        meshRenderer.material = snapFaceMatTex;
        snapFaceMatTex.mainTexture = snapFaceTex;
    }



    //// TRAIN CONSTRUCTOR
    //if (Input.GetKeyDown(KeyCode.T))
    //{
    //    GameObject go = new GameObject("Train");
    //    go.name = "Train";

    //    sc = go.AddComponent<SplineContainer>(); //implicitne vytvori Spline 0

    //    splines = new List<BezierKnot>();
    //}


    //if (Input.GetMouseButtonDown(0))
    //{
    //    splines.Add(new BezierKnot(snapPoint));
    //}


    //if (Input.GetKeyDown(KeyCode.R))
    //{
    //    sc.Spline.Knots = splines.ToArray();
    // 





    public void txtSetTile(int x, int y, string valueTile)
    {
        txtTiles[x, y] = valueTile;
    }



    public string txtGetTile(int x, int y)
    {
        return txtTiles[x, y];
    }






    void DefaultTxtTiles()
    {
        for (int i = 0; i <= 256 - 1; i++)
        {
            for (int j = 0; j <= 256 - 1; j++)
            {
                txtTiles[i, j] = "0";
            }
        }

        using (StreamWriter sw = new StreamWriter("C:\\game\\SaveData.txt"))
        {
            for (int i = 0; i <= 256 - 1; i++)
            {
                for (int j = 0; j <= 256 - 1; j++)
                {
                    sw.WriteLine(i + " " + j + " " + txtTiles[i, j]);
                }
            }
        }
    }




    public string[,] LoadTxtTilesFromFile()
    {
        string[] splitParse;
        List<string> parseList01 = new List<string>();
        List<string> parseList02 = new List<string>();

        using (StreamReader sr = new StreamReader("C:\\game\\SaveData.txt"))
        {
            string line;

            while((line = sr.ReadLine()) != null)
            {
                int i, j;
                string value;

                splitParse = line.Split(' ');

                parseList01.Clear();
                for (int x = 0; x < 2; x++)
                {
                    parseList01.Add(splitParse[x]);
                }

                i = int.Parse(parseList01[0]);
                j = int.Parse(parseList01[1]);

                parseList02.Clear();
                for (int x = 2; x < splitParse.Length; x++)
                {
                    parseList02.Add(splitParse[x]);
                }

                value = string.Join(" ", parseList02);

                txtTiles[i, j] = value;
            }
        }
        return txtTiles;
    }



    public void SaveTxtTilesToFile()
    {
        using (StreamWriter sw = new StreamWriter("C:\\game\\SaveData.txt"))
        {
            for (int i = 0; i <= 256 - 1; i++)
            {
                for (int j = 0; j <= 256 - 1; j++)
                {
                    sw.WriteLine(i + " " + j + " " + txtTiles[i, j]); //priklad: 5 3 t1 s1
                }
            }
        }
    }






    public Vector3 SnapVertex(Vector3 hitPoint)
    {
        float snapX = 0, snapY = 0, snapZ = 0;

        int xx = (int)hitPoint.x; float x = hitPoint.x - xx;
        int zz = (int)hitPoint.z; float z = hitPoint.z - zz;

        if (x >= 0.0f && x <= 0.5f) { snapX = xx; }
        else snapX = xx + 1;

        if (z >= 0.0f && z <= 0.5f) { snapZ = zz; }
        else snapZ = zz + 1;

        snapY = FunctionalValueY(snapX, snapZ);
        snapSphere.transform.position = new Vector3(snapX, snapY, snapZ);

        return new Vector3(snapX, snapY, snapZ);
    }



    public Vector3 SnapLineFace(Vector3 hitPoint)
    {
        float[] Xaxis = new float[4];
        float[] Yaxis = new float[4];
        float[] Zaxis = new float[4];

        int xx = (int)hitPoint.x; float x = hitPoint.x - xx;
        int zz = (int)hitPoint.z; float z = hitPoint.z - zz;


        if (x >= 0.0f && x <= 1.0f)
        {
            Xaxis[0] = xx;
            Xaxis[1] = xx;
            Xaxis[2] = xx + 1;
            Xaxis[3] = xx + 1;
        }


        if (z >= 0.0f && z <= 1.0f)
        {
            Zaxis[0] = zz;
            Zaxis[1] = zz + 1;
            Zaxis[2] = zz + 1;
            Zaxis[3] = zz;
        }


        Yaxis[0] = FunctionalValueY(Xaxis[0], Zaxis[0]);
        Yaxis[1] = FunctionalValueY(Xaxis[1], Zaxis[1]);
        Yaxis[2] = FunctionalValueY(Xaxis[2], Zaxis[2]);
        Yaxis[3] = FunctionalValueY(Xaxis[3], Zaxis[3]);


        lineRenderer01.SetPosition(0, new Vector3(Xaxis[0], Yaxis[0], Zaxis[0]));
        lineRenderer01.SetPosition(1, new Vector3(Xaxis[1], Yaxis[1], Zaxis[1]));

        lineRenderer02.SetPosition(0, new Vector3(Xaxis[1], Yaxis[1], Zaxis[1]));
        lineRenderer02.SetPosition(1, new Vector3(Xaxis[2], Yaxis[2], Zaxis[2]));

        lineRenderer03.SetPosition(0, new Vector3(Xaxis[2], Yaxis[2], Zaxis[2]));
        lineRenderer03.SetPosition(1, new Vector3(Xaxis[3], Yaxis[3], Zaxis[3]));

        lineRenderer04.SetPosition(0, new Vector3(Xaxis[0], Yaxis[0], Zaxis[0]));
        lineRenderer04.SetPosition(1, new Vector3(Xaxis[3], Yaxis[3], Zaxis[3]));


        return GetCenterFace(hitPoint);
    }



    public Vector3 SnapMeshFace(Vector3 hitPoint)
    {
        Vector3[] verticesFace;
        Vector3[] normalsFace;
        Vector2[] uvFace;
        int[] trianglesFace;

        float[] Xaxis = new float[4];
        float[] Yaxis = new float[4];
        float[] Zaxis = new float[4];

        int xx = (int)hitPoint.x; float x = hitPoint.x - xx;
        int zz = (int)hitPoint.z; float z = hitPoint.z - zz;

        float dy = 0.01f;

        if (x >= 0.0f && x <= 1.0f)
        {
            Xaxis[0] = xx;
            Xaxis[1] = xx;
            Xaxis[2] = xx + 1;
            Xaxis[3] = xx + 1;
        }


        if (z >= 0.0f && z <= 1.0f)
        {
            Zaxis[0] = zz;
            Zaxis[1] = zz + 1;
            Zaxis[2] = zz + 1;
            Zaxis[3] = zz;
        }


        Yaxis[0] = FunctionalValueY(Xaxis[0], Zaxis[0]);
        Yaxis[1] = FunctionalValueY(Xaxis[1], Zaxis[1]);
        Yaxis[2] = FunctionalValueY(Xaxis[2], Zaxis[2]);
        Yaxis[3] = FunctionalValueY(Xaxis[3], Zaxis[3]);


        verticesFace = new Vector3[]
        {
            new Vector3(Xaxis[0], Yaxis[0] + dy, Zaxis[0]),
            new Vector3(Xaxis[1], Yaxis[1] + dy, Zaxis[1]),
            new Vector3(Xaxis[2], Yaxis[2] + dy, Zaxis[2]),
            new Vector3(Xaxis[3], Yaxis[3] + dy, Zaxis[3])
        };


        trianglesFace = new int[]
        {
            0, 1, 2,
            2, 3, 0
        };


        normalsFace = new Vector3[]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };


        uvFace = new Vector2[]
        {
            new Vector2(Xaxis[0], Zaxis[0]),
            new Vector2(Xaxis[1], Zaxis[1]),
            new Vector2(Xaxis[2], Zaxis[2]),
            new Vector2(Xaxis[3], Zaxis[3])
        };


        meshFace.Clear();
        meshFace.vertices = verticesFace;
        meshFace.triangles = trianglesFace;
        meshFace.normals = normalsFace;
        meshFace.uv = uvFace;
        meshFace.RecalculateNormals();

        return GetCenterFace(hitPoint);
    }





    //pretazit metodu GetCenterFace na Tile(x,z)
    
    
    
    public Vector3 GetCenterFace(Vector3 hitPoint)
    {
        float[] Xaxis = new float[4];
        float[] Yaxis = new float[4];
        float[] Zaxis = new float[4];

        int xx = (int)hitPoint.x; float x = hitPoint.x - xx;
        int zz = (int)hitPoint.z; float z = hitPoint.z - zz;


        if (x >= 0.0f && x <= 1.0f)
        {
            Xaxis[0] = xx;
            Xaxis[1] = xx;
            Xaxis[2] = xx + 1;
            Xaxis[3] = xx + 1;
        }


        if (z >= 0.0f && z <= 1.0f)
        {
            Zaxis[0] = zz;
            Zaxis[1] = zz + 1;
            Zaxis[2] = zz + 1;
            Zaxis[3] = zz;
        }

        float dx = (Xaxis[0] < Xaxis[3]) ? (Xaxis[0] + Xaxis[3]) / 2 : (Xaxis[1] + Xaxis[2]) / 2;
        float dz = (Zaxis[0] < Zaxis[1]) ? (Zaxis[0] + Zaxis[1]) / 2 : (Zaxis[3] + Zaxis[2]) / 2;

        float dy = 0;

        Yaxis[0] = FunctionalValueY(Xaxis[0], Zaxis[0]);
        Yaxis[1] = FunctionalValueY(Xaxis[1], Zaxis[1]);
        Yaxis[2] = FunctionalValueY(Xaxis[2], Zaxis[2]);
        Yaxis[3] = FunctionalValueY(Xaxis[3], Zaxis[3]);


        for (int i = 0; i < 4; i++)
        {
            if (Yaxis[0] == Yaxis[i])
            {
                dy = Yaxis[0];
            }
            else
            {
                dy = (Yaxis[i] + Yaxis[i - 1]) / 2;
            }
        }

        return new Vector3(dx, dy, dz);
    }



    float FunctionalValueY(float snapX, float snapZ)
    {
        float snapY = 0;

        for (int Ci = 0, Cz = 0; Cz <= TerrainManager.instance.terrainWidth; Cz++)
        {
            for (int Cx = 0; Cx <= TerrainManager.instance.terrainWidth; Cx++, Ci++)
            {
                if (TerrainManager.instance.coordsF[Ci].x == snapX &&
                    TerrainManager.instance.coordsF[Ci].z == snapZ)
                {
                    snapY = TerrainManager.instance.coordsF[Ci].y;
                }
            }
        }
        return snapY;
    }












    /* POSTUP:
     * 1. Vykoname Coord Up/Down
     * 2. Vygeneruje izo mapu podla Coords
     * 3. Spusti sa Algoritmus pre izo upravu vyskovych cisiel
     * 4. Pregeneruje sa nova izo mapa na nove Coords, teda realne Y suradnice Coords
     * 5. Vymaze sa stary teren z Coords
     * 6. Vygeneruje sa nova mapa z noveho Coords
     * 
     * */





}
