using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mundo : MonoBehaviour
{
    public int carril = 0;
    public GameObject[] pisos;
    public int pisosDiferencia;
    public float longitudPiso = 1; // Assume each floor is 1 unit long, adjust if not

    public void Start()
    {
        for (int i = 0; i < pisosDiferencia; i++)
        {
            CrearPisos();
        }
    }

    public void CrearPisos()
    {
        Instantiate(pisos[Random.Range(0, pisos.Length)], new Vector3(0, 0, carril * longitudPiso), Quaternion.identity);
        carril++;
    }
}