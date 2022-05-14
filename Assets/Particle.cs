using UnityEngine;

public class Particle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public TrailRenderer trailRenderer;
    public float lifeTime;
    public float expandedTime;
    public int previousFinger;
}