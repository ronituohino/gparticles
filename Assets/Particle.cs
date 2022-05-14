using UnityEngine;

public class Particle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SimpleRigidbody rb;
    public TrailRenderer trailRenderer;
    public float lifeTime;
    public float expandedTime;
    public int previousFinger;
}