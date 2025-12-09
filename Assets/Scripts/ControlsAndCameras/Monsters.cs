using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class Monsters : MonoBehaviour //, IPointerClickHandler
{
    [NonSerialized]
    public int monster_id;
    [NonSerialized]
    public int currentIndex;
    private InputAction moveAction;

    private bool isMoving = false; // prevent multiple moves at once

    public delegate void MonsterMoved(int index);

    public MonsterMoved onMonsterMoved;
    public Image monsterImage;
    public Image backgroundImage;
    public Slider healthBar;
    public Slider manaBar;
    public Image mask;

    [NonSerialized]
    public string[] buffsDebuffs = new string[9];
    public Image [] buffsDebuffsImage = new Image[9];

    private Sprite monsterSpriteToClean;
    private List<Sprite> buffSpritesToClean = new List<Sprite>();
    private int buffCount = 0;

    [NonSerialized]
    public Monster monsterData;
    [NonSerialized]
    public int health;
    [NonSerialized]
    public int mana;
    [NonSerialized]
    public int damage;



    void OnEnable()
    {
        moveAction = new InputAction("Move", InputActionType.Value);
        onMonsterMoved += movementSubscriber;

        // Composite binding for WASD
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        onMonsterMoved -= movementSubscriber;
    }

    void OnDestroy()
    {
        moveAction.Disable();
        onMonsterMoved -= movementSubscriber;
        if (monsterSpriteToClean != null) Addressables.Release(monsterSpriteToClean);
        foreach (Sprite buffSprite in buffSpritesToClean)
        {
            if (buffSprite != null)
            {
                Addressables.Release(buffSprite);
            }
        }
    }

    void Start()
    {
        // 0 if full 1 is empty.
        SetHealth(0f);
        SetMana(0f);
    }

    void Update()
    {
        if (isMoving) return; // block input while moving
        if (InGameChat.isChatFocused) return; // block input if chat is focused
        if (BoardManager.currentlyActiveMonster != monster_id) return;
        if (GameManager.Instance.alreadyMoved[monster_id]) return;

        // Use wasPressedThisFrame to ensure actions are triggered only once per key press
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) { MoveUp(); return; }
            if (Keyboard.current.sKey.wasPressedThisFrame) { MoveDown(); return; }
            if (Keyboard.current.aKey.wasPressedThisFrame) { MoveLeft(); return; }
            if (Keyboard.current.dKey.wasPressedThisFrame) { MoveRight(); return; }
        }
    }

    public void OnSelect()
    {
        // If this monster is already the active monster, do nothing.
        if (BoardManager.currentlyActiveMonster == monster_id) return;
        BoardManager.currentlyActiveMonster = monster_id;
        if (TileInputManager.tileSelected != null)
        {
            SpriteRenderer sr = TileInputManager.tileSelected.GetComponent<SpriteRenderer>();
            sr.color = Color.white;
            TileInputManager.tileSelected = null;
        }
        Debug.Log("Left mouse button ('Select') clicked on " + gameObject.name + " (ID: " + monster_id + ")");
    }

    public void OnShow()
    {
        InGameChat.ShowMonsterCard(monsterData);
    }
    private void movementSubscriber(int index)
    {
        Debug.Log($"Monster {monster_id} moved to tile {index}");
        StartCoroutine(LerpToTile(index));
    }

    private void SendActionToServer(int targetIndex, ActionType action)
    {
        // This is intentionally commented out for client-side testing as per your request.
        // ConnectionManager.SendMovement(monster_id, currentIndex, targetIndex, action);
    }
    public void MoveRight()
    {
        if (currentIndex % 7 != 6)
        {
            int targetIndex = currentIndex + 1;
            if (BoardManager.Instance.monsterPositions.ContainsValue(targetIndex))
            {
                Debug.Log("There is a monster in that tile");
                int keyOfMonsterAtTarget = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == targetIndex).Key;
                if((monster_id < 24 && keyOfMonsterAtTarget >= 24) || (monster_id >= 24 && keyOfMonsterAtTarget < 24))
                {
                    Console.WriteLine("Attacking enemy monster!");
                    AlreadyMoved();
                    SendActionToServer(targetIndex, ActionType.Attack);
                    DestructionTesting(targetIndex);
                } else {
                    Console.WriteLine("Cannot move onto a tile occupied by an ally.");
                }
                return; // Prevent movement if tile is occupied
            }
            // If no monster in target tile, then move
            SendActionToServer(targetIndex, ActionType.Move);
            StartCoroutine(LerpToTile(targetIndex));
        }
    }

    public void MoveLeft()
    {
        if (currentIndex % 7 != 0)
        {   
            int targetIndex = currentIndex - 1;
            if (BoardManager.Instance.monsterPositions.ContainsValue(targetIndex))
            {
                Debug.Log("There is a monster in that tile");
                int keyOfMonsterAtTarget = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == targetIndex).Key;
                if((monster_id < 24 && keyOfMonsterAtTarget >= 24) || (monster_id >= 24 && keyOfMonsterAtTarget < 24))
                {
                    Console.WriteLine("Attacking enemy monster!");
                    AlreadyMoved();
                    SendActionToServer(targetIndex, ActionType.Attack);
                    DestructionTesting(targetIndex);
                } else {
                    Console.WriteLine("Cannot move onto a tile occupied by an ally.");
                }
                return; // Prevent movement if tile is occupied
            }
            // If no monster in target tile, then move
            SendActionToServer(targetIndex, ActionType.Move);
            StartCoroutine(LerpToTile(targetIndex));
        }
    }

    public void MoveUp()
    {
        if (currentIndex >= 7)
        {   
            int targetIndex = currentIndex - 7;
            if (BoardManager.Instance.monsterPositions.ContainsValue(targetIndex))
            {
                Debug.Log("There is a monster in that tile");
                int keyOfMonsterAtTarget = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == targetIndex).Key;
                if((monster_id < 24 && keyOfMonsterAtTarget >= 24) || (monster_id >= 24 && keyOfMonsterAtTarget < 24))
                {
                    Console.WriteLine("Attacking enemy monster!");
                    AlreadyMoved();
                    SendActionToServer(targetIndex, ActionType.Attack);
                    DestructionTesting(targetIndex);
                } else {
                    Console.WriteLine("Cannot move onto a tile occupied by an ally.");
                }
                return; // Prevent movement if tile is occupied
            }
            // If no monster in target tile, then move
            SendActionToServer(targetIndex, ActionType.Move);
            StartCoroutine(LerpToTile(targetIndex));
        }
    }

    public void MoveDown()
    {
        if (currentIndex < 42)
        {   
            int targetIndex = currentIndex + 7;
            if (BoardManager.Instance.monsterPositions.ContainsValue(targetIndex))
            {
                Debug.Log("There is a monster in that tile");
                int keyOfMonsterAtTarget = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == targetIndex).Key;
                if((monster_id < 24 && keyOfMonsterAtTarget >= 24) || (monster_id >= 24 && keyOfMonsterAtTarget < 24))
                {
                    Console.WriteLine("Attacking enemy monster!");
                    AlreadyMoved();
                    SendActionToServer(targetIndex, ActionType.Attack);
                    DestructionTesting(targetIndex);
                } else {
                    Console.WriteLine("Cannot move onto a tile occupied by an ally.");
                }
                return; // Prevent movement if tile is occupied
            }
            // If no monster in target tile, then move
            SendActionToServer(targetIndex, ActionType.Move);
            StartCoroutine(LerpToTile(targetIndex));
        }
    }

    private void DestructionTesting(int indexTo)
    {
        int key = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == indexTo).Key;
        {
            if(BoardManager.Instance.monsters.TryGetValue(key, out Monsters targetMonster))
            {
                Debug.Log($"Monster {targetMonster.monster_id} has been destroyed!");
                BoardManager.Instance.monsters.Remove(targetMonster.monster_id);
                BoardManager.Instance.monsterPositions.Remove(targetMonster.monster_id);
                Destroy(targetMonster.gameObject);
            }
        }
    }
    
    private void AlreadyMoved()
    {
        GameManager.Instance.alreadyMoved[monster_id] = true;
        mask.color = new Color(0f, 0f, 0f, 0.8f);
    }
    public void RevertColor()
    {
        mask.color = new Color(1f, 1f, 1f, 0.0f);
    }
    private IEnumerator LerpToTile(int index)
    {
        isMoving = true;
        AlreadyMoved();
        Vector3 startPos = transform.position;
        Vector3 targetPos = BoardManager.Instance.tilePrefab[index].transform.position;

        float elapsed = 0f;
        float duration = 0.7f; // one second

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentIndex = index;
        transform.position = targetPos; // snap at end
        BoardManager.Instance.RegisterPosition(monster_id, index);
        isMoving = false;
    }

    public void SetSprite(string monsterPath, Sprite backgroundSprite)
    {
        Sprite newSprite = Addressables.LoadAssetAsync<Sprite>(monsterPath).WaitForCompletion();
        monsterImage.sprite = newSprite;
        monsterSpriteToClean = newSprite; // Store the loaded sprite for cleaning up
        backgroundImage.sprite = backgroundSprite;
    }

    public void SetBuffsDebuffsSprite(string buffsDebuffsPath, string buffToAdd)
    {
        Sprite newBuffsDebuffsSprite = Addressables.LoadAssetAsync<Sprite>(buffsDebuffsPath).WaitForCompletion();
        buffSpritesToClean.Add(newBuffsDebuffsSprite); // Store the loaded sprite
        buffCount ++;
        if(buffCount <= 9)
        {
            buffsDebuffsImage[buffCount -1].sprite = newBuffsDebuffsSprite;
            buffsDebuffs[buffCount -1] = buffToAdd;
            return;
        } else
        {
            buffsDebuffsImage[0].sprite = buffsDebuffsImage[1].sprite;
            buffsDebuffsImage[1].sprite = buffsDebuffsImage[2].sprite;
            buffsDebuffsImage[2].sprite = buffsDebuffsImage[3].sprite;
            buffsDebuffsImage[3].sprite = buffsDebuffsImage[4].sprite;
            buffsDebuffsImage[4].sprite = buffsDebuffsImage[5].sprite;
            buffsDebuffsImage[5].sprite = buffsDebuffsImage[6].sprite;
            buffsDebuffsImage[6].sprite = buffsDebuffsImage[7].sprite;
            buffsDebuffsImage[7].sprite = buffsDebuffsImage[8].sprite;
            buffsDebuffsImage[8].sprite = newBuffsDebuffsSprite;   
            buffsDebuffs[0] = buffsDebuffs[1];
            buffsDebuffs[1] = buffsDebuffs[2];
            buffsDebuffs[2] = buffsDebuffs[3];
            buffsDebuffs[3] = buffsDebuffs[4];
            buffsDebuffs[4] = buffsDebuffs[5];
            buffsDebuffs[5] = buffsDebuffs[6];
            buffsDebuffs[6] = buffsDebuffs[7];
            buffsDebuffs[7] = buffsDebuffs[8];
            buffsDebuffs[8] = buffToAdd;         
        }
    }

    public void SetHealth(float healthPercent)
    {
        healthBar.value = healthPercent;
    }

    public void SetMana(float manaPercent)
    {
        manaBar.value = manaPercent;
    }
}