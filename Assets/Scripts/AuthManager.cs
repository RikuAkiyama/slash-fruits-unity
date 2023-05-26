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

public class AuthManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button registerButton;
    public Button loginButton;
    public TextMeshProUGUI messageText;

    public class responseToken
    {
        public string token { get; set; }
    }

    public class responseMessage
    {
        public string message { get; set; }
    }

    private void Start()
    {
        registerButton.onClick.AddListener((UnityEngine.Events.UnityAction) (() => SendRequest("/register")));
        loginButton.onClick.AddListener((UnityEngine.Events.UnityAction) (() => SendRequest("/login")));
        GameObject.Find("TitleBGM").GetComponent<AudioPlayController>().Play();
    }

    private async void SendRequest(string endpoint)
    {
        if(usernameField.text.Length < 4 || 8 < usernameField.text.Length || passwordField.text.Length < 4 || 8 < passwordField.text.Length)
        {
            messageText.text = "ERROR!: The username and password should have a length of 4 to 8 characters.";
        }
        else
        {
            // 認証リクエストを作成
            var param = new Dictionary<string, object>()
            {
                ["username"] = usernameField.text,
                ["password"] = passwordField.text,
            };
            var client = new RestClient(GameManager.instance.Api);
            var request = new RestRequest(endpoint).AddJsonBody(param);

            // レスポンスを取得
            var response = await client.ExecutePostAsync(request);

            // 認証が成功したとき、トークンを保存してゲーム画面に遷移
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseToken = JsonSerializer.Deserialize<responseToken>(response.Content);
                GameManager.instance.Username = this.usernameField.text;
                GameManager.instance.Token = responseToken.token;
                Debug.Log("認証成功: ユーザネーム: " + GameManager.instance.Username);
                SceneManager.LoadScene("WaitScene");
            }
            // 認証が失敗したとき、エラーメッセージを表示
            else
            {
                var responseMessage = JsonSerializer.Deserialize<responseMessage>(response.Content);
                Debug.Log("エラー: " + responseMessage.message);
                messageText.text = "ERROR!: " + responseMessage.message;
            }
        }
    }
}        
