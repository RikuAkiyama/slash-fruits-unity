using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// フェーズ情報を保持するクラス
[System.Serializable]
public class Phase
{
    public float phasePercentage; // このフェーズが何秒続くか
    public float spawnInterval; // このフェーズでのフルーツの生成間隔
}

public class FruitsGenerator : MonoBehaviour
{
    // フルーツのPrefabを保持する配列
    public GameObject[] fruitPrefabs;
    // ゲームの合計時間
    private float totalTime;
    // 各フェーズの情報を格納する配列
    public Phase[] phases;

    private float timePassed = 0f;
    private float nextSpawnTime = 0f;

    private void Start()
    {
        // ゲームの合計時間を取得
        totalTime = GameObject.Find("GameDirector").GetComponent<GameDirector>().time;

        float totalPercentage = 0f;
        foreach (var phase in phases)
        {
            totalPercentage += phase.phasePercentage;
        }

        // パーセンテージの合計を1に正規化
        if (totalPercentage != 1f)
        {
            foreach (var phase in phases)
            {
                phase.phasePercentage = phase.phasePercentage / totalPercentage;
            }
        }

        // 最初のフルーツを生成
        GenerateRandomFruit();
    }

    private void Update()
    {
        // 経過時間を更新
        timePassed += Time.deltaTime;

        // フルーツの生成時間がきたら、フルーツを生成し、次の生成時間を更新
        if(timePassed >= nextSpawnTime)
        {
            GenerateRandomFruit();
            nextSpawnTime += GetCurrentSpawnInterval();
        }
    }

    float GetCurrentSpawnInterval()
    {
        float phaseStartTime = 0;
        foreach(var phase in phases)
        {
            // もし現在が該当するフェーズ内なら、そのフェーズの生成間隔を返す
            float phaseDuration = phase.phasePercentage * totalTime;
            if(timePassed >= phaseStartTime && timePassed < phaseStartTime + phaseDuration)
            {
                return phase.spawnInterval;
            }
            // 該当しないなら、次のフェーズへ進む
            phaseStartTime += phaseDuration;
        }

        // すべてのフェーズを経過した場合、最後のフェーズの生成間隔を返す
        return phases[phases.Length - 1].spawnInterval;
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
