using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;
    private float minSwipeDistance = 50f; 
    private int posicionZ;
    public int pasos = 0;
    public TextMeshProUGUI textoPasos;
    public GameObject botonReiniciar;

    void Start()
    {
        distanciaSaltoLateral = distanciaSaltoZ;
        InvokeRepeating("MirarAbajo", 1, 0.5f);
        textoPasos.gameObject.SetActive(false);
        botonReiniciar.SetActive(false);
    }

    void Update()
    {
        if (!vivo) return;

        ActualizarPosicion();

        // Teclas de movimiento
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

        // swipe raton
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            touchEndPos = Input.mousePosition;
            ProcesarSwipe();
            isSwiping = false;
        }
    }
    

    void OnDrawGizmos()
{
    Gizmos.color = Color.green;
    Gizmos.DrawLine(grafico.position + Vector3.up * 5f, grafico.position + Vector3.up * 5f + grafico.forward * distanciaVista);
}

    private void ProcesarSwipe()
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;
        if (swipeDelta.magnitude > minSwipeDistance)
        {
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // swipe lateral
                MoverLados(Mathf.RoundToInt(Mathf.Sign(swipeDelta.x) * distanciaSaltoLateral));
            }
            else
            {
                // swipe vertical
                if (swipeDelta.y > 0)
                {
                    Avanzar();
                }
                else
                {
                    Retroceder();
                }
            }
        }
    }

    // Métodos de movimiento y rotación

    public void ActualizarPosicion()
    {
        if (!vivo) return;

        posObjetivo = new Vector3(lateral, 0, posicionZ);
        transform.position = posObjetivo;
    }

    public void Avanzar()
    {
        if (!vivo || MirarAdelante()) return;

        posicionZ += distanciaSaltoZ;
        ActualizarRotacion(Vector3.forward);
        if (posicionZ > carril)
        {
            carril = posicionZ;
            mundo.CrearPisos();
            pasos++;
        }
    }

    public void Retroceder()
    {
        if (!vivo || posicionZ <= carril - 3 * distanciaSaltoZ || MirarAdelante()) return;

        posicionZ -= distanciaSaltoZ;
        ActualizarRotacion(Vector3.back);
    }

    public void MoverLados(int cuanto)
{
    if (!vivo)
    {
        return;
    }

    grafico.rotation = Quaternion.Euler(0, 90 * Mathf.Sign(cuanto), 0); // permitir rotacion

    // comprobar obstáculos para movimiento
    if (MirarAdelante() && (cuanto == distanciaSaltoLateral || cuanto == -distanciaSaltoLateral))
    {
        return;
    }

    lateral += cuanto;
    lateral = Mathf.Clamp(lateral, -15, 15);
}

    private void ActualizarRotacion(Vector3 direccion)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direccion);
    grafico.rotation = targetRotation; // aplicar rotación directamente
    }

    public bool MirarAdelante()
    {
        RaycastHit hit;
    Ray rayo = new Ray(grafico.position + Vector3.up * 5f, grafico.forward);
    bool isHit = Physics.Raycast(rayo, out hit, distanciaVista, capaObstacles);
    if (isHit) {
        Debug.Log("Hit: " + hit.collider.name);
    }
    return isHit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coche")) 
        vivo = false;
        MostrarPasosFinales();
    }

    public void MirarAbajo()
    {
        RaycastHit hit;
        Ray rayo = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(rayo, out hit, 3, capaAgua) && hit.collider.CompareTag("Agua")) 
        vivo = false;
        MostrarPasosFinales();
    }

    private void MostrarPasosFinales()
    {
        if(!vivo)
        {
            textoPasos.text = "Pasos " + pasos.ToString() + "\nCoins " + GameManager.Instance.Coins;
            textoPasos.gameObject.SetActive(true); 
            botonReiniciar.SetActive(true); 
        }
    }

    public void ReiniciarJuego()
    {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}      