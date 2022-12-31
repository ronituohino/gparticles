using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    int idCounter = 0;
    public int amountToPool;
    public GameObject pooledObject;

    public List<Particle> particles = new List<Particle>();
    public Queue<Particle> inactive = new Queue<Particle>();

    void Awake()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            Add(NewParticle());
        }
    }

    public Particle NewParticle()
    {
        GameObject g = Instantiate(pooledObject);
        Particle p = new Particle();
        p.gameObject = g;

        p.id = idCounter;
        idCounter++;

        p.spriteRenderer = g.GetComponent<SpriteRenderer>();
        p.trailRenderer = g.GetComponent<TrailRenderer>();

        particles.Add(p);
        return p;
    }

    public void Add(Particle p)
    {
        p.gameObject.SetActive(false);
        inactive.Enqueue(p);
    }

    public Particle Get()
    {
        Particle p;
        if (inactive.Count == 0)
        {
            p = NewParticle();
        }
        else
        {
            p = inactive.Dequeue();
            p.gameObject.SetActive(true);
        }

        return p;
    }
}