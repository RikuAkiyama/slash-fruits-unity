using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayController : MonoBehaviour
{
    public void Play()
    {
        // 音を再生
        GetComponent<AudioSource>().Play();
    }
}
