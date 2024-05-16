using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_moveSpeed = 100.0f;
    public float m_jumpForce = 10.0f;
    public bool m_grounded = false;
    public AudioSource m_audioSource;
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] BoxCollider2D m_feetCollider;
    Rigidbody2D m_rigidBody;

    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        m_audioSource.clip = (AudioClip)Resources.Load("Audio/jump");
    }
    void Start()
    {
        if (m_collider == null)
            Debug.LogException(new Exception("player collider not assigned"));

        if (m_feetCollider == null)
            Debug.LogException(new Exception("player feet collider not assigned"));

        m_rigidBody = m_collider.attachedRigidbody;

        if (m_rigidBody == null)
            Debug.LogException(new Exception("player collider doesnt have an attached rigidbody"));
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();

        DoMovement();
    }
    void CheckGrounded()
    {
        m_grounded = Physics2D.OverlapBox(m_feetCollider.transform.position, m_feetCollider.size, 0, 1 << LayerMask.NameToLayer("Ground"));
    }
    void DoMovement()
    {
        Vector2 force = Vector2.zero;

        float hor = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");

        force.x = hor * m_moveSpeed;
        force.y = m_rigidBody.velocityY;

        if (m_grounded && jump > 0.0f)
        {
            force.y = jump * m_jumpForce;
            m_audioSource.Play();
        }

        m_rigidBody.velocity = force;
        //m_rigidBody.AddForce(force, ForceMode2D.Impulse);
    }
}
