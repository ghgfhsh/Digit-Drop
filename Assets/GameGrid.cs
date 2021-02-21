using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public float xSpacing, ySpacing = 1f;

    //Grid
    public DigitSquare[,] digitSquares { get; private set; }
    public Vector2[,] digitSquarePositions { get; private set; }
    public GridInfo gridInfo = new GridInfo(5, 7); //height and width must be a odd number if someone changes this

    public Transform test;


    private void Start()
    {
        digitSquares = new DigitSquare[gridInfo.gridWidth, gridInfo.gridHeight];
        digitSquarePositions = new Vector2[gridInfo.gridWidth, gridInfo.gridHeight];
        CreateVector2Points();
    }

    public void SpawnSquare(int posX, GameObject square, int value)
    {
        digitSquares[posX, gridInfo.topRow] = Instantiate(square, digitSquarePositions[posX, gridInfo.topRow], Quaternion.identity, test).GetComponent<DigitSquare>();
        digitSquares[posX, gridInfo.topRow].SetDigit(value);
        digitSquares[posX, gridInfo.topRow].OnSpawn(new Coord(posX, gridInfo.topRow), this);
    }

    private void DeleteAllDigitSquares()
    {
        for (int x = 0; x < gridInfo.gridWidth; x++)
        {
            for (int y = 0; y < gridInfo.gridHeight; y++)
            {
                Destroy(digitSquares[x, y].gameObject);
                digitSquares[x, y] = null;
            }
        }
    }

    //this function calls all functions added to the event and passes in the values of the loop
    private void CreateVector2Points()
    {
        for (int x = 0; x < gridInfo.gridWidth; x++)
        {
            for (int y = 0; y < gridInfo.gridHeight; y++)
            {
                digitSquarePositions[x, y] = new Vector2(transform.position.x + x * xSpacing, transform.position.y + y * ySpacing);
            }
        }
    }

    public DigitSquare.SurroundingDigitSquares GetSurroundingDigitSquares(Coord pos)
    {
        GameManager.Instance.digitsMoving--;


        DigitSquare left;
        DigitSquare right;
        DigitSquare below;

        //check that there is a grid spot to the left if so assign it
        if (pos.x - 1 >= 0)
            left = digitSquares[pos.x - 1, pos.y];
        else
            left = null;

        //check that there is a grid spot to the right if so assign it
        if (pos.x + 1 < gridInfo.gridWidth)
            right = digitSquares[pos.x + 1, pos.y];
        else
            right = null;

        //check that there is a grid spot below it if so assign it
        if (pos.y - 1 >= 0)
            below = digitSquares[pos.x, pos.y - 1];
        else
            below = null;

        return new DigitSquare.SurroundingDigitSquares(left, right, below);
    }

    public void MoveDigitSquarePosition(Coord oldPos, Coord newPos)
    {

        //if the space is null then it is falling, otherwise it is doubling the digit it is moving towards and destorying itself
        if (digitSquares[newPos.x, newPos.y] == null)
        {
            digitSquares[newPos.x, newPos.y] = digitSquares[oldPos.x, oldPos.y];
            digitSquares[oldPos.x, oldPos.y] = null;
        }
        else
        {

            digitSquares[newPos.x, newPos.y].doubleDigit();
            Destroy(digitSquares[oldPos.x, oldPos.y].gameObject);
            digitSquares[oldPos.x, oldPos.y] = null;

            digitSquares[newPos.x, newPos.y].UpdateSurroundingDigitSquares();
        }

    }


    public struct GridInfo
    {
        public int gridWidth { get; }
        public int gridHeight { get; }
        public int topRow { get; }

        public GridInfo(int gridWidth, int gridHeight)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            topRow = gridHeight - 1;
        }
    }
}
