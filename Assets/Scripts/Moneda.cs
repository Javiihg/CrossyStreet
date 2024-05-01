using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    public AudioClip audioClip; // Assign this AudioClip via the Unity Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(1); 
            AudioManager.Instance.PlaySound(audioClip); // Use the AudioManager instance to play the sound
            gameObject.SetActive(false); // Deactivate the coin gameObject after playing the sound
        }
    }
}
