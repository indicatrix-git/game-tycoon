using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Splines;


public class GameManager : MonoBehaviour
{
    Camera cam;


    private void Start()
    {
        cam = Camera.main;

        //IndicatrixAPI.instance.LoadTxtTiles();







        //-------------- T E S T ---------------------------------------------



        //IndicatrixAPI.instance.txtSetTile(100, 100, "t");





        //IndicatrixAPI.instance.txtSetTile(100, 100, "asd");


        //using (StreamWriter sw = new StreamWriter("C:\\game\\0000.txt"))
        //{
        //    for (int i = 0; i <= TerrainManager.instance.terrainWidth - 1; i++)
        //    {
        //        for (int j = 0; j <= TerrainManager.instance.terrainWidth - 1; j++)
        //        {
        //            sw.WriteLine(IndicatrixAPI.instance.txtGetTile(i, j));
        //        }
        //    }
        //}






        //terrainWidth = 256 - realne je pocet tilov = 256x256 tilov




        //TO-DO LIST:
        //1.uloha --- LOAD/SAVE z/do suboru/binarny/ z/do pamate
        //2.uloha --- vytvorenie MDLtile analogicky ako TXTtile
        //3.uloha --- do textovej reprezentacie zapisat znak T a podla toho generovat Texturu na danom Tile v mape a vice versa
        //4.uloha --- modelova reprezentacia



    }




    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100000))
        {



            //if (Input.GetKeyDown(KeyCode.M))
            //{
            //    Vector3 snapCenterFace = IndicatrixAPI.instance.SnapMeshFace(hit.point);

            //}

            //Vector3 snapCenterFace = IndicatrixAPI.instance.SnapMeshFace(hit.point);

       

            Vector3 snapPoint = IndicatrixAPI.instance.SnapVertex(hit.point);
            
            Debug.Log(snapPoint.x + " " + snapPoint.y + " " + snapPoint.z);


            if (Input.GetMouseButtonDown(0))
            {
                TerrainManager.instance.TerrainVertexUp(snapPoint.x, snapPoint.y, snapPoint.z);
            }

            //Vector3 snapCenterFace = IndicatrixAPI.instance.SnapMeshFace(hit.point);
            //Vector3 snapLineFaceCenter = IndicatrixAPI.instance.SnapLineFace(hit.point);

            //Debug.Log("SnapPoint: x = " + snapPoint.x + ", y = " + snapPoint.y + ", z = " + snapPoint.z);
            //Vector3 snapLineFaceCenter = IndicatrixAPI.instance.SnapLineFace(hit.point);


        }

    }

}
