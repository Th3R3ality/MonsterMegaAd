using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("42D ENEMY")]
    [HideInInspector] public StateMachine m_stateMachine;

    [Header("Enemy Stats")]
    public int m_damage;
    public int m_health;
    public float m_speed;

    [Header("Enemy Components")]
    public Rigidbody2D m_rigidbody;
    public Animator m_animator;
    public BoxCollider2D m_collider;
    public AudioSource m_audioSource;
    public SpriteRenderer m_spriteRenderer;

    [Header("Health Indicators")]
    public List<SpriteRenderer> m_healthIndicators;
    public GameObject m_healthIndicatorPrefab;
    public Transform m_healthIndicatorContainer;
    public float m_maxIndicatorsPerRow = 10.0f;

    [Header("Enemy Settings")]
    [Range(0.0f, 10.0f)] public float m_stateChangeDelay;
    [Range(0.0f, 10.0f)] public float m_playerDetectionRadius;
    [Range(0.0f, 25.0f)] public float m_pathDistance;
    [Range(0.0f, 10.0f)] public float m_maxChaseDistance;
    [Range(0.0f, 10.0f)][Tooltip("The Size of the BoxCast in front of the enemy")] public float m_attackBoxRange;
    [Range(0.0f, 10.0f)][Tooltip("The AI will attack if the player is in this range")] public float m_attackRange;
    
    [Header("StateMachine Settings")]
    public string m_currentState;
    public bool TESTING;
    private AudioClip deathSound;

    private void Awake()
    {
        m_stateMachine = this.gameObject.AddComponent<StateMachine>();

        if (!m_healthIndicatorContainer)
            Debug.LogException(new Exception("<healthIndicatorContain> not assigned"));
        if (!m_healthIndicatorPrefab)
            Debug.LogException(new Exception("<healthIndicatorPrefab> not assigned"));
        if (!m_stateMachine)
            Debug.LogException(new Exception("<stateMachine> not assigned"));
        if (!m_rigidbody)
            Debug.LogException(new Exception("<rigidbody> not assigned"));
        if (!m_animator)
            Debug.LogException(new Exception("<animator> not assigned"));
        if (!m_collider)
            Debug.LogException(new Exception("<collider> not assigned"));
        //if (!m_audioSource)
        //    Debug.LogException(new Exception("<audioSource> not assigned"));
        if (!m_spriteRenderer)
            Debug.LogException(new Exception("<spriteRenderer> not assigned"));


        m_stateMachine.m_smoothTime = m_stateChangeDelay;
        m_stateMachine.m_owner = this.gameObject;
        m_stateMachine.m_target = null;
        m_stateMachine.m_maxChaseDistance = m_maxChaseDistance;
        m_stateMachine.testing = TESTING;
        m_stateMachine.m_attackRange= m_attackRange;
        m_stateMachine.m_attackBoxSize = m_attackBoxRange;
        m_stateMachine.m_attackBoxSize = m_attackBoxRange;
        deathSound = (AudioClip)Resources.Load("Audio/enemy_death");
        
    }

    void Start()
    {
        m_stateMachine.ChangeState(new IdleState());
    }

    void Update()
    {
        UpdateHealthIndicators();
    }


    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 initialPosition = this.transform.position;
            Vector3 targetPosition = initialPosition + new Vector3(m_pathDistance, 0, 0);
            Gizmos.DrawLine(new Vector3(initialPosition.x - m_pathDistance, initialPosition.y, initialPosition.z), new Vector3(initialPosition.x + m_pathDistance, initialPosition.y, initialPosition.z));
            Gizmos.DrawSphere(initialPosition, 0.2f);
            Gizmos.DrawSphere(targetPosition, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, m_playerDetectionRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, m_maxChaseDistance);

            // Add Gizmos for attack box
            Gizmos.color = Color.green;
            Vector3 boxCenter = this.transform.position + transform.right * m_attackBoxRange / 2;
            Gizmos.DrawWireCube(boxCenter, new Vector3(m_attackBoxRange, m_collider.size.y, 1));

            // Add Gizmos for attack range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, m_attackRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Vector3 initialPosition = m_stateMachine.m_owner.transform.position;
            Vector3 targetPosition = initialPosition + new Vector3(m_pathDistance, 0, 0);
            Gizmos.DrawLine(new Vector3(initialPosition.x - m_pathDistance, initialPosition.y, initialPosition.z), new Vector3(initialPosition.x + m_pathDistance, initialPosition.y, initialPosition.z));
            Gizmos.DrawSphere(initialPosition, 0.2f);
            Gizmos.DrawSphere(targetPosition, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_stateMachine.m_owner.transform.position, m_playerDetectionRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(m_stateMachine.m_owner.transform.position, m_maxChaseDistance);

            // Add Gizmos for attack box
            Gizmos.color = Color.green;
            Vector3 boxCenter = m_stateMachine.m_owner.transform.position + m_stateMachine.m_owner.transform.right * m_attackBoxRange / 2;
            Gizmos.DrawWireCube(boxCenter, new Vector3(m_attackBoxRange, m_collider.size.y, 1));

            // Add Gizmos for attack range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(m_stateMachine.m_owner.transform.position, m_attackRange);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject) print("collided");
        if(collision.gameObject.CompareTag("Projectile"))
        {
            if (m_health - 1 <= 0) Destroy(this.gameObject); m_audioSource.PlayOneShot(deathSound);
            {
                m_health -= 1;
                m_stateMachine.ChangeState(new ChaseState(m_stateMachine.m_target,true));
            }
            
            
        }
    }

    void UpdateHealthIndicators()
    {
        if (m_healthIndicators.Count < 0) return;

        while (m_healthIndicators.Count != m_health)
        {
            if (m_healthIndicators.Count > m_health)
            { // destroy an indicator
                var indicator = m_healthIndicators[m_healthIndicators.Count - 1];
                m_healthIndicators.Remove(indicator);
                Destroy(indicator.gameObject);
            }
            else
            { // add an indicator
                GameObject indicatorObject = Instantiate(m_healthIndicatorPrefab, m_healthIndicatorContainer);

                var spriteRenderer = indicatorObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                    Debug.LogException(new Exception("indicator prefab does not contain <SpriteRenderer>"));

                spriteRenderer.color = m_spriteRenderer.color;

                m_healthIndicators.Add(spriteRenderer);
            }

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
