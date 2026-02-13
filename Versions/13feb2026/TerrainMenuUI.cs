using System;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMenuUI : MonoBehaviour
{
    [SerializeField] private Button terrainMenuLevelUpButton;
    [SerializeField] private Button terrainMenuLevelDownButton;

    void Start()
    {
        terrainMenuLevelUpButton.onClick.AddListener(TerrainMenuLevelUpButtonClick);
        terrainMenuLevelDownButton.onClick.AddListener(TerrainMenuLevelDownButtonClick);
    }

    private void TerrainMenuLevelUpButtonClick()
    {
        GameManager.instance.SetTerrainMode(GameManager.TerrainMode.LevelUp);
    }

    private void TerrainMenuLevelDownButtonClick()
    {
        GameManager.instance.SetTerrainMode(GameManager.TerrainMode.LevelDown);
    }



}
