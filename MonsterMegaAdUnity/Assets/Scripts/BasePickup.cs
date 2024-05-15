using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePickup : MonoBehaviour
{
    [SerializeField] string m_pickupeeTag = string.Empty;
    public abstract void DoPickup(Collider2D other);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == m_pickupeeTag)
        {
            DoPickup(other);
        }
    }
}
