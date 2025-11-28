using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class Monsters : MonoBehaviour
{
    public int monster_id;
    public int currentIndex;
    public int destinationIndex;
    private InputAction moveAction;
    private bool isMoving = false; // prevent multiple moves at once
    public delegate void MonsterMoved(int index);
    public MonsterMoved onMonsterMoved;
    public Image monsterImage;
    public Image backgroundImage;
    public Image buffsDebuffsImage;
    public Slider healthBar;
    public Slider manaBar;

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
    }

    void Start()
    {
        destinationIndex = currentIndex;
        monster_id = 0; // Assign a unique ID for this monster
        BoardManager.Instance.RegisterMonster(monster_id, this);
        transform.position = BoardManager.Instance.tilePrefab[currentIndex].transform.position;
    }

    void Update()
    {
        if (isMoving) return; // block input while moving

        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.y > 0) MoveUp();
        else if (input.y < 0) MoveDown();
        else if (input.x < 0) MoveLeft();
        else if (input.x > 0) MoveRight();
    }

    void OnMouseDown()
    {
        BoardManager.currentlyActiveMonster = monster_id;
    }
    private void movementSubscriber(int index)
    {
        Debug.Log($"Monster {monster_id} moved to tile {index}");
        StartCoroutine(LerpToTile(index));
    }

    private void Move(int index)
    {
        ConnectionManager.SendMovement(monster_id, currentIndex, index, "move");
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
                    
                } else return;
            }

            destinationIndex += 1;
            //Move(destinationIndex);
            StartCoroutine(LerpToTile(destinationIndex));
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
                    
                } else return;
            }
            destinationIndex -= 1;
            //Move(destinationIndex);
            StartCoroutine(LerpToTile(destinationIndex));
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
                    
                } else return;
            }

            destinationIndex -= 7;
            //Move(destinationIndex);
            StartCoroutine(LerpToTile(destinationIndex));
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
                    
                } else return;
            }

            destinationIndex += 7;
            //Move(destinationIndex);
            StartCoroutine(LerpToTile(destinationIndex));
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
        isMoving = false;
    }
}