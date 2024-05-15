using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : BaseWeapon
{
    [SerializeField] GameObject m_shotPrefab;
    public GameObject m_fireport;
    public float m_projectileVelocity = 5.0f;
    Camera m_cameraMain;
    Vector3 m_initialScale;
    bool m_flipped;
    private void Start()
    {
        m_initialScale = transform.localScale;
        m_cameraMain = Camera.main;
        if (m_shotPrefab == null)
        {
            Debug.LogException(new Exception("Rifle shot prefab not set"));
        }
    }

    private void Update()
    {
        LookAtCursor();
    }

    // rotate and flip gameobject to aim at the cursor
    public void LookAtCursor()
    {
        m_flipped = false;
        var rot = transform.eulerAngles;
        var pos = transform.position;
        var cursorPos = m_cameraMain.ScreenToWorldPoint(Input.mousePosition);

        var cursorDelta = cursorPos - pos;
        var scale = m_initialScale;
        if (cursorDelta.x < 0f)
        {
            scale.x *= -1;
            cursorDelta.y *= -1;
            m_flipped = true;
        }
        transform.localScale = scale;
        float rad = Mathf.Atan2(Mathf.Abs(cursorDelta.x), cursorDelta.y);
        rot.z = -rad * Mathf.Rad2Deg + 90;

        transform.eulerAngles = rot;
    }

    public override void Attack()
    {
        var shotObject = Instantiate(m_shotPrefab, m_fireport.transform.position, m_fireport.transform.rotation);

        var shot = shotObject.GetComponent<RangedShot>();

        if (shot == null)
        {
            Debug.LogException(new Exception("shot object does not contain a <ProjectileShot>"));
            return;
        }

        foreach (Projectile projectileObject in shot.m_projectiles)
        {
            var rb = projectileObject.rb;
            if (rb == null)
            {
                Debug.LogException(new Exception("projectile object does not contain a <Rigidbody2D>"));
                return;
            }

            rb.AddForce(projectileObject.transform.right * m_projectileVelocity * (m_flipped ? -1 : 1), ForceMode2D.Impulse);
        }
    }
}
