using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
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
    public TextMeshProUGUI textoRecord;
    public GameObject botonReiniciar;
    public GameObject background;
    public GameObject botonSalir;
    public AnimationCurve curve;
    public Animator animaciones;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distanciaSaltoLateral = distanciaSaltoZ;
        InvokeRepeating("MirarAbajo", 1, 0.5f);
        textoPasos.gameObject.SetActive(false);
        textoRecord.gameObject.SetActive(false);
        botonReiniciar.SetActive(false);
        background.SetActive(false);
        botonSalir.SetActive(false);
        Debug.Log("Record: " + PlayerPrefs.GetInt("Record", 0));
        MostrarRecord();
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

    public IEnumerator CambiarPosicion()
    {
        posObjetivo = new Vector3(lateral, 0, posicionZ);
        Vector3 posActual = transform.position;

        for (int i =0; i < 10; i++)
        {
            transform.position = Vector3.Lerp(posActual, posObjetivo, i * 0.1f) + Vector3.up * curve.Evaluate(i * 0.1f);
            yield return new WaitForSeconds(1f / velocidad);
        }
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
        StartCoroutine(CambiarPosicion());
    }

    public void Retroceder()
    {
        if (!vivo || posicionZ <= carril - 1 * distanciaSaltoZ || MirarAdelante()) return;

        posicionZ -= distanciaSaltoZ;
        ActualizarRotacion(Vector3.back);
        StartCoroutine(CambiarPosicion());
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
    lateral = Mathf.Clamp(lateral, -20, 20);
    StartCoroutine(CambiarPosicion());
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
        if (other.CompareTag("Tronco"))
        {
            Debug.Log("tocar tronco");
            this.transform.SetParent(other.transform);
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        if (other.CompareTag("Coche"))
        {
            animaciones.SetTrigger("morir");
            vivo = false;
            MostrarPasosFinales();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tronco"))
        {
            Debug.Log("salir tronco");
            this.transform.SetParent(null);
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    public void MirarAbajo()
    {
        RaycastHit hit;
        Ray rayo = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(rayo, out hit, 3, capaAgua) && hit.collider.CompareTag("Agua")) 
        {
            animaciones.SetTrigger("agua");
            vivo = false;
            MostrarPasosFinales();
        }
    }

    private void MostrarRecord()
    {
        int recordPasos = PlayerPrefs.GetInt("Record", 0);
        textoRecord.text = "Record: " + recordPasos;
    }


    private void MostrarPasosFinales()
    {
        if(!vivo)
        {
            int recordPasos = PlayerPrefs.GetInt("Record", 0);
            if (pasos > recordPasos)
            {
                PlayerPrefs.SetInt("Record", pasos);
                PlayerPrefs.Save();
                MostrarRecord();
            }
            textoPasos.text = "Score " + pasos.ToString() + "\nCoins " + GameManager.Instance.Coins;
            textoPasos.gameObject.SetActive(true); 
            botonReiniciar.SetActive(true); 
            background.SetActive(true);
            botonSalir.SetActive(true);
            textoRecord.gameObject.SetActive(true);
        }
    }
    public void ReiniciarJuego()
    {
         GameManager.Instance.ResetCoins();
         SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void SalirMenu()
    {
        SceneManager.LoadScene("MenuInicial");
    }
}