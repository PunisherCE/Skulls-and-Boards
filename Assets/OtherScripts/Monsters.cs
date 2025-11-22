using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Monsters : MonoBehaviour
{
    public int monster_id;
    public int currentIndex;
    private InputAction moveAction;
    private bool isMoving = false; // prevent multiple moves at once
    public delegate void MonsterMoved(int index);
    public MonsterMoved onMonsterMoved;

    void OnEnable()
    {
        moveAction = new InputAction("Move", InputActionType.Value);

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
    }

    void Start()
    {
        currentIndex = 0;
        monster_id = 0; // Assign a unique ID for this monster
        BoardManager.Instance.RegisterMonster(monster_id, this);
        onMonsterMoved += (index) =>
        {
            Debug.Log($"Monster {monster_id} moved to tile {index}");
            StartCoroutine(LerpToTile(index));
        };
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

    private void Move(int index)
    {
        ConnectionManager.SendMovement(monster_id, currentIndex, index, "move");
    }
    public void MoveRight()
    {
        if (currentIndex % 7 != 6)
        {
            currentIndex += 1;
            //Move(currentIndex);
            StartCoroutine(LerpToTile(currentIndex));
        }
    }

    public void MoveLeft()
    {
        if (currentIndex % 7 != 0)
        {
            currentIndex -= 1;
            //Move(currentIndex);
            StartCoroutine(LerpToTile(currentIndex));
        }
    }

    public void MoveUp()
    {
        if (currentIndex >= 7)
        {
            currentIndex -= 7;
            //Move(currentIndex);
            StartCoroutine(LerpToTile(currentIndex));
        }
    }

    public void MoveDown()
    {
        if (currentIndex < 42)
        {
            currentIndex += 7;
            //Move(currentIndex);
            StartCoroutine(LerpToTile(currentIndex));
        }
    }

    private IEnumerator LerpToTile(int index)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = BoardManager.Instance.tilePrefab[index].transform.position;

        float elapsed = 0f;
        float duration = 1f; // one second

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // snap at end
        isMoving = false;
    }
}