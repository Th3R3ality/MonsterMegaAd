using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : BasePickup
{
    [SerializeField] GameObject m_weaponPrefab;
    public override void DoPickup(Collider2D other)
    {
        var playerCombat = other.transform.GetComponent<PlayerCombat>();

        if (playerCombat == null)
        {
            Debug.LogException(new System.Exception("Couldn't find type <PlayerCombat>"));
            return;
        }

        playerCombat.GiveNewWeapon(m_weaponPrefab);

        Destroy(this.gameObject);
    }
}
