using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillFloor : MonoBehaviour
{
    public static KillFloor Instance;

    private void Awake()
    {
        if(Instance!=this) Instance = this;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject) SceneManager.LoadScene("level0");
    }

}
