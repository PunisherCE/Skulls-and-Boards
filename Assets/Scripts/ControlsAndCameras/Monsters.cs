using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Monsters : MonoBehaviour //, IPointerClickHandler
{
    [NonSerialized]
    public int monster_id;
    [NonSerialized]
    public int currentIndex;
    [NonSerialized]
    public int numberOfMoves = 0;
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
    //public string[] buffsDebuffs = new string[9];
    public List<string> buffDebuffNames = new List<string>();
    public List<Sprite> buffsDebuffsImage = new List<Sprite>();

    public Sprite monsterSprite;
    private AsyncOperationHandle<Sprite> monsterSpriteHandle;
    private Dictionary<string, AsyncOperationHandle<Sprite>> buffSpriteHandles = new();

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
    // Disable input and unsubscribe
    moveAction.Disable();
    onMonsterMoved -= movementSubscriber;

    // Release monster sprite handle if valid
    if (monsterSpriteHandle.IsValid())
        Addressables.Release(monsterSpriteHandle);

    // Release all buff/debuff sprite handles
    foreach (var kvp in buffSpriteHandles)
    {
        var handle = kvp.Value;
        if (handle.IsValid())
            Addressables.Release(handle);
    }

    // Clear dictionary to avoid dangling references
    buffSpriteHandles.Clear();
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
        if (BoardManager.redTurn && monster_id >= 24) return;
        if (!BoardManager.redTurn && monster_id < 24) return;

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
        if (numberOfMoves > 1)
        {
            GameManager.Instance.alreadyMoved[monster_id] = true;      
            mask.color = new Color(0f, 0f, 0f, 0.8f);            
        }
    }
    public void RevertColor()
    {
        mask.color = new Color(1f, 1f, 1f, 0.0f);
    }
    private IEnumerator LerpToTile(int index)
    {
        isMoving = true;
        numberOfMoves++;
        if(monster_id < 24)
            BoardManager.movedRed++;
        else
            BoardManager.movedBlue++;

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
        if ((monster_id < 24 && BoardManager.movedRed >= BoardManager.numberOfMoves) || (monster_id >= 24 && BoardManager.movedBlue >= BoardManager.numberOfMoves))
        {
            BoardManager.redTurn = !BoardManager.redTurn;
            BoardManager.movedRed = 0;
            BoardManager.movedBlue = 0;
            GameManager.Instance.ResetMoves();
        }
    }

    public void SetSprite(string monsterPath, Sprite backgroundSprite)
    {
        // Load the sprite via Addressables and keep the handle
        monsterSpriteHandle = Addressables.LoadAssetAsync<Sprite>(monsterPath);

        // Wait for completion to get the actual Sprite
        Sprite newSprite = monsterSpriteHandle.WaitForCompletion();
        monsterSprite = newSprite;

        // Assign to your UI
        monsterImage.sprite = newSprite;
        backgroundImage.sprite = backgroundSprite;
    }

    public void SetBuffsDebuffsSprite(string buffsDebuffsPath, string buffToAdd)
    {
        if(buffDebuffNames.Contains(buffToAdd))
        {
            // Already have this buff/debuff, do not add again
            return;
        }
        
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(buffsDebuffsPath);
        buffSpriteHandles[buffToAdd] = handle;
        //buffSpriteHandlesToClean.Add(handle);
        Sprite newBuffsDebuffsSprite = handle.Result;
        buffCount ++;
        if(buffCount <= 9)
        {
            buffsDebuffsImage[buffCount -1] = newBuffsDebuffsSprite;
            buffDebuffNames.Add(buffToAdd);
            //buffsDebuffs[buffCount -1] = buffToAdd;
        } else
        {
            buffsDebuffsImage.RemoveAt(0);
            buffsDebuffsImage.Add(newBuffsDebuffsSprite);
            buffDebuffNames.RemoveAt(0);
            buffDebuffNames.Add(buffToAdd);
        }
    }

    public void RemoveBuffsDebuffsSprite(string buffToRemove, Sprite buffsDebuffsSprite)
    {
        if(buffCount == 0) return;
        if (buffSpriteHandles.TryGetValue(buffToRemove, out var handle))
    {
        Addressables.Release(handle);
        buffSpriteHandles.Remove(buffToRemove);
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