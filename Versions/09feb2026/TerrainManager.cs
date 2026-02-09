using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;
    public int terrainWidth, elementWidth;
    public TerrainElement terrainPrefab;
    [HideInInspector]
    public Vector3[] coordsF;
    public Vector3[] coordsI;
    [HideInInspector]
    public TerrainElement[] terrainElements;

    Dictionary<float, float> heightMapGridY = new Dictionary<float, float>();

    void Start()
    {
        instance = this;

        CreateMapF();
        CreateMapI();
        CreateTerrainElements();

        heightMapGridY.Add(2.75f, -1);
        heightMapGridY.Add(3.0f, 0);
        heightMapGridY.Add(3.25f, 1);
        heightMapGridY.Add(3.5f, 2);
        heightMapGridY.Add(3.75f, 3);
        heightMapGridY.Add(4.0f, 4);
        heightMapGridY.Add(4.25f, 5);
        heightMapGridY.Add(4.5f, 6);
        heightMapGridY.Add(4.75f, 7);
        heightMapGridY.Add(5.0f, 8);
        heightMapGridY.Add(5.25f, 9);
        heightMapGridY.Add(5.5f, 10);
    }



    private float DeltaY(Vector3 first, Vector3 second)
    {
        float deltaY = Math.Abs(first.y - second.y);

        return deltaY;
    }


    public void TerrainCollapse(int iCoordClick)
    {
        bool repeatMapTraverse = true;

        Vector3 coordClick = coordsI[iCoordClick];


        while (repeatMapTraverse)
        {
            int x1 = (int)(coordClick.x) - 10;
            int x2 = x1 + 20;

            if (x1 <= 0) { x1 = 1; }
            if (x2 <= 0) { x2 = 1; }
            if (x1 >= 256) { x1 = 255; }
            if (x2 >= 256) { x2 = 255; }


            int z1 = (int)(coordClick.z) - 10;
            int z2 = z1 + 20;

            if (z1 <= 0) { z1 = 1; }
            if (z2 <= 0) { z2 = 1; }
            if (z1 >= 256) { z1 = 255; }
            if (z2 >= 256) { z2 = 255; }


            int iVertex = FunctionalVertexValueI(x1, z1);


            for (int i = iVertex, z = z1; z <= z2; z++)
            {
                for (int x = x1; x <= x2; x++, i++)
                {
                    int iXZvertex = FunctionalVertexValueI(x, z);

                    Vector3 mainVertex = coordsI[iXZvertex];

                    if (!((mainVertex.x == 0 || mainVertex.x == 256) && (mainVertex.z == 0 || mainVertex.z == 256)))
                    {
                        float upX = mainVertex.x + 1;
                        float downX = mainVertex.x - 1;
                        float leftZ = mainVertex.z + 1;
                        float rightZ = mainVertex.z - 1;


                        Vector3 upVertex = new Vector3(upX, FunctionalVertexValueY(upX, mainVertex.z), mainVertex.z);
                        Vector3 downVertex = new Vector3(downX, FunctionalVertexValueY(downX, mainVertex.z), mainVertex.z);
                        Vector3 leftVertex = new Vector3(mainVertex.x, FunctionalVertexValueY(mainVertex.x, leftZ), leftZ);
                        Vector3 rightVertex = new Vector3(mainVertex.x, FunctionalVertexValueY(mainVertex.x, rightZ), rightZ);

                        float n1 = DeltaY(mainVertex, upVertex);
                        float n2 = DeltaY(mainVertex, downVertex);
                        float n3 = DeltaY(mainVertex, leftVertex);
                        float n4 = DeltaY(mainVertex, rightVertex);

                        if (n1 == 2 || n2 == 2 || n3 == 2 || n4 == 2)
                        {
                            if (coordClick == mainVertex)
                            {
                                coordsI[iXZvertex].x = coordsI[iXZvertex].x;
                                coordsI[iXZvertex].y = coordsI[iXZvertex].y;
                                coordsI[iXZvertex].z = coordsI[iXZvertex].z;
                            }
                            else
                            {
                                coordsI[iXZvertex].x = coordsI[iXZvertex].x;
                                coordsI[iXZvertex].y = coordsI[iXZvertex].y + 1;
                                coordsI[iXZvertex].z = coordsI[iXZvertex].z;
                            }

                            repeatMapTraverse = true;
                        }
                        else repeatMapTraverse = false;
                    }
                }
            }
        }
    }




    float FunctionalVertexValueY(float coordX, float coordZ)
    {
        float coordY = 0;

        for (int Ci = 0, Cz = 0; Cz <= terrainWidth; Cz++)
        {
            for (int Cx = 0; Cx <= terrainWidth; Cx++, Ci++)
            {
                if (coordsI[Ci].x == coordX &&
                    coordsI[Ci].z == coordZ)
                {
                    coordY = coordsI[Ci].y;
                }
            }
        }
        return coordY;
    }




    int FunctionalVertexValueI(int xPos, int zPos)
    {
        int iVertex = 0;

        for (int i = 0, z = 0; z <= terrainWidth; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                if (coordsI[i].x == xPos && coordsI[i].z == zPos)
                {
                    iVertex = i;
                }
            }
        }
        return iVertex;
    }




    public void TerrainVertexLevel(float snapX, float snapY, float snapZ, bool levelUp)
    {
        int coordClick = 0;

        for (int i = 0, z = 0; z <= terrainWidth; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                if (coordsF[i].x == snapX && coordsF[i].y == snapY && coordsF[i].z == snapZ)
                {
                    if (levelUp == true)
                    {
                        coordsF[i].y = coordsF[i].y + 0.25f;
                    } 
                    else
                    {
                        coordsF[i].y = coordsF[i].y - 0.25f;
                    }
                    coordClick = i;
                }
            }
        }

        ConvertCoordsFI(true);

        TerrainCollapse(coordClick);

        ConvertCoordsFI(false);

        DeleteTerrainElements();
        CreateTerrainElements();
    }



    private void CreateMapF()
    {
        coordsF = new Vector3[(terrainWidth + 1) * (terrainWidth + 1)];

        for (int i = 0, z = 0; z <= terrainWidth; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                float y = 3.0f;
                coordsF[i] = new Vector3(x, y, z);
            }
        }
    }


    private void CreateMapI()
    {
        coordsI = new Vector3[(terrainWidth + 1) * (terrainWidth + 1)];

        for (int i = 0, z = 0; z <= terrainWidth; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                float y = 3.0f;
                coordsI[i] = new Vector3(x, y, z);
            }
        }
    }






    public void ConvertCoordsFI(bool coordsFI)
    {
        if (coordsFI)
        {
            for (int Ci = 0, Cz = 0; Cz <= terrainWidth; Cz++)
            {
                for (int Cx = 0; Cx <= terrainWidth; Cx++, Ci++)
                {
                    coordsI[Ci].x = coordsF[Ci].x;
                    coordsI[Ci].z = coordsF[Ci].z;

                    foreach (var dictHeightMap in heightMapGridY)
                    {
                        if (coordsF[Ci].y == dictHeightMap.Key)
                        {
                            coordsI[Ci].y = dictHeightMap.Value;
                        }
                    }
                }
            }
        }
        else
        {
            for (int Ci = 0, Cz = 0; Cz <= terrainWidth; Cz++)
            {
                for (int Cx = 0; Cx <= terrainWidth; Cx++, Ci++)
                {
                    coordsF[Ci].x = coordsI[Ci].x;
                    coordsF[Ci].z = coordsI[Ci].z;

                    foreach (var dictHeightMap in heightMapGridY)
                    {
                        if (coordsI[Ci].y == dictHeightMap.Value)
                        {
                            coordsF[Ci].y = dictHeightMap.Key;
                        }
                    }
                }
            }
        }
    }








    private void CreateTerrainElements()
    {
        int tilesPerSide = terrainWidth / elementWidth;
        terrainElements = new TerrainElement[tilesPerSide * tilesPerSide];

        for (int i = 0, z = 0; z < tilesPerSide; z++)
        {
            for (int x = 0; x < tilesPerSide; x++, i++)
            {
                TerrainElement elementInstance = Instantiate(terrainPrefab, this.transform);
                elementInstance.Initialize(x, z);
                terrainElements[i] = elementInstance;
            }
        }
    }



    private void DeleteTerrainElements()
    {
        int tilesPerSide = terrainWidth / elementWidth;

        for (int i = 0, z = 0; z < tilesPerSide; z++)
        {
            for (int x = 0; x < tilesPerSide; x++, i++)
            {
                string[] coordsLand = { x.ToString(), z.ToString() };

                GameObject goLandRegion = GameObject.Find(string.Join("_", coordsLand));
                Destroy(goLandRegion);
            }
        }
    }




}
