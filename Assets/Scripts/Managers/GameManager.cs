using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject monsterPrefab; // Assign this in the Inspector
    public Sprite redBackgroundSprite; // Assign this in the Inspector
    public Sprite blueBackgroundSprite; // Assign this in the Inspector

    public Dictionary<int, string> redTeam = new Dictionary<int, string>();
    public Dictionary<int, string> blueTeam = new Dictionary<int, string>();
    public Dictionary<int, bool> alreadyMoved = new Dictionary<int, bool>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i <= 11; i++)
        {
            redTeam.Add(i, "Dragon_1_L");
        }
        for (int i = 25; i <= 36; i++)
        {
            blueTeam.Add(i, "Wizzard_1_L");
        }
    }

    void Start()
    {
        instantiateMonsters();
    }

    void Update()
    {
        // Check if the Enter/Return key was pressed this frame
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Reset the alreadyMoved dictionary");
            foreach (var key in alreadyMoved.Keys.ToList())
            {
                alreadyMoved[key] = false;
                if (BoardManager.Instance.monsters.ContainsKey(key))
                {
                    BoardManager.Instance.monsters[key].RevertColor();
                }
            }
        }
    }


    public void instantiateMonsters()
    {
        int tilePositionIndex = -1;
        int monsterID = 0;
        foreach(KeyValuePair<int, string> entry in redTeam)
        {
            if(tilePositionIndex == 6) tilePositionIndex = 7;
            tilePositionIndex++;
            if(tilePositionIndex == 3) continue;
            GameObject monster = Instantiate(monsterPrefab);
            Monsters monsterComponent = monster.GetComponent<Monsters>();
            monsterComponent.monster_id = monsterID;
            monsterComponent.currentIndex = tilePositionIndex;
            monster.transform.position = BoardManager.Instance.tilePrefab[tilePositionIndex].transform.position;
            monster.transform.rotation = Quaternion.Euler(0, 0, 180);
            alreadyMoved.Add(monsterComponent.monster_id, false);
            BoardManager.Instance.RegisterMonster(monsterComponent.monster_id, monsterComponent);
            BoardManager.Instance.RegisterPosition(monsterComponent.monster_id, tilePositionIndex);
            monsterComponent.SetSprite(entry.Value, redBackgroundSprite);
            monsterID++;
            //Debug.Log("Red Team Monster ID: " + entry.Key + ", Name: " + entry.Value);
        }
        tilePositionIndex = 41;
        monsterID = 24;
        foreach(KeyValuePair<int, string> entry in blueTeam)
        {
            if(tilePositionIndex == 48) tilePositionIndex = 35;
            tilePositionIndex++;
            if(tilePositionIndex == 45) continue;
            GameObject monster = Instantiate(monsterPrefab);
            Monsters monsterComponent = monster.GetComponent<Monsters>();
            monsterComponent.monster_id = monsterID;
            monsterComponent.currentIndex = tilePositionIndex;
            monster.transform.position = BoardManager.Instance.tilePrefab[tilePositionIndex].transform.position;
            alreadyMoved.Add(monsterComponent.monster_id, false);
            BoardManager.Instance.RegisterMonster(monsterComponent.monster_id, monsterComponent);
            BoardManager.Instance.RegisterPosition(monsterComponent.monster_id, tilePositionIndex);
            monsterComponent.SetSprite(entry.Value, blueBackgroundSprite);
            monsterID++;
            //Debug.Log("Blue Team Monster ID: " + entry.Key + ", Name: " + entry.Value);
        }
    }

}
