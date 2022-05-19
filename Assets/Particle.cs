using UnityEngine;

public struct Particle
{
    public int id;
    public GameObject gameObject;
    public SpriteRenderer spriteRenderer;
    public float lifeTime;
    public float expandedTime;
    public int previousFinger;

    public Vector3 velocity;
    public float drag;

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }
}