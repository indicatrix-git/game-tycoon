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


    private const float baseHeight = 2.75f;
    private const float step = 0.25f;


    void Start()
    {
        instance = this;

        coordsF = CreateMap(3.00f);
        coordsI = CreateMap(0);

        CreateTerrainElements();
    }



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
                    coordsF[i].y += levelUp ? 0.25f : -0.25f;

                    coordClick = i;
                }
            }
        }

        ConvertCoordsFI(true);
        TerrainCollapse(coordClick);
        ConvertCoordsFI(false);

        UpdateRegion(coordClick);

    }




    private Vector3[] CreateMap(float initialHeight)
    {
        int size = (terrainWidth + 1) * (terrainWidth + 1);
        Vector3[] map = new Vector3[size];

        int i = 0;
        for (int z = 0; z <= terrainWidth; z++)
        {
            for (int x = 0; x <= terrainWidth; x++)
            {
                map[i++] = new Vector3(x, initialHeight, z);
            }
        }

        return map;
    }




    public void ConvertCoordsFI(bool floatToInt)
    {
        for (int i = 0; i < coordsF.Length; i++)
        {
            if (floatToInt)
            {
                coordsI[i].x = coordsF[i].x;
                coordsI[i].z = coordsF[i].z;

                coordsI[i].y = Mathf.RoundToInt((coordsF[i].y - baseHeight) / step);
            }
            else
            {
                coordsF[i].x = coordsI[i].x;
                coordsF[i].z = coordsI[i].z;

                coordsF[i].y = baseHeight + coordsI[i].y * step;
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
