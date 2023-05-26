using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using RestSharp;
using RestSharp.Authenticators;

public class ResultManager : MonoBehaviour
{
    public Button retryButton;
    public TextMeshProUGUI result;
    public TextMeshProUGUI userRank;
    public TextMeshProUGUI highScoreUser;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI yourRank;
    public TextMeshProUGUI yourHighScoreUser;
    public TextMeshProUGUI yourHighScore;
    public TextMeshProUGUI messageText;

    public class responseToken
    {
        public string token { get; set; }
    }

    public class responseMessage
    {
        public string message { get; set; }
    }

    public class responseRanking
    {
        public int rank { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public int score_id { get; set; }
        public int score { get; set; }
    }

    private void Start()
    {
        GameObject.Find("ResultBGM").GetComponent<AudioPlayController>().Play();
        result.text = GameManager.instance.Username + "'S SCORE: " + GameManager.instance.Score + " P";
        retryButton.onClick.AddListener((UnityEngine.Events.UnityAction) (() => ReplayTokenRefresh("/tokenRefresh")));
        GetRanking("/ranking");
        GetRanking("/userRanking");
    }

    private async void ReplayTokenRefresh(string endpoint)
    {
        // トークン再取得リクエストを作成
        var client = new RestClient(GameManager.instance.Api);
        var authenticator = new JwtAuthenticator(GameManager.instance.Token);
        var request = new RestRequest(endpoint)
        {
            Authenticator = authenticator
        };

        // レスポンスを取得
        var response = await client.ExecuteGetAsync(request);

        // 認証が成功したとき、トークンを上書きしてゲーム画面に遷移
        if(response.StatusCode == HttpStatusCode.OK)
        {
            var responseToken = JsonSerializer.Deserialize<responseToken>(response.Content);
            GameManager.instance.Token = responseToken.token;
            Debug.Log("認証成功: " + GameManager.instance.Token);
            SceneManager.LoadScene("WaitScene");
        }
        // 認証が失敗したとき、タイトル画面に遷移
        else
        {
            var responseMessage = JsonSerializer.Deserialize<responseMessage>(response.Content);
            Debug.Log("エラー: " + responseMessage.message);
            SceneManager.LoadScene("TitleScene");
        }
    }

    private async void GetRanking(string endpoint)
    {
        // ランキング取得リクエストを作成
        var client = new RestClient(GameManager.instance.Api);
        var authenticator = new JwtAuthenticator(GameManager.instance.Token);
        var request = new RestRequest(endpoint)
        {
           Authenticator = authenticator
        };

        // レスポンスを取得
        var response = await client.ExecuteGetAsync(request);

        // 取得が成功したとき、ランキングの表示関数を呼び出す
        if(response.StatusCode == HttpStatusCode.OK)
        {
            responseRanking[] rankingArray = JsonSerializer.Deserialize<responseRanking[]>(response.Content);
            Debug.Log("ランキング取得成功: " + endpoint);
            ShowRanking(rankingArray, endpoint);
        }
        // 取得が失敗したとき、エラーを表示
        else
        {
            var responseMessage = JsonSerializer.Deserialize<responseMessage>(response.Content);
            Debug.Log("エラー: " + responseMessage.message);
            messageText.text = "ERROR!: Failed to access ranking data";
        }
    }

    private void ShowRanking(responseRanking[] rankingArray, string endpoint)
    {
        string showRank = "";
        string showUsername = "";
        string showScore = "";
        foreach (responseRanking ranking in rankingArray)
        {
            showRank += ranking.rank + "\n";
            showUsername += ranking.username + "\n";
            showScore += ranking.score + "\n";
        }
        if(endpoint == "/ranking")
        {
            userRank.text = showRank;
            highScoreUser.text = showUsername;
            highScore.text = showScore;
        }
        if(endpoint == "/userRanking")
        {
            yourRank.text = showRank;
            yourHighScoreUser.text = showUsername;
            yourHighScore.text = showScore;
        }

    }
}
