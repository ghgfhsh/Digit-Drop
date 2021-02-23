using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquare : MonoBehaviour
{
    //number change info
    int digitValue;
    [SerializeField]SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    //moving info
    private bool isMoving = false;
    
    //posInfo
    private SurroundingDigitSquares surroundingDigitSquares = new SurroundingDigitSquares(null, null, null, null);
    [SerializeField] Transform raycastPoint;

    //cache
    private DigitSquare digitSquareToDouble;

    //state
    bool justSpawned = true;
    bool isFalling;
    bool isplayingAnimation = false;
    [SerializeField]bool isFrozen = false;
    bool isCombiningDown = false;
    bool isUnfreezeTimerActive = false;

    //position checks
    [SerializeField]private Transform rightPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform belowPoint;
    [SerializeField] private Transform abovePoint;

    [SerializeField]public bool debug = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameManager.Instance.digitSquareBeingControlled = this;
        rb.velocity = Vector2.down * GameManager.Instance.currentMoveSpeed;
        isFalling = true;
        GameManager.Instance.digitsMoving++;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver)
            this.enabled = false;


        //don't do anything while the animation is playing
        if (isFrozen)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if(rb.velocity.magnitude <= .5f)
        {
            //when the digit first stops moving after being spawned remove it from the current digit being controlled
            if (justSpawned)
            {
                GameManager.Instance.digitSquareBeingControlled = null;
                justSpawned = false;
            }

            //when it first stops isfalling is toggled and it triggers check surrounding squares
            if (isFalling)
            {
                isFalling = false;
                Debug.Log("stopped");
                GameManager.Instance.digitsMoving--;
                CheckSurroundingDigitSquares();
            }
        }
        else
        {
            if(!isFalling)
                GameManager.Instance.digitsMoving++;
            isFalling = true;
        }

            rb.velocity = Vector2.down * GameManager.Instance.currentMoveSpeed;

    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            this.enabled = false;

        PrintSurroundingDigits(); //Debug code

        if (isplayingAnimation)
        {
            if(transform.position != digitSquareToDouble.gameObject.transform.position)
                transform.position = Vector3.MoveTowards(transform.position, digitSquareToDouble.gameObject.transform.position, GameManager.Instance.fallSpeed * Time.deltaTime);
            else
            {
                digitSquareToDouble.doubleDigit();
                digitSquareToDouble.StartUnfreezeTimer();
                GameManager.Instance.digitsMoving--;


                //since when combining down the digitsquare being doubled is not moving, it needs to be told to check the squares around it after doubling
                if (isCombiningDown)
                {
                }

                Destroy(gameObject);
            }
        }
    }

    private void PrintSurroundingDigits()
    {
        if (debug == true)
        {
            Debug.Log("Left: " + (surroundingDigitSquares.left == null ? "none" : surroundingDigitSquares.left.digitValue.ToString()) +
            ", Right: " + (surroundingDigitSquares.right == null ? "none" : surroundingDigitSquares.right.digitValue.ToString()) +
            ", Below: " + (surroundingDigitSquares.below == null ? "none" : surroundingDigitSquares.below.digitValue.ToString()) +
            ", Below: " + (surroundingDigitSquares.above == null ? "none" : surroundingDigitSquares.above.digitValue.ToString()));
            debug = false;
        }
    }

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

    private void doubleDigit()
    {
        GameManager.Instance.AddScore(digitValue * GameManager.Instance.digitsMoving);
        SetDigit(digitValue * 2);
    }

    private void FreezeDigit()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        isFrozen = true;
    }

    private void UnfreezeDigit()
    {
        rb.simulated = true;
        isFrozen = false;
        isplayingAnimation = false;
        isUnfreezeTimerActive = false;
        CheckSurroundingDigitSquares();
    }

    private void StartUnfreezeTimer()
    {
        if (!isUnfreezeTimerActive)
        {
            isUnfreezeTimerActive = true;
            StartCoroutine(UnfreezeTimer());
        }
    }

    IEnumerator UnfreezeTimer()
    {
        yield return new WaitForSeconds(.2f);
        UnfreezeDigit();
        yield return null;
    }

    private void UpdateSurroundingDigitSquares()
    {
        //update right
        var hit = Physics2D.OverlapCircle(rightPoint.position, 0.1f, LayerMask.GetMask("DigitSquare"));
        if (hit)
            surroundingDigitSquares.right = hit.GetComponent<DigitSquare>();
        else
            surroundingDigitSquares.right = null;

        //update left
        hit = Physics2D.OverlapCircle(leftPoint.position, 0.1f, LayerMask.GetMask("DigitSquare"));
        if (hit)
            surroundingDigitSquares.left = hit.GetComponent<DigitSquare>();
        else
            surroundingDigitSquares.left = null;

        //update above
        hit = Physics2D.OverlapCircle(abovePoint.position, 0.1f, LayerMask.GetMask("DigitSquare"));
        if (hit)
            surroundingDigitSquares.above = hit.GetComponent<DigitSquare>();
        else
            surroundingDigitSquares.above = null;

        //update below
        hit = Physics2D.OverlapCircle(belowPoint.position, 0.1f, LayerMask.GetMask("DigitSquare"));
        if (hit)
            surroundingDigitSquares.below = hit.GetComponent<DigitSquare>();
        else
            surroundingDigitSquares.below = null;

    }

    private void CheckSurroundingDigitSquares()
    {
        UpdateSurroundingDigitSquares();
        bool foundCombination = false;

        if (surroundingDigitSquares.above != null && surroundingDigitSquares.above.tag == "LoseGameTrigger")
            Debug.Log("Lost Game");

        if (surroundingDigitSquares.left == null && surroundingDigitSquares.right == null && surroundingDigitSquares.below != null && surroundingDigitSquares.below.digitValue == digitValue)  //shift down animation
        {
            foundCombination = true;
            isCombiningDown = true;
            Combine(surroundingDigitSquares.below);
        }
        else
        {
            if(surroundingDigitSquares.left != null && surroundingDigitSquares.left.digitValue == digitValue)
            {
                foundCombination = true;
                surroundingDigitSquares.left.Combine(this);
            }

            if (surroundingDigitSquares.right != null && surroundingDigitSquares.right.digitValue == digitValue)
            {
                foundCombination = true;
                surroundingDigitSquares.right.Combine(this);
            }

            if (surroundingDigitSquares.below != null && surroundingDigitSquares.below.digitValue == digitValue)
            {
                foundCombination = true;
                surroundingDigitSquares.below.Combine(this);
            }
        }

        if (!foundCombination)
        {
            //checkLostGame
            var hit = Physics2D.OverlapCircle(abovePoint.position, 0.1f, LayerMask.GetMask("LoseGameTrigger"));
            if (hit)
                GameManager.Instance.GameOver();
        }
    }


    private void Combine(DigitSquare digitSquare)
    {
        digitSquareToDouble = digitSquare;
        GameManager.Instance.digitsMoving++;
        GetComponent<BoxCollider2D>().enabled = false;

        //disable functionality
        FreezeDigit();
        digitSquareToDouble.FreezeDigit();


        isplayingAnimation = true;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void AddSurroundingDigitSquare(DigitSquareTrigger.DigitSquareTriggerSide digitSquareTriggerSide, DigitSquare digitSquare)
    {
        switch (digitSquareTriggerSide)
        {
            case DigitSquareTrigger.DigitSquareTriggerSide.left:
                surroundingDigitSquares.left = digitSquare;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.right:
                surroundingDigitSquares.right = digitSquare;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.below:
                surroundingDigitSquares.below = digitSquare;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.above:
                surroundingDigitSquares.above = digitSquare;
                break;
        }
    }

    public void RemoveSurroundingDigitSquare(DigitSquareTrigger.DigitSquareTriggerSide digitSquareTriggerSide, DigitSquare digitSquare)
    {
        //ontrigger enter will be called first so the surrounding digit square will be changed. There for when ontriggerexit calls if there was no new square, 
        //it will be the same one being removed instead of the new one 
        switch (digitSquareTriggerSide)
        {
            case DigitSquareTrigger.DigitSquareTriggerSide.left:
                if (surroundingDigitSquares.left == digitSquare)
                    surroundingDigitSquares.left = null;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.right:
                if (surroundingDigitSquares.right == digitSquare)
                    surroundingDigitSquares.right = null;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.below:
                if (surroundingDigitSquares.below == digitSquare)
                    surroundingDigitSquares.below = null;
                break;
            case DigitSquareTrigger.DigitSquareTriggerSide.above:
                if (surroundingDigitSquares.above == digitSquare && rb.simulated == true)
                    surroundingDigitSquares.above = null;
                break;
        }
    }

    public void PlayerShiftPos(int xPos)
    {
        Vector2 newPos = new Vector3(GameManager.Instance.spawnPoints[xPos].position.x, transform.position.y, transform.position.z);
        float direction = (newPos - (Vector2)raycastPoint.transform.position).x;

        var hit = Physics2D.Raycast(raycastPoint.position, new Vector2(direction, 0f), Vector2.Distance(newPos, transform.position), layerMask : LayerMask.GetMask("DigitSquare"));

        if(!hit)
        {
            Debug.Log("safe");
            transform.position = newPos;
        }
        else
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.Log("Unsafe");
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
