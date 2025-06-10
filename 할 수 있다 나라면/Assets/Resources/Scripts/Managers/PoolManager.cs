using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> pool = new();

    public void Init() { }

    public GameObject Get(string key, GameObject prefab)
    {
        if (!pool.ContainsKey(key) || pool[key].Count == 0)
        {
            GameObject go = Instantiate(prefab);
            go.name = key;
            return go;
        }
        GameObject obj = pool[key].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void Return(string key, GameObject obj)
    {
        obj.SetActive(false);
        if (!pool.ContainsKey(key)) pool[key] = new Queue<GameObject>();
        pool[key].Enqueue(obj);
    }
}
