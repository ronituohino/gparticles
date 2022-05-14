using UnityEngine;

public class SimpleRigidbody : MonoBehaviour
{
    public Vector3 velocity;
    public float drag;

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    void FixedUpdate()
    {
        velocity -= velocity * drag;
        transform.position += velocity;
    }
}