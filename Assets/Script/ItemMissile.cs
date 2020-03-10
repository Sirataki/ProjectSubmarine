using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMissile : MonoBehaviour
{
    public float speedY;

    private float speedX;

    private Vector3 pos;

    private float angle;
    
    public void RandSet(int rand)
    {
        if (rand == 0)
        {
            pos = new Vector3(-11.0f, 0.0f, -0.5f);
            speedX = 0.03f;
        }
        else
        {
            pos = new Vector3(11.0f, 0.0f, -0.5f);
            speedX = -0.03f;
        }

        transform.position = pos;
    }

    void Start()
    {
        //SE
        soundManager.Instance.PlaySound(6, false);
        angle = 0;
    }

    void Update()
    {
        angle += 0.1f;

        pos.x += speedX;
        pos.y += Mathf.Cos(angle) * speedY;

        transform.position = pos;

        if (transform.position.x > 11.0f || transform.position.x < -11.0f )
        {
            Destroy( gameObject );
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "submarine")
        {
            Destroy(gameObject);
        }
    }
}
