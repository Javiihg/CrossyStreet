using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevolverCoche : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coche") || other.CompareTag("Tronco"))
        {
            other.transform.Translate(0, 0, -80);
        }
    }
}
