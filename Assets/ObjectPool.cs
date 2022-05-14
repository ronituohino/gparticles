using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public int amountToPool;
    public GameObject pooledObject;

    public Queue<GameObject> inactive = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            Add(NewParticle());
        }
    }

    public GameObject NewParticle()
    {
        return Instantiate(pooledObject);
    }

    public void Add(GameObject obj)
    {
        obj.SetActive(false);
        inactive.Enqueue(obj);
    }

    public GameObject Get()
    {
        GameObject g = null;
        if (inactive.Count == 0)
        {
            g = NewParticle();
        }
        else
        {
            g = inactive.Dequeue();
            g.SetActive(true);
        }

        return g;
    }
}