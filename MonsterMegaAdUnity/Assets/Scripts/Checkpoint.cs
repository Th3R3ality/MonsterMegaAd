using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public AudioSource player;
    public ParticleSystem particles;
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            print("You won!");
            player.Play();
            particles.Play();
        }
    }

}
