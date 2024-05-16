using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int m_health;
    public HealthIndicator m_healthIndicator;
    public SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        if (m_healthIndicator == null)
            Debug.LogException(new System.Exception("<HealthIndicator> not assigned"));
        if (m_spriteRenderer == null)
            Debug.LogException(new System.Exception("<SpriteRenderer> not assigned"));

        UpdateHealthIndicator();
    }

    void Update()
    {
        // death cause
        if (m_health <= 0)
        {
            print("player dead"); //delete later
            SceneManager.LoadScene("level0");
            
        }

        
    }
    public void UpdateHealthIndicator()
    {
        m_healthIndicator.UpdateIndicators(m_health, m_spriteRenderer.color);
    }
}
