using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool Instance;

    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> objects = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Inicializa el pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            objects.Enqueue(newObject);
        }
    }

    public GameObject GetObject()
    {
        if (objects.Count == 0)
        {
            // Si no hay objetos disponibles, crea uno nuevo
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            return newObject;
        }

        GameObject objectToReuse = objects.Dequeue();
        objectToReuse.SetActive(true);
        return objectToReuse;
    }

    public void ReturnObject(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        objects.Enqueue(objectToReturn);
    }
}