using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Static instance accessible globally
    public static BoardManager Instance { get; private set; }
    public static int currentlyActiveMonster = -1;

    public static bool redTurn = true;
    public static int numberOfMoves = 3;
    public static int movedRed = 0;
    public static int movedBlue = 0;

    // Your tile prefabs
    public GameObject[] tilePrefab = new GameObject[49];
    //public Transform monsterParent; // Canvas or an empty object inside the Canvas
    public Dictionary<int, int> monsterPositions = new Dictionary<int, int>();
    public Dictionary<int, Monsters> monsters = new Dictionary<int, Monsters>();


    public delegate void MonsterMovement();

    private void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist across scenes
    }

    void Start()
    {
        // Initialization logic here
    }

    void Update()
    {
        // Per-frame logic here
    }
    public void RegisterMonster(int id, Monsters unit)
    {
        if (!monsters.ContainsKey(id))
            monsters.Add(id, unit);
    }

    public void RegisterPosition(int monsterId, int tileIndex)
    {
        if (monsterPositions.ContainsKey(monsterId))
        {
            monsterPositions[monsterId] = tileIndex;
        }
        else
        {
            monsterPositions.Add(monsterId, tileIndex);
        }
    }
    public Monsters GetMonster(int id)
    {
        monsters.TryGetValue(id, out Monsters unit);
        return unit;
    }

}