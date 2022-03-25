using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] float touchScalar;

    public delegate void InputResponse(float inutDelta);
    public event InputResponse responseEvent;

    private void Start()
    {

    }

    Vector3 mousePosDelta, mousePosPrev, mousePosCurr;
    [SerializeField] float mouseDeltaX, mouseDeltaY;

    /*
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosCurr = mousePosPrev = Input.mousePosition;
            mousePosDelta = mousePosCurr - mousePosPrev;
        }
        else if (Input.GetMouseButton(0))
        {
            mousePosCurr = Input.mousePosition;
            mousePosDelta = mousePosCurr - mousePosPrev;
            mouseDeltaX = mousePosDelta.x * touchScalar;
            mouseDeltaY = mousePosDelta.y * touchScalar;

            //deltaX should be sent to player to change position and direction
            //event should be fired
            //responseEvent(mouseDeltaX);
            mousePosPrev = mousePosCurr;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ResetInput();
            //responseEvent(0);
        }
    }
    */

    void ResetInput()
    {
        mousePosCurr = mousePosPrev = mousePosDelta = Vector3.zero;
        mouseDeltaX = mousePosDelta.x;
        mouseDeltaY = mousePosDelta.y;
    }
}
