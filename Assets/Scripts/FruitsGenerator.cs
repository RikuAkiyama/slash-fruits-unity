using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsGenerator : MonoBehaviour
{
    // フルーツのPrefabを保持する配列
    public GameObject[] fruitPrefabs;
    // ゲームの合計時間
    private float totalTime;

    private void Start()
    {
        // ゲームの合計時間を取得
        totalTime = GameObject.Find("GameDirector").GetComponent<GameDirector>().time;
    }

    private void Update()
    {
        // TODO:
        // フルーツを生成する間隔をコントロール
    }

    // ランダムなフルーツを生成するメソッド
    void GenerateRandomFruit()
    {
        // フルーツPrefabの配列からランダムなインデックスを選択
        int randomIndex = Random.Range(0, fruitPrefabs.Length);
        // 選択したフルーツPrefabを取得
        GameObject randomFruit = fruitPrefabs[randomIndex];
        // フルーツPrefabをインスタンス化し、シーンに生成
        Instantiate(randomFruit, transform.position, Quaternion.identity);
    }
}
