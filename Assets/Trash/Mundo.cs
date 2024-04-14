using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mundo : MonoBehaviour
{
    public int carril = 0;
    public GameObject[] pisos;

    public void CrearPisosDiez()
    {
        for (int i = 0; i < 10; i++)
        {
            CrearPisos();
        }
    }

    public void CrearPisos()
    {
        Instantiate(pisos[Random.Range(0, pisos.Length)], Vector3.forward * carril, Quaternion.identity);
        carril++;
    }
}
