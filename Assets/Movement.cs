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
     public int distanciaSaltoZ = 4;
     public int distanciaSaltoLateral = 4;
     public Transform grafico;
     public LayerMask capaObstacles;
     public LayerMask capaAgua;
     public float distanciaVista = 5;
     public bool vivo = true;

     int posicionZ;

    void Start()
    {
        distanciaSaltoLateral = distanciaSaltoZ;
        InvokeRepeating("MirarAbajo", 1, 0.5f);
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
            MoverLados(distanciaSaltoLateral);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoverLados(-distanciaSaltoLateral);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(grafico.position + Vector3.up * 5f, grafico.position + Vector3.up * 5f + grafico.forward * distanciaVista);
    }

    public void ActualizarPosicion()
    {
        if (!vivo)
        {
            return;
        }
        posObjetivo = new Vector3(lateral, 0, posicionZ);
        transform.position = posObjetivo;
    }

    public void Avanzar()
    {
        if (!vivo)
        {
            return;
        }
        grafico.eulerAngles = Vector3.zero;
        if (MirarAdelante())
        {
            return;
        }

        posicionZ += distanciaSaltoZ;
        if (posicionZ > carril)
        {
            carril = posicionZ;
            mundo.CrearPisos();
        }
    }

    public void Retroceder()
    {
        if (!vivo)
        {
            return;
        }
        grafico.eulerAngles = new Vector3(0, 180, 0);
        if (MirarAdelante())
        {
            return;
        }

        if (posicionZ > carril - 3 * distanciaSaltoZ)
        {
            posicionZ-= distanciaSaltoZ;
        }
    }
    public void MoverLados(int cuanto)
    {
        if (!vivo)
        {
            return;
        }
        grafico.rotation = Quaternion.Euler(0, 90 * Mathf.Sign(cuanto), 0);
        if (MirarAdelante())
        {
            return;
        }

        lateral += cuanto;
        lateral = Mathf.Clamp(lateral, -15, 15);
    }

    public bool MirarAdelante()
    {
        RaycastHit hit;
        Ray rayo = new Ray(grafico.position + Vector3.up * 5f, grafico.forward);

        if (Physics.Raycast(rayo, out hit, distanciaVista, capaObstacles))
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coche"))
        {
            vivo = false;
        }
    }

    public void MirarAbajo()
    {
        RaycastHit hit;
        Ray rayo = new Ray(transform.position + Vector3.up, Vector3.down);

        if (Physics.Raycast(rayo, out hit, 3, capaAgua))
        {
            if (hit.collider.CompareTag("Agua"))
            {
                vivo = false;
            }
        }
    }
}