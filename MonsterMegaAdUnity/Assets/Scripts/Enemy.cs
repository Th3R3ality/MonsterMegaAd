using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("42D ENEMY")]
    [HideInInspector] public StateMachine m_stateMachine;
    [HideInInspector] public static Enemy Instance;
    [Header("Enemy Stats")]
    public float m_damage;
    public float m_health;
    public float m_speed;
    [Header("Enemy Components(all comps besides animator are added at runtime")]
    public Rigidbody2D m_rigidbody;
    public Animator m_animator;
    public BoxCollider2D m_collider;
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

    private void Awake()
    {
        if (Instance != this) Instance = this;
        m_stateMachine = this.gameObject.AddComponent<StateMachine>();
        if (!m_rigidbody) m_rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
        if (!m_animator) m_animator = this.gameObject.GetComponent<Animator>();
        if (!m_collider) m_collider = this.gameObject.AddComponent<BoxCollider2D>();
        if (!m_rigidbody || !m_animator || !m_stateMachine) Debug.LogError("42D ENEMY - something broke during setup (｡◕‿◕｡)" + this.gameObject);
        m_stateMachine.m_smoothTime = m_stateChangeDelay;
        m_stateMachine.m_owner = this.gameObject;
        m_stateMachine.m_target = null;
        m_stateMachine.m_maxChaseDistance = m_maxChaseDistance;
        m_stateMachine.testing = TESTING;
        m_stateMachine.m_attackRange= m_attackRange;
        m_stateMachine.m_attackBoxSize = m_attackBoxRange;
        m_stateMachine.m_attackBoxSize = m_attackBoxRange;
    }

    void Start()
    {
        m_stateMachine.ChangeState(new IdleState());
    }

    void Update()
    {

    }

    private void OnDrawGizmos()
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

}
