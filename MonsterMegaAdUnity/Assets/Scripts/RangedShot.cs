using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedShot : MonoBehaviour
{
    public Projectile[] m_projectiles;

    private void Update()
    {
        bool allNull = true;
        for (int i = 0; i < m_projectiles.Length; i++)
        {
            if (m_projectiles[i] != null) allNull = false;
        }
        if (allNull)
            Destroy(gameObject);
    }
}


