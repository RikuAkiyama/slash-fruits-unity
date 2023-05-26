using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 
using TMPro;
using UnityEngine.SceneManagement;

public class LoginRegister : MonoBehaviour
{
    public static LoginRegister Instance;

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button registerButton;
    public Button loginButton;

    public static string AuthToken { get; private set; }

    private string registerUrl = "https://slash-fruits-bslg2vwzva-an.a.run.app/register";
    private string loginUrl = "https://slash-fruits-bslg2vwzva-an.a.run.app/login";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        registerButton.onClick.AddListener(() => StartCoroutine(Authenticate(registerUrl)));
        loginButton.onClick.AddListener(() => StartCoroutine(Authenticate(loginUrl)));
    }

    public class PostData
    {
        public string username;
        public string password;
    }

    IEnumerator Authenticate(string url)
    {
        Debug.Log($"Sending request to {url}");

        PostData postData = new PostData
        {
            username = usernameField.text,
            password = passwordField.text
        };

        string json = JsonUtility.ToJson(postData);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Log the request headers
        Debug.Log($"Request header: Content-Type: {request.GetRequestHeader("Content-Type")}");

        // Log the request body
        Debug.Log("Request body:");
        Debug.Log(json);

        yield return request.SendWebRequest();

        Debug.Log($"Received response from {url}");
        ProcessResponse(request);
    }

    private void ProcessResponse(UnityWebRequest request)
    {
        Debug.Log($"Processing response for {request.url}");

        int statusCode = (int)request.responseCode;
        Debug.Log($"Status code: {statusCode}");

        string responseText = request.downloadHandler.text;

        // Log the response headers
        Debug.Log("Response headers:");
        foreach (var header in request.GetResponseHeaders())
        {
            Debug.Log($"{header.Key}: {header.Value}");
        }

        // Log the response body
        Debug.Log("Response body:");
        Debug.Log(responseText);

        var responseJson = JsonUtility.FromJson<Dictionary<string, string>>(responseText);

        Debug.Log("Response json:");
        Debug.Log(responseJson);
        foreach (var kvp in responseJson)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.result);
            Debug.LogError(request.error);

            if (responseJson.ContainsKey("message"))
            {
                Debug.LogError($"Error: {request.responseCode}, Message: {responseJson["message"]}");
            }
        }
        else
        {
            if (request.responseCode == 200 && responseJson.ContainsKey("token"))
            {
                AuthToken = responseJson["token"];
                Debug.Log("Authentication successful.");
                SceneManager.LoadScene("GameScene");
            }
            else if (responseJson.ContainsKey("message"))
            {
                Debug.LogError($"Error: {request.responseCode}, Message: {responseJson["message"]}");
            }
            else
            {
                Debug.LogError("Unexpected response.");
            }
        }
    }
}
