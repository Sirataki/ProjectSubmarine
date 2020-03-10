using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private Vector3 direction;

    public float speed;

    void Start()
    {
        GameObject ship = GameObject.Find("ship").gameObject;

        Vector3 shipPos = ship.transform.position;

        direction = shipPos - transform.position;

        direction.Normalize();
    }

    void Update()
    {
        transform.position += direction * speed;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ship")
            Destroy(gameObject);
    }
}
