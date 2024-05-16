using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSelfDeleter : MonoBehaviour
{
    public float m_delay;
    void Start()
    {
        StartCoroutine(ImminentDeath(m_delay));
    }

    IEnumerator ImminentDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(this.gameObject);

        yield return null;
    }

}
