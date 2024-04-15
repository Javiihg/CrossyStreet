using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
     public int carril;
     public int lateral;
     public Vector3 posObjetivo;
     public float velocidad;
     public Mundo mundo;

     int posicionZ;

    void Start()
    {
        
    }

    void Update()
    {
        ActualizarPosicion();

        if (Input.GetKeyDown(KeyCode.W))
        {
            Avanzar();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Retroceder();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            MoverLados(1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoverLados(-1);
        }
    }

    public void ActualizarPosicion()
    {
        posObjetivo = new Vector3(lateral, 0, posicionZ);
        transform.position = posObjetivo;
    }

    public void Avanzar()
    {
        posicionZ++;
        if (posicionZ > carril)
        {
            carril = posicionZ;
            mundo.CrearPisos();
        }
    }

    public void Retroceder()
    {
        if (posicionZ > carril - 3)
        {
            posicionZ--;
        }
    }
    public void MoverLados(int cuanto)
    {
        lateral += cuanto;
        lateral = Mathf.Clamp(lateral, -5, 5);
    }
}