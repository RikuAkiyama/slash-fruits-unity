using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WaitManager : MonoBehaviour
{
    public TextMeshProUGUI countDown;
    float time = 2.7f;
    float displayTime;

    void Update()
    {
        this.time -= Time.deltaTime;
        if(time <= 0)
        {
            SceneManager.LoadScene("GameScene");
        }
        displayTime = time + 0f;
        countDown.text = this.displayTime.ToString("F0");
    }
}
