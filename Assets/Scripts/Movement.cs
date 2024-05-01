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
    public float distanciaVista = 2;
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
    public AudioClip movementSound;
    public AudioClip loseSound;
    private bool sonidoLose = false;
    public CoinCounterUI coinCounterUI;
    private bool movimientoRealizado = false;

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
        if (coinCounterUI != null)
{
    coinCounterUI.gameObject.SetActive(false);
}
    }

    void Update()
    {
        if (!vivo) return;

    // Resetear el flag al comienzo de cada actualización
    movimientoRealizado = false;

    // Detectar inicio de toque/clic
    if (Input.GetMouseButtonDown(0))
    {
        touchStartPos = Input.mousePosition;
        isSwiping = false;  // Reiniciar la detección de swipe
    }

    // Verificar movimiento continuo del mouse o toque en pantalla
    if (Input.GetMouseButton(0))
    {
        touchEndPos = Input.mousePosition;
        if (!isSwiping && (touchEndPos - touchStartPos).magnitude > minSwipeDistance)
        {
            isSwiping = true;
            ProcesarSwipe();  // Procesa el swipe directamente aquí
        }
    }

    // Al soltar el botón del mouse o levantar el dedo
    if (Input.GetMouseButtonUp(0))
    {
        if (!isSwiping && (touchEndPos - touchStartPos).magnitude < minSwipeDistance)
        {
            Avanzar();  // Avanzar solo si no fue un swipe y el movimiento fue mínimo
        }
        isSwiping = false;  // Restablecer la detección de swipe
    }

    // Teclas de movimiento por teclado
    if (!movimientoRealizado)  // Permite teclas de movimiento si no se ha realizado un toque o swipe
    {
        CheckKeyboardInputs();
    }
}

private void CheckKeyboardInputs()
{
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
            movimientoRealizado = true;
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
        StartCoroutine(CambiarPosicion());
        AudioManager.Instance.PlaySound(movementSound);
        if (posicionZ > carril)
        {
            carril = posicionZ;
            mundo.CrearPisos();
            pasos++;
        }
    }

    public void Retroceder()
    {
        if (!vivo || posicionZ <= carril - 2 * distanciaSaltoZ || MirarAtras()) return;

        posicionZ -= distanciaSaltoZ;
        ActualizarRotacion(Vector3.back);
        StartCoroutine(CambiarPosicion());
        AudioManager.Instance.PlaySound(movementSound);
    }

    public void MoverLados(int cuanto)
{
    if (!vivo) return;

    int nuevaLateral = lateral + cuanto;
    if (EsObstaculoALado(nuevaLateral)) return; // Verifica si hay un obstáculo al lado antes de moverse

    grafico.rotation = Quaternion.Euler(0, 90 * Mathf.Sign(cuanto), 0);
    lateral = Mathf.Clamp(nuevaLateral, -20, 20);
    StartCoroutine(CambiarPosicion());
    AudioManager.Instance.PlaySound(movementSound);
}

private bool EsObstaculoALado(int nuevaLateral)
{
    Vector3 direccion = (nuevaLateral > lateral) ? Vector3.right : Vector3.left;
    RaycastHit hit;
    Vector3 origen = transform.position + Vector3.up * 1.0f; // Ajusta esta altura según la configuración de tus colliders
    if (Physics.Raycast(origen, direccion, out hit, distanciaSaltoLateral, capaObstacles))
    {
        Debug.DrawRay(origen, direccion * distanciaSaltoLateral, Color.red);
        if (hit.collider != null)
        {
            Debug.Log("Obstáculo al lado: " + hit.collider.gameObject.name);
            return true;
        }
    }
    return false;
}

    private void ActualizarRotacion(Vector3 direccion)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direccion);
    grafico.rotation = targetRotation; // aplicar rotación directamente
    }

    public bool MirarAdelante()
{
    RaycastHit hit;
    // Asegura que el rayo empiece justo delante del personaje para evitar colisiones inmediatas
    Vector3 start = transform.position + Vector3.up * 0.5f; // Ajusta esta altura según la altura de tu personaje
    Vector3 direction = transform.forward;
    float checkDistance = distanciaSaltoZ; // o la distancia que prefieras para detectar obstáculos

    Debug.DrawRay(start, direction * checkDistance, Color.green);

    if (Physics.Raycast(start, direction, out hit, checkDistance, capaObstacles))
    {
        Debug.Log("Obstacle ahead: " + hit.collider.name);
        return true;
    }
    return false;
}
    public bool MirarAtras()
{
    RaycastHit hit;
    Vector3 start = grafico.position + Vector3.up * 1.5f; // Ajusta la altura según la posición y tamaño del personaje
    Ray rayo = new Ray(start, -grafico.forward);  // Revisa hacia atrás
    bool isHit = Physics.Raycast(rayo, out hit, distanciaVista, capaObstacles);
    Debug.DrawRay(start, -grafico.forward * distanciaVista, Color.red);

    if (isHit) {
        Debug.Log("Obstacle behind: " + hit.collider.name);
    }
    return isHit;
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tronco"))
        {
            Debug.Log("tocar tronco");
            this.transform.SetParent(other.transform, true);
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
        if (other.CompareTag("Moneda") && coinCounterUI != null)
{
    coinCounterUI.UpdateCoinCount(GameManager.Instance.Coins);
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
            if (!sonidoLose)
            {
                AudioManager.Instance.PlaySound(loseSound);
                sonidoLose = true;
            }
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
