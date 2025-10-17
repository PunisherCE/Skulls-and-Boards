using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;

public class ListsCreator : MonoBehaviour
{
    [SerializeField] UIDocument deckManager;

    static List<VisualElement> toDropOn;
    static List<VisualElement> ToPickFrom;

    private void OnEnable()
    {
        toDropOn = deckManager.rootVisualElement.Query<VisualElement>(className: "tiles").ToList();
        ToPickFrom = deckManager.rootVisualElement.Query<VisualElement>(className: "monsters").ToList();

        Debug.Log($"ListsCreator initialized with {toDropOn.Count} drop zones and {ToPickFrom.Count} pick zones.");

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
