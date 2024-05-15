using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BaseWeapon m_currentWeapon;

    public void GiveNewWeapon(GameObject weaponPrefab)
    {
        var weaponObj = Instantiate(weaponPrefab, this.transform);
        if (weaponObj == null )
        {
            Debug.LogException(new System.Exception("Couldn't instantiate weapon prefab"));
            return;
        }

        var weapon = GetComponent<BaseWeapon>();
        if (weapon == null)
        {
            Debug.LogException(new System.Exception("Couldn't find type of <BaseWeapon> on instantiated prefab"));
            Destroy(weaponObj);
            return;
        }

        if (m_currentWeapon != null)
        {
            Destroy(m_currentWeapon);
        }

        m_currentWeapon = weapon;
    }

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
