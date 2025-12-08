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

    [NonSerialized]
    public string[] buffsDebuffs = new string[3];
    private Image [] buffsDebuffsImage = new Image[3];

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

        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.y > 0) MoveUp();
        else if (input.y < 0) MoveDown();
        else if (input.x < 0) MoveLeft();
        else if (input.x > 0) MoveRight();
    }

    public void OnSelect()
    {
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

    private void Move(int index, string action)
    {
        //ConnectionManager.SendMovement(monster_id, currentIndex, index, action);
    }
    public void MoveRight()
    {
        if (currentIndex % 7 != 6)
        {
            if (BoardManager.Instance.monsterPositions.ContainsValue(currentIndex + 1))
            {
                int keyOfMonster = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == currentIndex + 1).Key;
                if((monster_id < 24 && keyOfMonster >= 24) || (monster_id >= 24 && keyOfMonster < 24))
                {
                    Move(currentIndex + 1, "attack");
                }
                else
                {
                    Console.WriteLine("Cannot move onto a tile occupied by an ally.");
                    return;
                } 
                    
            } else Move(currentIndex + 1, "move");

            StartCoroutine(LerpToTile(currentIndex + 1));
        }
    }

    public void MoveLeft()
    {
        if (currentIndex % 7 != 0)
        {
            if (BoardManager.Instance.monsterPositions.ContainsValue(currentIndex - 1))
            {
                int keyOfMonster = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == currentIndex - 1).Key;
                if((monster_id < 24 && keyOfMonster >= 24) || (monster_id >= 24 && keyOfMonster < 24))
                {
                    Move(currentIndex - 1, "move");
                } else return;
            } else Move(currentIndex - 1, "move");
            StartCoroutine(LerpToTile(currentIndex - 1));
        }
    }

    public void MoveUp()
    {
        if (currentIndex >= 7)
        {
            if (BoardManager.Instance.monsterPositions.ContainsValue(currentIndex - 7))
            {
                int keyOfMonster = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == currentIndex - 7).Key;
                if((monster_id < 24 && keyOfMonster >= 24) || (monster_id >= 24 && keyOfMonster < 24))
                {
                    Move(currentIndex - 7, "move");
                } else return;
            } else Move(currentIndex - 7, "move");

            StartCoroutine(LerpToTile(currentIndex - 7));
        }
    }

    public void MoveDown()
    {
        if (currentIndex < 42)
        {
            if (BoardManager.Instance.monsterPositions.ContainsValue(currentIndex + 7))
            {
                int keyOfMonster = BoardManager.Instance.monsterPositions.FirstOrDefault(x => x.Value == currentIndex + 7).Key;
                if((monster_id < 24 && keyOfMonster >= 24) || (monster_id >= 24 && keyOfMonster < 24))
                {
                    Move(currentIndex + 7, "move");
                } else return;
            } else Move(currentIndex + 7, "move");

            StartCoroutine(LerpToTile(currentIndex + 7));
        }
    }

    private IEnumerator LerpToTile(int index)
    {
        isMoving = true;

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
        if(buffCount <= 3)
        {
            buffsDebuffsImage[buffCount -1].sprite = newBuffsDebuffsSprite;
            buffsDebuffs[buffCount -1] = buffToAdd;
            return;
        } else
        {
            buffsDebuffsImage[0].sprite = buffsDebuffsImage[1].sprite;
            buffsDebuffsImage[1].sprite = buffsDebuffsImage[2].sprite;
            buffsDebuffsImage[2].sprite = newBuffsDebuffsSprite;   
            buffsDebuffs[0] = buffsDebuffs[1];
            buffsDebuffs[1] = buffsDebuffs[2];
            buffsDebuffs[2] = buffToAdd;         
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