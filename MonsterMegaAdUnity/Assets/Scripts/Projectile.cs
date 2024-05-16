using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public AudioClip shot, ground, enemy;

    private void Awake()
    {
        shot = (AudioClip)Resources.Load("Audio/shot");
        ground = (AudioClip)Resources.Load("Audio/ground");
        enemy = (AudioClip)Resources.Load("Audio/enemy");
    }
    private void Start()
    {

        AudioAfterDestroy.PlayAudio(shot, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        Gizmos.DrawLine(transform.position + transform.right, transform.position + transform.right * 0.9f + transform.up * 0.05f);
        Gizmos.DrawLine(transform.position + transform.right, transform.position + transform.right * 0.9f - transform.up * 0.05f);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            AudioAfterDestroy.PlayAudio(enemy, transform.position);
            Destroy(this.gameObject);
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Kill Floor"))
        {
            AudioAfterDestroy.PlayAudio(ground, transform.position);
            Destroy(this.gameObject);
        }
    }
}
