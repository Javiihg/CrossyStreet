using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public static SwipeController instance;
    Vector3 clickInicial;
    Vector3 alSoltarClick;
    
    float offset = 100f;
    float moveSpeed = 5f;

    public delegate void Swipe(Vector3 direction);
    public event Swipe OnSwipe;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickInicial = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            alSoltarClick = Input.mousePosition;

            Vector3 diferencia = alSoltarClick - clickInicial;

            if (Mathf.Abs(diferencia.magnitude) > offset)
            {
                diferencia = diferencia.normalized;
                diferencia.z = diferencia.y;

                if (Mathf.Abs(diferencia.x) > Mathf.Abs(diferencia.z))
                {
                    diferencia.z = 0.0f;
                }
                else
                {
                    diferencia.x = 0.0f;
                }

                diferencia.y = 0.0f;

                if (OnSwipe != null)
                {
                    OnSwipe(diferencia);
                    Move(diferencia);
                }
            }
        }
    }

    void Move(Vector3 direction)
    {
        // Mueve el objeto en la dirección del deslizamiento
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}