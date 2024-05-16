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
    public HealthIndicator m_healthIndicator;

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


        if (!m_stateMachine)
            Debug.LogException(new Exception("<stateMachine> not assigned"));
        if (!m_rigidbody)
            Debug.LogException(new Exception("<rigidbody> not assigned"));
        if (!m_animator)
            Debug.LogException(new Exception("<animator> not assigned"));
        if (!m_collider)
            Debug.LogException(new Exception("<collider> not assigned"));
        if (!m_audioSource)
            Debug.LogException(new Exception("<audioSource> not assigned"));
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
        m_healthIndicator.UpdateIndicators(m_health, m_spriteRenderer.color);
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
        if(collision.gameObject.layer == LayerMask.NameToLayer("SolidProjectile"))
        {
            if (m_health - 1 <= 0) 
            {
                m_audioSource.PlayOneShot(deathSound);
                Destroy(this.gameObject);
                
            }

            m_health -= 1;
            m_stateMachine.ChangeState(new ChaseState(m_stateMachine.m_target,true));
        }
    }

    
}
