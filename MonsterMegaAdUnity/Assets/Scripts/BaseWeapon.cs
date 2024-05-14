using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    abstract public void Attack(Vector2 attackDestination);
    abstract public void AltAttack(Vector2 attackDestination);

}
