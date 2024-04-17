using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SwipeController;

public class SwipeController : MonoBehaviour
{
    Vector3 clickStart;
    Vector3 clickEnd;
    [SerializeField] GameObject player;
    public float ofset = 100f;
    [SerializeField] LeanTweenType easeType;

    public static SwipeController instance;
    public delegate void swipe(Vector3 direction);
    public event swipe OnSwipe;

    private void Awake()
    {
        if (SwipeController.instance == null)
        {
            SwipeController.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickEnd = Input.mousePosition;
            Vector3 diferencia = clickEnd - clickStart;

            if (diferencia == new Vector3(0f, 0f, 0f))
            {
                diferencia.z = 0.0f;
                player.transform.eulerAngles = new Vector3(0f, 0f, 0f);

                if (OnSwipe != null)
                {
                    OnSwipe(diferencia);
                }
            }

            if (Mathf.Abs(diferencia.magnitude) > ofset)
            {
                diferencia = diferencia.normalized;
                diferencia.z = diferencia.y;

                if (Mathf.Abs(diferencia.x) > Mathf.Abs(diferencia.z))
                {
                    diferencia.z = 0.0f;
                    if (diferencia.x < 0.0f)
                    {
                        player.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                    }
                    else
                    {
                        player.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                    }
                }
                else
                {
                    diferencia.x = 0.0f;
                    if (diferencia.z < 0.0f)
                    {
                        diferencia.z = 0.0f;
                        player.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                    }
                    else
                    {
                        diferencia.z = 0.0f;
                        player.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    }
                }

                diferencia.y = 0.0f;

                if (OnSwipe != null)
                {
                    OnSwipe(diferencia);
                }

            }
        }
    }
}