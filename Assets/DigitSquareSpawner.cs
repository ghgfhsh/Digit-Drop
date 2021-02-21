using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquareSpawner : MonoBehaviour
{
    [SerializeField] private GameGrid gameGrid;

    [SerializeField]private GameObject digitPreFab;

    [Range(1, 12)]
    public int digitToSpawn;

    [Range(-1, 4)]
    public int positionToSpawn;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            int tempDigitToSpawn = (int)Mathf.Pow(2, digitToSpawn);
            if (positionToSpawn == -1)
                gameGrid.SpawnSquare(Random.Range(0, gameGrid.gridInfo.gridWidth), digitPreFab, tempDigitToSpawn);
            else
                gameGrid.SpawnSquare(positionToSpawn, digitPreFab, tempDigitToSpawn);
        }
    }

}
