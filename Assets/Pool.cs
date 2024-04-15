using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool Instance { get; private set; }

    public GameObject prefabDeNivel; // Asigna tu prefab de nivel aquí en el inspector
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            AddObjectsToPool(1);
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    private void AddObjectsToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var instanceToAdd = Instantiate(prefabDeNivel);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }
}
