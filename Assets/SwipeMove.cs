using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeMove : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;
    private Touch touch;
    private IEnumerator moveCoroutine;
    private bool coroutineAllowed = true;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended && coroutineAllowed)
            {
                endTouchPosition = touch.position;

                if ((endTouchPosition.y > startTouchPosition.y) && Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    moveCoroutine = Move(new Vector3(0f, 0f, 0.25f));
                    StartCoroutine(moveCoroutine);
                }
                else if ((endTouchPosition.y < startTouchPosition.y) && Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x))
                {
                    moveCoroutine = Move(new Vector3(0f, 0f, -0.25f));
                    StartCoroutine(moveCoroutine);
                }
                else if ((endTouchPosition.x < startTouchPosition.x) && Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                {
                    moveCoroutine = Move(new Vector3(-0.25f, 0f, 0f));
                    StartCoroutine(moveCoroutine);
                }
                else if ((endTouchPosition.x > startTouchPosition.x) && Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                {
                    moveCoroutine = Move(new Vector3(0.25f, 0f, 0f));
                    StartCoroutine(moveCoroutine);
                }
            }
        }
    }

    private IEnumerator Move(Vector3 direction)
    {
        coroutineAllowed = false;

        for (int i = 0; i < 3; i++)
        {
            transform.Translate(direction);
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < 3; i++)
        {
            transform.Translate(-direction);
            yield return new WaitForSeconds(0.01f);
        }

        coroutineAllowed = true;
    }
}