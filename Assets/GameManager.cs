using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int totalMonedas = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMonedas(int amount)
    {
        totalMonedas += amount;
    }

    public int GetTotalMonedas()
    {
        return totalMonedas;
    }

    public void ResetMonedas()
    {
        totalMonedas = 0;
    }
}