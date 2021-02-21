using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquare : MonoBehaviour
{
    //number change info
    int value;
    [SerializeField]SpriteRenderer spriteRenderer;

    //moving info
    private bool isMoving = false;
    
    //posInfo
    private Coord currentPos;
    private GameGrid gameGrid;
    private SurroundingDigitSquares surroundingDigitSquares;

    //cache
    private Vector3 nextPosVector3;
    private Coord nextPosCoord;

    public void SetDigit(int value)
    {
        this.value = value;
        //make the sprite change every color but repeat after the given number of sprites
        int colorIndex = ((int)Mathf.Log(value, 2) - 1) % GameManager.Instance.digitSquareSprites.Length;
        spriteRenderer.sprite = GameManager.Instance.digitSquareSprites[colorIndex];
        gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = value.ToString();
    }

    public void doubleDigit()
    {
        SetDigit(value * 2);
    }

    public void OnSpawn(Coord spawnPos, GameGrid gameGrid)
    {
        this.gameGrid = gameGrid;
        currentPos = spawnPos;
        GameManager.Instance.digitsMoving++;
        UpdateSurroundingDigitSquares();
    }

    private void Update()
    {
        if (isMoving)
            MovingToNextPos();
    }

    private void SetDigitSquareGridPos(Coord newPos)
    {
        gameGrid.MoveDigitSquarePosition(currentPos, newPos);
        currentPos = newPos;
        UpdateSurroundingDigitSquares();
    }

    public void UpdateSurroundingDigitSquares()
    {
        surroundingDigitSquares = gameGrid.GetSurroundingDigitSquares(currentPos);

        if (surroundingDigitSquares.below == null && currentPos.y > 0) // check if there is an empty space below it if so start falling
            FallOneSpace();
        else
        {
            //all the if statements below check if the surrounding blocks have the same number as this one, if they do move them inwards and double by the amount of blocks moved int
            if (surroundingDigitSquares.below != null && surroundingDigitSquares.below.value == value)
            {
                surroundingDigitSquares.below.MoveUp();
            }

            if (surroundingDigitSquares.right != null && surroundingDigitSquares.right.value == value)
            {
                surroundingDigitSquares.right.MoveLeft();
            }

            if (surroundingDigitSquares.left != null && surroundingDigitSquares.left.value == value)
            {
                surroundingDigitSquares.left.MoveRight();
            }
        }

    }

    private void MoveRight()
    {
        ShiftPos(1, 0);
    }

    private void MoveLeft()
    {
        ShiftPos(-1, 0);
    }

    private void MoveUp()
    {
        ShiftPos(0, 1);
    }

    //this function will set is falling to true making the block fall one space
    private void FallOneSpace()
    {
        ShiftPos(0, -1);
    }

    void ShiftPos(int changeX, int changeY)
    {
        GameManager.Instance.digitsMoving++;
        Debug.Log(currentPos.x + ", " + currentPos.y + ": " + changeX + ", " + changeY);
        nextPosVector3 = gameGrid.digitSquarePositions[currentPos.x + changeX, currentPos.y + changeY];
        nextPosCoord = new Coord(currentPos.x + changeX, currentPos.y + changeY);
        isMoving = true;
    }

    void MovingToNextPos()
    {
        if(transform.position != nextPosVector3)
            transform.position = Vector3.MoveTowards(transform.position, nextPosVector3, GameManager.Instance.moveSpeed * Time.deltaTime);
        else
        {
            isMoving = false;
            SetDigitSquareGridPos(nextPosCoord);
        }
    }

    public struct SurroundingDigitSquares
    {
        public DigitSquare left;
        public DigitSquare right;
        public DigitSquare below;

        public SurroundingDigitSquares(DigitSquare left, DigitSquare right, DigitSquare below)
        {
            this.left = left;
            this.right = right;
            this.below = below;
        }
    }

}
