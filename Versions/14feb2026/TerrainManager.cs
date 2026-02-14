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







    //int GetIndex(int x, int z)
    //{
    //    return z * (terrainWidth + 1) + x;
    //}
    //coordsI[GetIndex(x, z)]






    public void TerrainCollapse(int startIndex)
    {
        int size = terrainWidth + 1;

        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(startIndex);
        visited.Add(startIndex);

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();

            Vector3 current = coordsI[currentIndex];
            int cx = (int)current.x;
            int cz = (int)current.z;
            float currentHeight = current.y;

            // 4 smeroví susedia (kríž – NIE diagonály!)
            Vector2Int[] directions =
            {
            new Vector2Int( 1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int( 0, 1),
            new Vector2Int( 0,-1)
        };

            foreach (var dir in directions)
            {
                int nx = cx + dir.x;
                int nz = cz + dir.y;

                if (nx < 0 || nz < 0 || nx > terrainWidth || nz > terrainWidth)
                    continue;

                int neighborIndex = nz * size + nx;

                float neighborHeight = coordsI[neighborIndex].y;
                float delta = Mathf.Abs(currentHeight - neighborHeight);

                if (delta > 1)
                {
                    if (neighborHeight < currentHeight)
                        coordsI[neighborIndex].y = currentHeight - 1;
                    else
                        coordsI[neighborIndex].y = currentHeight + 1;

                    if (!visited.Contains(neighborIndex))
                    {
                        queue.Enqueue(neighborIndex);
                        visited.Add(neighborIndex);
                    }
                }
            }
        }
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

        UpdateRegion(coordClick);

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



    private void UpdateRegion(int centerIndex)
    {
        Vector3 center = coordsI[centerIndex];

        int minX = Mathf.Max(0, (int)center.x - 10);
        int maxX = Mathf.Min(terrainWidth, (int)center.x + 10);

        int minZ = Mathf.Max(0, (int)center.z - 10);
        int maxZ = Mathf.Min(terrainWidth, (int)center.z + 10);

        int elementSize = elementWidth;
        int tilesPerSide = terrainWidth / elementSize;

        HashSet<int> elementsToUpdate = new HashSet<int>();

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int elementX = x / elementSize;
                int elementZ = z / elementSize;

                int elementIndex = elementZ * tilesPerSide + elementX;

                elementsToUpdate.Add(elementIndex);
            }
        }

        foreach (int i in elementsToUpdate)
        {
            terrainElements[i].Rebuild();
        }
    }







}
