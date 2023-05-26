using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeGenerator : MonoBehaviour
{
    public GameObject knifePrefab;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameObject knife = Instantiate(knifePrefab);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldDir = ray.direction;
            knife.GetComponent<KnifeController>().Shoot(worldDir.normalized * 3000);
        }
    }
}
