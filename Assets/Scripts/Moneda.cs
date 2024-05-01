using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    public AudioClip audioClip; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(1); 
            AudioManager.Instance.PlaySound(audioClip); 
            gameObject.SetActive(false); 
        }
    }
}
