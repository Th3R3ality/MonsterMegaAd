using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public interface IState
{
    void Enter(GameObject owner);
    void Execute(GameObject owner);
    void Exit(GameObject owner);
}

public class StateMachine : MonoBehaviour
{
    [SerializeField] public IState m_currentState;
    IState m_nextState;
    public GameObject m_owner, m_target;
    public float m_smoothTime;
    private float m_stateChangeTimer;
    public float m_maxChaseDistance;
    public float m_attackRange;
    public float m_attackBoxSize;
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

        if (m_currentState != null && !isChangingState && m_owner)
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
    GameObject m_target;
    bool movingToInitialPosition;


    public void Enter(GameObject owner)
    {
        m_animator = owner.GetComponent<Animator>();
        m_rigidbody = owner.GetComponent<Rigidbody2D>();
        if (!m_animator) Debug.LogError("No Animator Found on gameObject" + owner.name);
        if (!m_rigidbody) Debug.LogError("No Rigidbody2D Found on gameObject" + owner.name);

        m_initialPosition = owner.transform.position;
        m_targetPosition = m_initialPosition + new Vector3(owner.GetComponent<Enemy>().m_pathDistance, 0, 0);
        m_target = owner.GetComponent<StateMachine>().m_target;
        if (!m_target) m_target = GameObject.FindWithTag("Player");
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


        if (Vector2.Distance(owner.gameObject.transform.position, m_target.transform.position) <= radius)
        {
            owner.GetComponent<StateMachine>().ChangeState(new ChaseState(m_target, false));
            return;
        }

        Vector3 target = movingToInitialPosition ? m_initialPosition : m_targetPosition;
        Vector3 direction = (target - owner.transform.position).normalized;
        m_rigidbody.velocity = direction * speed;

        if (Vector3.Distance(owner.transform.position, target) <= 0.1f)
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
    bool chaseNoMatterWhat;
    Rigidbody2D rigidbody;
    StateMachine stateMachine;
    float radius, speed, range;
    public ChaseState(GameObject tar, bool flag)
    {
        m_target = tar;
        chaseNoMatterWhat = flag;
    }
    
  

    public void Enter(GameObject owner)
    {
        m_animator = owner.GetComponent<Animator>();
        if (!m_animator) Debug.LogError("No Animator Found on gameObject" + owner.name);
        rigidbody = owner.GetComponent<Rigidbody2D>();
        speed = owner.GetComponent<Enemy>().m_speed;
        radius = owner.GetComponent<Enemy>().m_maxChaseDistance;
        range = owner.GetComponent<Enemy>().m_attackRange;
        stateMachine = owner.GetComponent<StateMachine>();
        m_target = stateMachine.m_target;
        if (!m_target) m_target = GameObject.FindWithTag("Player");


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

        if (!rigidbody)
        {
            Debug.LogError("42D ENEMY - No rigidbody" + m_target.name);
        }
        else
        {
            if (stateMachine.testing) Debug.Log("42D ENEMY - Running on Chase");
            float distanceToTarget = Vector2.Distance(owner.transform.position, m_target.transform.position);
            if (distanceToTarget >= radius && !chaseNoMatterWhat) stateMachine.ChangeState(new IdleState());
            if (distanceToTarget <= range) stateMachine.ChangeState(new AttackState(m_target));
            else
            {
                Vector2 direction = (m_target.transform.position - owner.transform.position).normalized;
                rigidbody.velocity = direction * speed;
            }


        }
    }

}

public class AttackState : IState
{
    public AttackState(GameObject tar)
    {
        m_target = tar;
    }
    Animator m_animator;
    GameObject m_target;

    public void Enter(GameObject owner)
    {
        m_animator = owner.GetComponent<Animator>();
        if (!m_animator) Debug.LogError("No Animator Found on gameObject" + owner.name);



    }

    public void Execute(GameObject owner)
    {
        Attack(owner, m_target);
    }


    public void Exit(GameObject owner)
    {

    }

    void Attack(GameObject owner, GameObject target)
    {
        if (owner.GetComponent<StateMachine>().testing) Debug.Log("42D ENEMY - Running on Attack");
        StateMachine stateMachine = owner.GetComponent<StateMachine>();
        float boxSize = owner.GetComponent<Enemy>().m_attackBoxRange;
        int damage = owner.GetComponent<Enemy>().m_damage;
        if (Vector2.Distance(owner.transform.position, target.transform.position) >= (stateMachine.m_attackRange)) stateMachine.ChangeState(new ChaseState(target, false));
        Collider2D hitCollider = Physics2D.OverlapBox(owner.transform.position, new Vector2(boxSize, boxSize), 0, LayerMask.GetMask("Entity"));
        if (hitCollider && hitCollider.CompareTag("Player"))
        {
            Player player = hitCollider.GetComponent<Player>();
            player.m_health -= damage;
            player.UpdateHealthIndicator();
            owner.GetComponent<StateMachine>().ChangeState(new IdleState());
            Debug.Log("42D ENEMY - Player Hit!");
        }
        else
        {
            Debug.Log("42D ENEMY - Player Hit!");
        }
        
    }
}