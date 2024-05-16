using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        Gizmos.DrawLine(transform.position + transform.right, transform.position + transform.right * 0.9f + transform.up * 0.05f);
        Gizmos.DrawLine(transform.position + transform.right, transform.position + transform.right * 0.9f - transform.up * 0.05f);
    }

}
