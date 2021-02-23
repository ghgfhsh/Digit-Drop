using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("PlayerInput")))
            {
                MouseButtonDown(hit.collider.gameObject.GetComponent<PlayerInputHitBox>().xPos);
            }
        }
        else
        {
            MouseButtonUp();
        }
    }

    private void MouseButtonDown(int xPos)
    {
        if(GameManager.Instance.digitSquareBeingControlled != null)
        {
            GameManager.Instance.digitSquareBeingControlled.PlayerShiftPos(xPos);
            GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.fallingFaster);
        }
    }

    private void MouseButtonUp()
    {
        if (GameManager.Instance.currentSpeedMode != GameManager.SpeedMode.combine)
        {
            GameManager.Instance.SetSpeedMode(GameManager.SpeedMode.falling);
        }
    }

}
