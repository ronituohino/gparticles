using System.Collections.Generic;
using UnityEngine;

public class TouchScripts : MonoBehaviour
{
    List<Vector3> touchPositions = new List<Vector3>();
    public GameObject particle;

    public PlatformDetection platform;
    public Gradient[] gradients;
    public Color[] colors;

    //Particle amounts
    public int maxAmount;
    public int spawnRate;

    [Space]

    int currentAmount;
    int fingerCount;
    public float minLifetime;
    public float maxLifetime;

    [Space]

    public float particleSpeed;

    List<Particle> allParticles = new List<Particle>();

    float time;
    float spawnInterval;

    void Start()
    {
        spawnInterval = 1f / spawnRate;
    }

    void Update()
    {
        //Get touches
        fingerCount = Input.touchCount;
        Touch[] touches = Input.touches;

        touchPositions.Clear();

        //Record their positions
        for (int i = 0; i < fingerCount; i++)
        {
            RaycastHit rch;
            Physics.Raycast(Camera.main.ScreenPointToRay(touches[i].position, Camera.MonoOrStereoscopicEye.Mono), out rch);
            touchPositions.Add(rch.point);
        }

        if (!platform.onPhone)
        {
            if (Input.GetMouseButton(0))
            {
                fingerCount = 1;
                RaycastHit rch;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out rch);
                touchPositions.Add(rch.point);
            }
        }



        //Spawning timing
        time += Time.deltaTime;
        int amount = Mathf.FloorToInt(time / spawnInterval);
        if (amount > 0)
        {
            time = 0f;
        }
        if (currentAmount + amount > maxAmount)
        {
            amount = maxAmount - currentAmount;
        }




        //Spawning
        for (int it = 0; it < amount; it++)
        {
            if (fingerCount > 0)
            {
                Particle p = null;
                p = SpawnParticles(fingerCount);
                p.rigidbody.AddForce(new Vector2(Random.Range(-1f, 1f) * particleSpeed, Random.Range(-1f, 1f) * particleSpeed), ForceMode2D.Force);

                int colorNum = Random.Range(0, gradients.Length - 1);
                p.trailRenderer.colorGradient = gradients[colorNum];
                p.spriteRenderer.color = colors[colorNum];

                if (p != null)
                {
                    allParticles.Add(p);
                }
            }
        }


        //Keep track of particles, do physics and remove those whose lifetime is up
        List<Particle> particlesToRemove = new List<Particle>();

        foreach (Particle p in allParticles)
        {
            //Physics
            Rigidbody2D r = p.rigidbody;

            if (fingerCount == 1)
            {
                r.AddForce((r.transform.position - touchPositions[0]).normalized * particleSpeed);
            }
            else if (fingerCount > 1)
            {
                int nextFinger = GetNextFinger(p.previousFinger);
                if (Vector2.Distance(r.transform.position, touchPositions[nextFinger]) < 0.5f)
                {
                    p.previousFinger = nextFinger;
                }

                r.AddForce((touchPositions[nextFinger] - r.transform.position).normalized * particleSpeed);
            }

            Vector2 screenPoint = Camera.main.WorldToViewportPoint(r.transform.position);
            bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            //Removing
            if (!onScreen)
            {
                p.expandedTime += Time.deltaTime;

                if (p.expandedTime > p.lifeTime)
                {
                    particlesToRemove.Add(p);
                }
            } else
            {
                p.expandedTime = 0f;
            }
        }

        foreach (Particle p in particlesToRemove)
        {
            allParticles.Remove(p);
            Destroy(p.rigidbody.gameObject);
        }
        particlesToRemove.Clear();

        currentAmount = allParticles.Count;
    }

    Particle SpawnParticles(int touchAmount)
    {
        int finger = Random.Range(0, touchAmount);
        GameObject g = Instantiate(particle, touchPositions[finger], Quaternion.identity, transform) as GameObject;
        return new Particle(g.GetComponent<SpriteRenderer>(), g.GetComponent<Rigidbody2D>(), g.GetComponent<TrailRenderer>(), Random.Range(minLifetime, maxLifetime), 0f, finger);
    }

    int GetNextFinger(int finger)
    {
        if (finger + 1 >= fingerCount)
        {
            return 0;
        }
        else
        {
            return finger + 1;
        }
    }

    public class Particle
    {
        public SpriteRenderer spriteRenderer;
        public Rigidbody2D rigidbody;
        public TrailRenderer trailRenderer;
        public float lifeTime;
        public float expandedTime;
        public int previousFinger;

        public Particle(SpriteRenderer spriteRenderer, Rigidbody2D rigidbody, TrailRenderer trail, float lifeTime, float expandedTime, int previousFinger)
        {
            this.spriteRenderer = spriteRenderer;
            this.rigidbody = rigidbody;
            this.trailRenderer = trail;
            this.lifeTime = lifeTime;
            this.expandedTime = expandedTime;
            this.previousFinger = previousFinger;
        }
    }


}

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
