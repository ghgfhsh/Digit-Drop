using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitSquareTrigger : MonoBehaviour
{
    [SerializeField] private DigitSquareTriggerSide digitSquareTriggerSide;
    [SerializeField] private DigitSquare digitSquare;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.layer == 8)
    //        digitSquare.AddSurroundingDigitSquare(digitSquareTriggerSide, collision.gameObject.GetComponent<DigitSquare>());
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == 8)
    //        digitSquare.RemoveSurroundingDigitSquare(digitSquareTriggerSide, collision.gameObject.GetComponent<DigitSquare>());
    //}

    public enum DigitSquareTriggerSide{
        above, below, left, right
    }
}
