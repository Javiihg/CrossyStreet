using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounterUI : MonoBehaviour
{
    public static CoinCounterUI Instance;
    public TextMeshProUGUI coinText; // Asegúrate de que este componente está correctamente asignado en el editor de Unity

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Opcional: conservar entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCoinCount(int count)
    {
        coinText.text = " " + count;
        ShowAndFade();
    }

    private void ShowAndFade()
    {
        coinText.gameObject.SetActive(true);
        CanvasGroup canvasGroup = coinText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = coinText.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1; // Asegura que el texto es totalmente visible

        // Cancela cualquier animación previa que pudiera estar ejecutándose
        LeanTween.cancel(coinText.gameObject);

        // Aplica una animación para desvanecer el texto
        LeanTween.alphaCanvas(canvasGroup, 0, 1).setDelay(1).setOnComplete(() => {
            coinText.gameObject.SetActive(false); // Desactiva el objeto de texto una vez completada la animación
        });
    }
}