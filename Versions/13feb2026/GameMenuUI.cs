using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button terrainMenuButton;
    [SerializeField] private GameObject terrainMenuUIPanel;

    void Start()
    {
        terrainMenuButton.onClick.AddListener(TerrainMenuButtonClick);
    }

    void TerrainMenuButtonClick()
    {
        bool newState = !terrainMenuUIPanel.activeSelf;
        terrainMenuUIPanel.SetActive(newState);

        if (!newState)
        {
            GameManager.instance.SetTerrainMode(GameManager.TerrainMode.None);
        }
    }

}
