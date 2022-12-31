using System.Collections.Generic;
using UnityEngine;

public class TouchScripts : MonoBehaviour
{
    public Camera gameGamera;
    public ObjectPool pool;

    List<Vector3> touchPositions = new List<Vector3>();

    public PlatformDetection platform;

    // Particle amounts
    public float spawnRate;
    float spawnTimer = 1f;

    [Space]

    float hsv = 0f;
    public float rainbowChangeSpeed = 0.01f;

    [Space]

    int fingerCount;
    public float minLifetime;
    public float maxLifetime;

    bool triggeredFade = false;
    bool faded = false;
    public CanvasGroup cg;

    [Space]

    public float particleSpeed;
    public float particleDrag;
    public float particleMaxSpeed;

    void Update()
    {
        //Get touches
        fingerCount = Input.touchCount;
        Touch[] touches = Input.touches;

        touchPositions.Clear();

        //Record their positions
        for (int i = 0; i < fingerCount; i++)
        {
            Touch t = touches[i];
            Vector3 pos = gameGamera.ScreenToWorldPoint(t.position, Camera.MonoOrStereoscopicEye.Mono);
            touchPositions.Add(new Vector3(pos.x, pos.y, 0f));
        }

        if (!platform.onPhone)
        {
            if (Input.GetMouseButton(0))
            {
                fingerCount = 1;
                Vector3 pos = gameGamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
                touchPositions.Add(new Vector3(pos.x, pos.y, 0f));
            }
        }

        // Start text fade
        if (!faded)
        {
            if (!triggeredFade && touchPositions.Count > 0)
            {
                triggeredFade = true;
            }
            if (triggeredFade)
            {
                cg.alpha = Mathf.Lerp(cg.alpha, 0f, 0.05f);
                if (cg.alpha < 0.01f)
                {
                    cg.alpha = 0f;
                    faded = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Do physics and remove those whose lifetime is up
        int activeParticleCount = 0;
        for(int id = 0; id < pool.particles.Count; id++)
        {
            Particle p = pool.particles[id];
            if(!p.active) 
            {
                continue;
            }
            activeParticleCount++;

            // Physics
            if (fingerCount == 1)
            {
                p.AddVelocity((p.gameObject.transform.position - touchPositions[0]).normalized * particleSpeed);
            }
            else if (fingerCount > 1)
            {
                int nextFinger = GetNextFinger(p.previousFinger);
                if (Vector2.Distance(p.gameObject.transform.position, touchPositions[nextFinger]) < 0.5f)
                {
                    p.previousFinger = nextFinger;
                }

                p.AddVelocity((touchPositions[nextFinger] - p.gameObject.transform.position).normalized * particleSpeed);
            }

            Vector2 screenPoint = gameGamera.WorldToViewportPoint(p.gameObject.transform.position);
            bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            // Removing
            if (!onScreen)
            {
                p.expandedTime += Time.deltaTime;

                if (p.expandedTime > p.lifeTime)
                {
                    p.active = false;
                    pool.Add(p);
                }
            }
            else
            {
                p.expandedTime = 0f;
            }

            p.velocity *= particleDrag;
            p.gameObject.transform.position += p.velocity;
        }

        // Spawning
        spawnTimer += spawnRate;
        if(activeParticleCount < pool.particles.Count && spawnTimer > 1f && fingerCount > 0)
        {
            Particle p = SpawnParticle(Random.Range(0, fingerCount));
            p.AddVelocity(new Vector3(Random.Range(-1f, 1f) * particleSpeed, Random.Range(-1f, 1f) * particleSpeed, 0f));
            spawnTimer = 0f;
        }
    }

    Particle SpawnParticle(int finger)
    {
        // GameObject g = Instantiate(particle, touchPositions[finger], Quaternion.identity, transform) as GameObject;
        Particle p = pool.Get();

        p.active = true;

        p.gameObject.transform.position = touchPositions[finger];
        p.expandedTime = 0f;
        p.lifeTime = Random.Range(minLifetime, maxLifetime);
        p.previousFinger = finger;
        p.velocity = Vector3.zero;
        p.maxVelocity = particleMaxSpeed;

        Color color = Color.HSVToRGB(hsv, 1f, 1f);
        p.spriteRenderer.color = color;
        p.trailRenderer.startColor = color;
        p.trailRenderer.endColor = color;

        p.trailRenderer.Clear();
        hsv += rainbowChangeSpeed;
        if (hsv > 1f)
        {
            hsv = 0f;
        }

        return p;
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
}