using UnityEngine;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;

public class TerrainUIwindow : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform dragRectTransform;

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta;
    }
}
