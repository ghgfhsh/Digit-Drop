using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquareSpawner : MonoBehaviour
{
    [SerializeField]private GameObject digitPreFab;
    public Transform gridCanvas;

    private bool waitingToSpawn = false;

    private void Update()
    {
        if (GameManager.Instance.digitSquareBeingControlled == null && GameManager.Instance.digitsMoving == 0 && !waitingToSpawn && !GameManager.Instance.isGameOver)
        {
            StartCoroutine(spawnDigitSquare());
        }
    }

    IEnumerator spawnDigitSquare()
    {
        waitingToSpawn = true;
        yield return new WaitForSeconds(.4f);
        if(GameManager.Instance.digitSquareBeingControlled == null && GameManager.Instance.digitsMoving == 0 && !GameManager.Instance.isGameOver)
        {
            int value = (int)Mathf.Pow(2, Random.Range(1, GameManager.Instance.difficultyLevel));
            int position = Random.Range(0, GameManager.Instance.spawnPoints.Count);
            DigitSquare digitSquare = Instantiate(digitPreFab, GameManager.Instance.spawnPoints[position].position, Quaternion.identity, gridCanvas).GetComponent<DigitSquare>();
            digitSquare.SetDigit(value);
            GameManager.Instance.AddScore(1);
        }
        waitingToSpawn = false;
        yield return null;

    }

}
