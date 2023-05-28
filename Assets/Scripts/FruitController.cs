using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    // フルーツの最小速度と最大速度
    public float minSpeed = 5f;  
    public float maxSpeed = 20f;  

    // フルーツが向かう目標のx座標とy座標の範囲
    public Vector2 targetAreaX = new Vector2(-25, 25); 
    public Vector2 targetAreaY = new Vector2(10, 30); 

    // フルーツが飛ぶ実際の速度
    private float speed;
    // フルーツが向かう目標地点
    private Vector3 target;
    // 目標地点を通過したかどうかを表すフラグ
    private bool passedTarget = false;
    // 分割されたフルーツのPrefab
    public GameObject cutFruitPrefab;

    void Start()
    {
        // フルーツの初期位置を設定
        float initialX = Random.Range(-55, 55);
        float initialY = initialX >= -50 && initialX <= 50 ? (Random.value > 0.5f ? -5 : 45) : Random.Range(0, 45);
        float initialZ = Random.Range(15, 30);
        transform.position = new Vector3(initialX, initialY, initialZ);

        // フルーツの目標地点を設定
        float targetX = Random.Range(targetAreaX.x, targetAreaX.y);
        float targetY = Random.Range(targetAreaY.x, targetAreaY.y);
        target = new Vector3(targetX, targetY, initialZ);

        // フルーツの速度を設定
        speed = Random.Range(minSpeed, maxSpeed);
    }
    
    void Update()
    {
        // フルーツが目標地点に向かう
        Vector3 direction = (target - transform.position).normalized;
        float step = speed * Time.deltaTime;
        transform.position += direction * step;

        // 目標地点を通過したら新しい目標地点を設定
        if ((transform.position - target).magnitude < step && !passedTarget) 
        {
            passedTarget = true;
            target = transform.position + direction * 100;
        }

        // 目標地点を通過し、フルーツが画面から出た場合は削除
        if (passedTarget && !IsInsideScreen())
        {
            Destroy(gameObject);
        }
    }

    // フルーツが画面内にいるか確認
    bool IsInsideScreen()
    {
        Vector3 pos = transform.position;
        return pos.x >= -50 && pos.x <= 50 && pos.y >= 0 && pos.y <= 40;
    }

    void OnTriggerEnter(Collider other)
    {
        // フルーツがナイフと衝突した場合の処理
        if (other.gameObject.CompareTag("Knife"))
        {
            // ヒットSEを再生
            if(this.tag == "GoldApple")
            {
                GameObject.Find("SpecialHitSE").GetComponent<AudioPlayController>().Play();
            }
            else
            {
                GameObject.Find("HitSE").GetComponent<AudioPlayController>().Play();
            }

            // 分割されたフルーツを生成
            GameObject cutFruit1 = Instantiate(cutFruitPrefab, transform.position, transform.rotation);
            GameObject cutFruit2 = Instantiate(cutFruitPrefab, transform.position, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0)));

            // 分割されたフルーツに力を加えて飛ばす
            Rigidbody rb1 = cutFruit1.GetComponent<Rigidbody>();
            Rigidbody rb2 = cutFruit2.GetComponent<Rigidbody>();
            Vector3 forceDirection = transform.right;  // 飛ばす方向
            rb1.AddForce(forceDirection * 1, ForceMode.Impulse);
            rb2.AddForce(-forceDirection * 1, ForceMode.Impulse);

            // 元のフルーツを削除
            Destroy(gameObject);
        }
    }
}
