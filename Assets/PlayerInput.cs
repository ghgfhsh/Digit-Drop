using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    int xPos;

    public void SetXPos(int xPos)
    {
        this.xPos = xPos;
    }

    private void OnMouseDrag()
    {
        if(GameManager.Instance.digitSquareBeingControlled != null)
        {
            GameManager.Instance.digitSquareBeingControlled.PlayerShiftPos(xPos);
        }

        if (GameManager.Instance.currentSpeedMode != GameManager.SpeedMode.combine)
        {
            GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.fallingFaster);
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.currentSpeedMode != GameManager.SpeedMode.combine)
        {
            GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.falling);
        }
    }

}
