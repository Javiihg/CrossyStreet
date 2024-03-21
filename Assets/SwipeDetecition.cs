using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDetector : MonoBehaviour
{
	private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    public float swipeThreshold = 20f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerUpPos = Input.mousePosition;
            fingerDownPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            fingerDownPos = Input.mousePosition;
            DetectSwipe();
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerDownPos = Input.mousePosition;
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        float verticalSwipeValue = VerticalMoveValue();
        float horizontalSwipeValue = HorizontalMoveValue();

        if (verticalSwipeValue > swipeThreshold && verticalSwipeValue > horizontalSwipeValue)
        {
            // Vertical swipe detected
            if (fingerDownPos.y - fingerUpPos.y > 0)
            {
                OnSwipeUp();
            }
            else if (fingerDownPos.y - fingerUpPos.y < 0)
            {
                OnSwipeDown();
            }
            fingerUpPos = fingerDownPos;
        }
        else if (horizontalSwipeValue > swipeThreshold && horizontalSwipeValue > verticalSwipeValue)
        {
            // Horizontal swipe detected
            if (fingerDownPos.x - fingerUpPos.x > 0)
            {
                OnSwipeRight();
            }
            else if (fingerDownPos.x - fingerUpPos.x < 0)
            {
                OnSwipeLeft();
            }
            fingerUpPos = fingerDownPos;
        }
        else
        {
            Debug.Log("No Swipe Detected!");
        }
    }

    float VerticalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    float HorizontalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    void OnSwipeUp()
    {
        // Do something when swiped up
    }

    void OnSwipeDown()
    {
        // Do something when swiped down
    }

    void OnSwipeLeft()
    {
        // Move left
        rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, 0f);
        Jump();
    }

    void OnSwipeRight()
    {
        // Move right
        rb.velocity = new Vector3(moveSpeed, rb.velocity.y, 0f);
        Jump();
    }

    void Jump()
    {
        // Add upward force for jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}