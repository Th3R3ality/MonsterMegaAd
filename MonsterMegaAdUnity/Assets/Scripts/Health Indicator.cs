using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    [Header("Health Indicator Settings")]
    public List<SpriteRenderer> m_healthIndicators;
    public GameObject m_healthIndicatorPrefab;
    public Transform m_healthIndicatorContainer;
    public float m_maxIndicatorsPerRow = 10.0f;

    private void Awake()
    {
        if (!m_healthIndicatorPrefab)
            Debug.LogException(new Exception("<healthIndicatorPrefab> not assigned"));
    }

    public void UpdateIndicators(int health, Color color)
    {
        while (m_healthIndicators.Count != health)
        {
            if (m_healthIndicators.Count > health)
            { // destroy an indicator
                var indicator = m_healthIndicators[m_healthIndicators.Count - 1];
                m_healthIndicators.Remove(indicator);
                Destroy(indicator.gameObject);
            }
            else if (m_healthIndicators.Count < health)
            { // add an indicator

                Transform parent = m_healthIndicatorContainer != null ? m_healthIndicatorContainer : this.transform;
                GameObject indicatorObject = Instantiate(m_healthIndicatorPrefab, parent);

                var spriteRenderer = indicatorObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                    Debug.LogException(new Exception("indicator prefab does not contain <SpriteRenderer>"));

                spriteRenderer.color = color;

                m_healthIndicators.Add(spriteRenderer);
            }

            if (m_healthIndicators.Count <= 0) return;

            // align indicators (Evil dark shadow magic mathematics)
            float rowCount = m_maxIndicatorsPerRow * Mathf.Floor(m_healthIndicators.Count / m_maxIndicatorsPerRow);

            for (float i = 0.0f; i < m_healthIndicators.Count; i += 1.0f)
            {
                Vector3 pos = Vector3.zero;

                pos.y = 0.1f * Mathf.Floor(i / m_maxIndicatorsPerRow);

                float evenCountAlignment = 0;
                if (m_maxIndicatorsPerRow % 2 == 0)
                {
                    if (i >= rowCount)
                    {
                        evenCountAlignment = m_healthIndicators.Count % 2 * 0.02f;
                    }
                    else
                    {
                        evenCountAlignment = 0.02f;
                    }
                }
                else
                {
                    if (i >= rowCount)
                    {
                        evenCountAlignment = (1 - (m_healthIndicators.Count % 2)) * 0.02f;
                    }
                }

                float centerRowAlignment = 0f;
                if (m_maxIndicatorsPerRow % 2 == 0)
                {
                    if (i < rowCount)
                    {
                        centerRowAlignment = -(m_maxIndicatorsPerRow / 2 * 0.04f);
                    }
                    else
                    {
                        centerRowAlignment = -(((m_healthIndicators.Count % m_maxIndicatorsPerRow) - (1 - (m_healthIndicators.Count % 2))) / 2 * 0.04f);
                    }
                }
                else
                {
                    if (i < rowCount)
                    {
                        centerRowAlignment = -(m_maxIndicatorsPerRow / 2 * 0.04f) + 0.02f;
                    }
                    else
                    {
                        centerRowAlignment = -(((m_healthIndicators.Count % m_maxIndicatorsPerRow) - ((m_healthIndicators.Count % 2))) / 2 * 0.04f);
                    }
                }

                pos.x = i % m_maxIndicatorsPerRow * 0.04f + centerRowAlignment + evenCountAlignment;
                m_healthIndicators[(int)i].transform.localPosition = pos;
            }
        }
    }
}
