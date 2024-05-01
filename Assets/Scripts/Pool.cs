using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [System.Serializable]
    public class PoolItem
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static Pool Instance;

    [SerializeField]
    private List<PoolItem> items; // Lista para configurar en el inspector
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (PoolItem item in items)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < item.size; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(item.tag, objectQueue);
        }
    }

    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToReuse = poolDictionary[tag].Count > 0 ? poolDictionary[tag].Dequeue() : Instantiate(poolDictionary[tag].Peek()); 
        objectToReuse.SetActive(true);
        return objectToReuse;
    }

    public void ReturnObject(GameObject objectToReturn, string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }
        
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}