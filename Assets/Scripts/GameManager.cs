using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Secrets secrets;
    public static GameManager instance = null;
    private string username;
    private string token;
    private int score;
    private string api;

    public string Username
    {
        get { return username; }
        set { username = value; }
    }
    public string Token
    {
        get { return token; }
        set { token = value; }
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public string Api
    {
        get { return api; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        this.api = secrets.api;
    }
}