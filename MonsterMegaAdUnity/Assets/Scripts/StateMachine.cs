using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;


public interface IState
{
    void Enter(GameObject owner);
    void Execute(GameObject owner);
    void Exit(GameObject owner);
}

public class StateMachine : MonoBehaviour
{
    [SerializeField] IState m_currentState;
    IState m_nextState;
    public GameObject m_owner, m_target;
    public float m_smoothTime;
    private float m_stateChangeTimer;
    public float m_maxChaseDistance;
    private bool isChangingState = false;
    public bool testing;

    public void ChangeState(IState newState)
    {
        if (isChangingState)
            return;

        isChangingState = true;
        m_nextState = newState;
        m_stateChangeTimer = m_smoothTime;
    }

    private void PerformStateChange()
    {
        if (m_currentState != null)
            m_currentState.Exit(m_owner);

        m_currentState = m_nextState;
        m_currentState.Enter(m_owner);

        isChangingState = false;
    }

    void Update()
    {
        if (isChangingState)
        {
            m_stateChangeTimer -= Time.deltaTime;
            if (m_stateChangeTimer <= 0)
            {
                PerformStateChange();
            }
        }

        if (m_currentState != null && !isChangingState)
        {
            m_currentState.Execute(m_owner);
        }
    }

    void FixedUpdate()
    {
        if (m_currentState != null && !isChangingState)
        {
            m_currentState.Execute(m_owner);
        }
    }
}

public class IdleState : IState
{
    Animator m_animator;
    Rigidbody2D m_rigidbody;
    Vector3 m_initialPosition;
    Vector3 m_targetPosition;
    bool movingToInitialPosition;

    public void Enter(GameObject owner)
    {
        m_animator = owner.GetComponent<Animator>();
        m_rigidbody = owner.GetComponent<Rigidbody2D>();
        if (!m_animator) Debug.LogError("No Animator Found on gameObject" + owner.name);
        if (!m_rigidbody) Debug.LogError("No Rigidbody2D Found on gameObject" + owner.name);

        m_initialPosition = owner.transform.position;
        m_targetPosition = m_initialPosition + new Vector3(owner.GetComponent<Enemy>().m_pathDistance, 0, 0);
        movingToInitialPosition = false;
    }

    public void Execute(GameObject owner)
    {
        Move(owner);
    }

    void Move(GameObject owner)
    {
        if (owner.GetComponent<StateMachine>().testing) Debug.Log("42D ENEMY - Running on Idle");

        float speed = owner.GetComponent<Enemy>().m_speed;
        float radius = owner.GetComponent<Enemy>().m_playerDetectionRadius;

        Collider2D hitCollider = Physics2D.OverlapBox(owner.transform.position, new Vector2(radius, radius), 0, LayerMask.GetMask("Entity"));
        if (hitCollider && hitCollider.CompareTag("Player"))
        {
            owner.GetComponent<StateMachine>().ChangeState(new ChaseState(hitCollider.gameObject));
            return;
        }

        Vector3 target = movingToInitialPosition ? m_initialPosition : m_targetPosition;
        Vector3 direction = (target - owner.transform.position).normalized;
        m_rigidbody.velocity = direction * speed;

        if (Vector3.Distance(owner.transform.position, target) < 0.1f)
        {
            movingToInitialPosition = !movingToInitialPosition;
        }
    }

    public void Exit(GameObject owner)
    {
        m_rigidbody.velocity = Vector3.zero;
    }
}


public class ChaseState : IState
{
    GameObject m_target;
    Animator m_animator;
    public ChaseState(GameObject tar)
    {
        m_target = tar;
    }
    
  

    public void Enter(GameObject owner)
    {
        m_animator = owner.GetComponent<Animator>();
        if (!m_animator) Debug.LogError("No Animator Found on gameObject" + owner.name);



    }

    public void Execute(GameObject owner)
    {
        Move(owner);
    }


    public void Exit(GameObject owner)
    {

    }


    void Move(GameObject owner)
    {
        Rigidbody2D rigidbody = owner.GetComponent<Rigidbody2D>();
        float speed = owner.GetComponent<Enemy>().m_speed;
        StateMachine stateMachine = owner.GetComponent<StateMachine>();
        if (!rigidbody) Debug.LogError("42D ENEMY - No rigidbody" +  m_target.name);
        else
        {
            if (stateMachine.testing) Debug.Log("42D ENEMY - Running on Idle");
            if (Vector2.Distance(owner.transform.position, m_target.transform.position) < (stateMachine.m_maxChaseDistance)) stateMachine.ChangeState(new IdleState());
            else
            {
                rigidbody.velocity = m_target.transform.position - owner.transform.position.normalized * Time.deltaTime * speed;
            }

            
        }
    }
}