using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private Rigidbody rb;
    GameObject director;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        this.director = GameObject.Find("GameDirector");
    }

    public void Shoot(Vector3 dir)
    {
        rb.AddForce(dir);
    }

    void FixedUpdate()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(rb.velocity);
        rb.rotation = Quaternion.Lerp(rb.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    void Update()
    {
        Vector3 position = transform.position;
        if (position.x < -51 || position.x > 51 || position.y < -1 || position.y > 41 || position.z < -51 || position.z > 51)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        string fruitTag = other.gameObject.tag;
        switch (fruitTag)
        {
            case "Apple":
                director.GetComponent<GameDirector>().HitApple();
                break;
            case "Pear":
                director.GetComponent<GameDirector>().HitPear();
                break;
            case "Kiwi":
                director.GetComponent<GameDirector>().HitKiwi();
                break;
            case "Lemon":
                director.GetComponent<GameDirector>().HitLemon();
                break;
            case "Orange":
                director.GetComponent<GameDirector>().HitOrange();
                break;
            case "GoldApple":
                director.GetComponent<GameDirector>().HitGoldApple();
                break;
            default:
                Debug.Log("Unknown fruit tag: " + fruitTag);
                break;
        }
    }
}
