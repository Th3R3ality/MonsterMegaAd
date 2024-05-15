using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_health;



    public void Update()
    {
        if (m_health < 0) print("player dead"); Application.Quit();
    }
}
