using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int m_health;
    public SpriteRenderer[] srs;


    void Update()
    {
        // death cause
        if (m_health <= 0)
        {
            print("player dead"); //delete later
            SceneManager.LoadScene("level0");
            
        }

        UpdateSpriteVisibility();
    }



    void UpdateSpriteVisibility()
    {
        // Calculate how many sprites should be enabled
        int activeSprites = Mathf.FloorToInt(m_health / (100f / 3f));


        for (int i = 0; i < srs.Length; i++)
        {
            if (i < activeSprites)
            {
                srs[i].enabled = true; 
            }
            else
            {
                srs[i].enabled = false; 
            }
        }
    }
}
