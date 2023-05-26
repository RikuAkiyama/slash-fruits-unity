using System.Net;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using RestSharp;
using RestSharp.Authenticators;

public class GameDirector : MonoBehaviour
{
   GameObject timerText;
   GameObject pointText;
   public float time = 30.0f;
   int point = 0;
   bool flag = true;

   public class responseMessage
   {
      public string message { get; set; }
   }
   
   public void HitGoldApple()
   {
      this.point += 500;
      // this.point *= 2;      
   }
   
   public void HitApple()
   {
      this.point += 50;
   }

   public void HitPear()
   {
      this.point += 25;
   }

   public void HitKiwi()
   {
      this.point += 125;
   }

   public void HitLemon()
   {
      this.point += 100;
   }
   
   public void HitOrange()
   {
      this.point += 75;
   }
    
   void Start()
   {
      GameManager.instance.Score = this.point;
      this.timerText = GameObject.Find("Time");
      this.pointText = GameObject.Find("Point");
      // GameBGMを再生
      GameObject.Find("GameBGM").GetComponent<AudioPlayController>().Play();
   }
    
   void Update()
   {
      this.time -= Time.deltaTime;
      if(0<this.time)
      {
         this.timerText.GetComponent<TextMeshProUGUI>().text = this.time.ToString("F1");
         this.pointText.GetComponent<TextMeshProUGUI>().text = this.point.ToString() + " P";
      }
      else
      {
         this.timerText.GetComponent<TextMeshProUGUI>().text = "0.0";
         if(this.flag)
         {
            this.flag = false;
            GameManager.instance.Score = this.point;
            SendScore("/score");
         }
      }
   }

   private async void SendScore(string endpoint)
   {
      // スコア登録リクエストを作成
      var param = new Dictionary<string, object>()
      {
         ["score"] = GameManager.instance.Score,
      };
      var client = new RestClient(GameManager.instance.Api);
      var authenticator = new JwtAuthenticator(GameManager.instance.Token);
      var request = new RestRequest(endpoint)
      {
         Authenticator = authenticator
      };
      request.AddJsonBody(param);
      
      // レスポンスを取得
      var response = await client.ExecutePostAsync(request);
      if(response.StatusCode == HttpStatusCode.OK)
      {
         Debug.Log("スコア登録成功");
         SceneManager.LoadScene("ResultScene");
      }
      else
      {
         var responseMessage = JsonSerializer.Deserialize<responseMessage>(response.Content);
         Debug.Log("エラー: " + responseMessage.message);
         SceneManager.LoadScene("ResultScene");
      }
   }
}
