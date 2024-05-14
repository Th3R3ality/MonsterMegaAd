using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BaseWeapon m_currentWeapon;
    void Update()
    {
        
        if (m_currentWeapon != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_currentWeapon.Attack();
            }

            if (Input.GetMouseButtonDown(1))
            {
                m_currentWeapon.AltAttack();
            }
        }
    }
}
