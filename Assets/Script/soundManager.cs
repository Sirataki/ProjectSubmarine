using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class AudioSet
{
    public AudioClip seClip;
    public float volume;
}

public class soundManager : MonoBehaviour
{
    #region //シングルトン
    public static soundManager Instance{ get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    #endregion
    [SerializeField]
    public List<AudioSet> seList = new List<AudioSet>();
    public AudioSource bgmSource;
    public AudioSource seSource;
    public AudioClip[] bgmClip;
    // Use this for initialization
    public void PlaySound(int seNum, bool isPlaying)
    {
        if (isPlaying)
        {
            //seSource.volume =  seList[seNum].volume;
            if (!seSource.isPlaying)
            {
                seSource.PlayOneShot(seList[seNum].seClip, seList[seNum].volume);
            }
        }
        else
        {
            seSource.PlayOneShot(seList[seNum].seClip, seList[seNum].volume);
        }
    }
    public void ChangeBgm(int bgmNum)
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.clip = bgmClip[bgmNum];
            bgmSource.Play();
        }
    }
    public void StopBgm()
    {
        bgmSource.Stop();
    }
}
