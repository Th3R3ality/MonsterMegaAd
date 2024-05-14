using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    abstract public void Attack();
    virtual public void AltAttack()
    {

    }
}
