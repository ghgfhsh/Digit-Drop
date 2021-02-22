using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Pattern
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public Sprite[] digitSquareSprites;


    //game state
    public int digitsMoving = 0;
    public DigitSquare digitSquareBeingControlled;

    //fall speed
    public float currentMoveSpeed { get; private set; }
    [SerializeField] private float fallSpeed;
    [SerializeField] private float fallSpeedFaster;
    [SerializeField] private float combineSpeed;
    public SpeedMode currentSpeedMode { get; private set; } = SpeedMode.falling;

    public int difficultyLevel = 4;

    private int score = 0;
    [SerializeField] private TMPro.TMP_Text scoreText;

    public bool isGameOver = false;
    [SerializeField] private GameObject gameOverScreen;

    private void Start()
    {
        currentMoveSpeed = fallSpeed;
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverScreen.SetActive(true);
    }

    public void SetSpeedMode(SpeedMode speedMode)
    {
        if(speedMode == SpeedMode.combine)
        {
            currentMoveSpeed = combineSpeed;
            currentSpeedMode = SpeedMode.combine;
        }
        else if (speedMode == SpeedMode.falling)
        {
            currentMoveSpeed = fallSpeed;
            currentSpeedMode = SpeedMode.falling;
        }
        else if (speedMode == SpeedMode.fallingFaster && digitSquareBeingControlled != null)
        {
            currentMoveSpeed = fallSpeedFaster;
            currentSpeedMode = SpeedMode.fallingFaster;
        }
    }

    public void AddScore(int score)
    {
        this.score += score * 10;
        scoreText.text = this.score.ToString();
    }

    public enum SpeedMode
    {
        falling, fallingFaster, combine
    }

}
