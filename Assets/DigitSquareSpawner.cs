using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquareSpawner : MonoBehaviour
{
    [SerializeField] private GameGrid gameGrid;

    [SerializeField]private GameObject digitPreFab;

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
        yield return new WaitForSeconds(.1f);
        int value = (int)Mathf.Pow(2, Random.Range(1, GameManager.Instance.difficultyLevel));
        int position = Random.Range(0, gameGrid.gridInfo.gridWidth);
        gameGrid.SpawnDigit(position, digitPreFab, value);
        GameManager.Instance.AddScore(1);
        waitingToSpawn = false;
        yield return null;

    }

}
