using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquare : MonoBehaviour
{
    //number change info
    int digitValue;
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

    private bool isCombiningUp = false;

    public void SetDigit(int value)
    {
        this.digitValue = value;
        if (digitValue == (int)Mathf.Pow(2, GameManager.Instance.difficultyLevel + 3))
            GameManager.Instance.difficultyLevel++;

        //make the sprite change every color but repeat after the given number of sprites
        int colorIndex = ((int)Mathf.Log(value, 2) - 1) % GameManager.Instance.digitSquareSprites.Length;
        spriteRenderer.sprite = GameManager.Instance.digitSquareSprites[colorIndex];
        gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = value.ToString();
    }

    public void doubleDigit()
    {
        GameManager.Instance.AddScore(digitValue * GameManager.Instance.digitsMoving);
        SetDigit(digitValue * 2);
    }

    public void OnSpawn(Coord spawnPos, GameGrid gameGrid)
    {
        this.gameGrid = gameGrid;
        currentPos = spawnPos;
        CheckSurroundingDigitSquares();
    }

    private void Update()
    {
        if (isMoving)
            MovingToNextPos();
    }

    public void UpdateAbove(DigitSquare digitSquare)
    {
         if (surroundingDigitSquares.above != null)
            surroundingDigitSquares.above.CheckSurroundingDigitSquares();
    }

    private void SetDigitSquareGridPos(Coord newPos)
    {
        GameManager.Instance.digitsMoving--;
        gameGrid.CalculateDigitSquarePositionChange(currentPos, newPos, this);
        currentPos = newPos;
        CheckSurroundingDigitSquares();
    }

    public void SetSquareAbove(DigitSquare digitSquare)
    {
        surroundingDigitSquares.above = digitSquare;
    }

    public void CheckSurroundingDigitSquares()
    {
        surroundingDigitSquares = gameGrid.GetSurroundingDigitSquares(currentPos);

        if (surroundingDigitSquares.below == null && currentPos.y > 0 && !isCombiningUp) // check if there is an empty space below it if so start falling
            FallOneSpace();
        else
        {
            GameManager.Instance.digitSquareBeingControlled = null;
            //all the if statements below check if the surrounding blocks have the same number as this one, if they do move them inwards and double by the amount of blocks moved int


            if (surroundingDigitSquares.below != null && surroundingDigitSquares.below.digitValue == digitValue)
            {
                if ((surroundingDigitSquares.right == null || surroundingDigitSquares.right.digitValue != digitValue) && (surroundingDigitSquares.left == null || surroundingDigitSquares.left.digitValue != digitValue))
                    CombineDown();
                else
                    surroundingDigitSquares.below.CombineUp();
            }

            if (surroundingDigitSquares.right != null && surroundingDigitSquares.right.digitValue == digitValue)
            {
                surroundingDigitSquares.right.CombineLeft();
            }

            if (surroundingDigitSquares.left != null && surroundingDigitSquares.left.digitValue == digitValue)
            {
                surroundingDigitSquares.left.CombineRight();
            }
        }
    }


    public void PlayerShiftPos(int position)
    {
        if (gameGrid.digitSquares[position, currentPos.y] == null)
        {
            Coord newPos = new Coord(position, currentPos.y);

            nextPosVector3 = gameGrid.digitSquarePositions[position, nextPosCoord.y];
            gameGrid.ShiftPos(currentPos, newPos);
            currentPos = newPos;
            nextPosCoord = new Coord(position, nextPosCoord.y);
            transform.position = new Vector3(nextPosVector3.x, transform.position.y, transform.position.z);
        }
    }

    private void CombineRight()
    {
        GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.combine);
        MoveTowardsPosition(1, 0);
    }

    private void CombineLeft()
    {
        GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.combine);
        MoveTowardsPosition(-1, 0);
    }

    private void CombineUp()
    {
        isCombiningUp = true;
        GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.combine);
        MoveTowardsPosition(0, 1);
    }

    private void CombineDown()
    {
        GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.combine);
        MoveTowardsPosition(0, -1);
    }

    //this function will set is falling to true making the block fall one space
    public void FallOneSpace()
    {
        GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.falling);
        MoveTowardsPosition(0, -1);
    }

    void MoveTowardsPosition(int changeX, int changeY)
    {
        nextPosVector3 = gameGrid.digitSquarePositions[currentPos.x + changeX, currentPos.y + changeY];
        nextPosCoord = new Coord(currentPos.x + changeX, currentPos.y + changeY);
        if (isMoving == false && !isCombiningUp)
        {
            GameManager.Instance.digitsMoving++;
        }
        isMoving = true;
    }

    void MovingToNextPos()
    {
        if(transform.position != nextPosVector3)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosVector3, GameManager.Instance.currentMoveSpeed * Time.deltaTime);
        }
        else
        {          
            isMoving = false;
            if (surroundingDigitSquares.above != null)
            {
                surroundingDigitSquares.above.UpdateAbove(this);
            }
            SetDigitSquareGridPos(nextPosCoord);
        }
    }

    public struct SurroundingDigitSquares
    {
        public DigitSquare left;
        public DigitSquare right;
        public DigitSquare below;
        public DigitSquare above;

        public SurroundingDigitSquares(DigitSquare left, DigitSquare right, DigitSquare below, DigitSquare above)
        {
            this.left = left;
            this.right = right;
            this.below = below;
            this.above = above;
        }
    }

}
