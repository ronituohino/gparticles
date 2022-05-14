using System.Collections.Generic;
using UnityEngine;

public class TouchScripts : MonoBehaviour
{
    public Camera gameGamera;
    public ObjectPool pool;

    List<Vector3> touchPositions = new List<Vector3>();

    public PlatformDetection platform;

    // Particle amounts
    public int spawnRate;

    [Space]

    float hsv = 0f;
    public float rainbowChangeSpeed = 0.01f;

    [Space]

    int fingerCount;
    public float minLifetime;
    public float maxLifetime;

    bool triggeredFade = false;

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
        int currentAmount = pool.amountToPool - pool.inactive.Count;
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

        if (!triggeredFade && touchPositions.Count > 0)
        {

        }

        // Spawning timing
        time += Time.deltaTime;
        int amount = Mathf.FloorToInt(time / spawnInterval);
        if (amount > 0)
        {
            time = 0f;
        }
        if (currentAmount + amount > pool.amountToPool)
        {
            amount = pool.amountToPool - currentAmount;
        }

        // Spawning
        for (int it = 0; it < amount; it++)
        {
            if (fingerCount > 0)
            {
                Particle p = SpawnParticle(Random.Range(0, fingerCount));
                p.rb.AddForce(new Vector2(Random.Range(-1f, 1f) * particleSpeed, Random.Range(-1f, 1f) * particleSpeed), ForceMode2D.Force);

                allParticles.Add(p);
            }
        }


        // Do physics and remove those whose lifetime is up
        foreach (Particle p in allParticles)
        {
            if (p.gameObject.activeInHierarchy)
            {
                // Physics
                Rigidbody2D r = p.rb;

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

                Vector2 screenPoint = gameGamera.WorldToViewportPoint(r.transform.position);
                bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                // Removing
                if (!onScreen)
                {
                    p.expandedTime += Time.deltaTime;

                    if (p.expandedTime > p.lifeTime)
                    {
                        pool.Add(p.gameObject);
                    }
                }
                else
                {
                    p.expandedTime = 0f;
                }
            }

        }

        currentAmount = allParticles.Count;
    }

    Particle SpawnParticle(int finger)
    {
        // GameObject g = Instantiate(particle, touchPositions[finger], Quaternion.identity, transform) as GameObject;
        GameObject g = pool.Get();
        g.transform.position = touchPositions[finger];

        Particle p = g.GetComponent<Particle>();
        p.trailRenderer.Clear();
        p.expandedTime = 0f;
        p.lifeTime = Random.Range(minLifetime, maxLifetime);
        p.previousFinger = finger;

        Color color = Color.HSVToRGB(hsv, 1f, 1f);
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = color;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = Color.black;
        colorKeys[1].time = 1f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;

        gradient.SetKeys(colorKeys, alphaKeys);

        p.trailRenderer.colorGradient = gradient;
        p.spriteRenderer.color = color;

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