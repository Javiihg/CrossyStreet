using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
        public int carril;
        public int lateral;
        public Vector3 posObjetivo;
        public int posicionZ;
        public float posicionYInicial = 0.51f;
        public int saltoDistancia = 2;
        public Mundo mundo;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, posicionYInicial, transform.position.z);
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

        else if (Input.GetKeyDown(KeyCode.D))
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
        posObjetivo = new Vector3 (lateral, posicionYInicial, posicionZ);
        transform.position = posObjetivo;
    }

    public void Avanzar()
    {
        posicionZ += saltoDistancia;

        if (posicionZ > carril)
        {
            carril = posicionZ;
            mundo.CrearPisos();
        }
    }

    public void Retroceder()
    {
        {
            posicionZ -= saltoDistancia;
        }
    }

    public void MoverLados(int cuanto)
    {
        lateral += cuanto;
        lateral = Mathf.Clamp(lateral, -12, 5);
    }
}
