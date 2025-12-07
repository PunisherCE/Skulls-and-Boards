using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesSelection : MonoBehaviour
{

    public int tileIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect()
    {
        // If there's an active monster and this tile has a monster on it,
        // it's a potential attack/interaction target.
        if (BoardManager.currentlyActiveMonster != -1 && BoardManager.Instance.monsterPositions.ContainsKey(tileIndex))
        {
            // TODO: Implement monster movement/attack logic here.
            Debug.LogFormat("Monster {0} is moving to attack monster on tile {1}", BoardManager.currentlyActiveMonster, tileIndex);
            return; // Exit early, we don't want to select the tile itself.
        }

        // If we click the same tile again, do nothing.
        if (gameObject == TileInputManager.tileSelected) return;

        // Set this tile as the newly selected one.
        BoardManager.currentlyActiveMonster = -1;
        TileInputManager.tileSelected = gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0f, 0.5f, 0f, 0.5f);
        Debug.Log("Tile selected: " + gameObject.name);
    }
}
