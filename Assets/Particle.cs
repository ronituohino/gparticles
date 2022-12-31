using UnityEngine;

public class Particle
{
    public int id;
    public GameObject gameObject;
    public SpriteRenderer spriteRenderer;
    public TrailRenderer trailRenderer;
    public float lifeTime;
    public float expandedTime;
    public int previousFinger;
    public Vector3 velocity;
    public float maxVelocity;

    public void AddVelocity(Vector3 vel) 
    {
        Vector3 newVelocity = velocity + vel;
        if(newVelocity.magnitude > maxVelocity) 
        {
            newVelocity = newVelocity.normalized * maxVelocity;
        }

        velocity = newVelocity;
    }
}