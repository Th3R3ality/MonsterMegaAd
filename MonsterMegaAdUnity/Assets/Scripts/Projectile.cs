using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public AudioSource player;
    public AudioClip ground, enemy;

    private void Awake()
    {
        player = gameObject.AddComponent<AudioSource>();
        ground = (AudioClip)Resources.Load("Audio/ground");
        enemy = (AudioClip)Resources.Load("Audio/enemy");
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
        if(collision.gameObject.CompareTag("Enemy"))
        {
            player.PlayOneShot(enemy);
            Destroy(this.gameObject);
        }
        
        if (collision.gameObject.layer == 1 << LayerMask.NameToLayer("Ground"))
        {
            player.PlayOneShot(ground);
            Destroy(this.gameObject);
        }
    }
}
