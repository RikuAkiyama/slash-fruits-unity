using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldFruitController : MonoBehaviour
{
    public float minSpeed = 5f;   // 最低速度
    public float maxSpeed = 20f;  // 最大速度
    public Vector2 targetAreaX = new Vector2(-25, 25); // 目標地点のx座標範囲
    public Vector2 targetAreaY = new Vector2(10, 30);  // 目標地点のy座標範囲
    private float speed;          // 実際の速度
    private Vector3 target;       // 目標地点
    private bool passedTarget = false; // 目標地点を通過したかどうか
    public GameObject cutFruitPrefab; // 分割されたフルーツのプレハブ

    void Start()
    {
        // フルーツの初期位置を設定（画面の外周と、その外周+5の領域内のランダムな点）
        float initialX = Random.Range(-55, 55);
        float initialY = initialX >= -50 && initialX <= 50 ? (Random.value > 0.5f ? -5 : 45) : Random.Range(0, 45);
        float initialZ = Random.Range(15, 30);
        transform.position = new Vector3(initialX, initialY, initialZ);

        // 目標地点を画面中央部分の一定の長方形領域内のランダムな一点に設定
        float targetX = Random.Range(targetAreaX.x, targetAreaX.y);
        float targetY = Random.Range(targetAreaY.x, targetAreaY.y);
        target = new Vector3(targetX, targetY, initialZ);

        // フルーツの速度をランダムに設定
        speed = Random.Range(minSpeed, maxSpeed);
    }
    
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if ((transform.position - target).magnitude < step) 
        {
            passedTarget = true;
            // Update the target to a point further in the same direction
            target = transform.position + (target - transform.position).normalized * 100;
        }

        if (passedTarget && !IsInsideScreen())
        {
            Destroy(gameObject);
        }
    }

    bool IsInsideScreen()
    {
        Vector3 pos = transform.position;
        return pos.x >= -50 && pos.x <= 50 && pos.y >= 0 && pos.y <= 40;
    }

    void OnTriggerEnter(Collider other)
    {
        // ナイフとの衝突を検出
        if (other.gameObject.CompareTag("Knife"))
        {
            GameObject.Find("SpecialHitSE").GetComponent<AudioPlayController>().Play();

            // 分割されたフルーツのインスタンスを生成
            GameObject cutFruit1 = Instantiate(cutFruitPrefab, transform.position, transform.rotation);
            GameObject cutFruit2 = Instantiate(cutFruitPrefab, transform.position, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0))); // 180度回転

            // Rigidbodyを取得し、力を加える
            Rigidbody rb1 = cutFruit1.GetComponent<Rigidbody>();
            Rigidbody rb2 = cutFruit2.GetComponent<Rigidbody>();

            // オブジェクトが飛び散る方向を定義（この例では水平方向に飛び散る）
            Vector3 forceDirection = transform.right;

            // 力を適用
            rb1.AddForce(forceDirection * 1, ForceMode.Impulse);
            rb2.AddForce(-forceDirection * 1, ForceMode.Impulse);

            // 元のフルーツを破棄
            Destroy(gameObject);
        }
    }
}
