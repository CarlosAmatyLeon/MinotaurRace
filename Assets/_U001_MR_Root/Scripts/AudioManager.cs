using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] public AudioSource sfxPlayer;

    private void Awake()
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
    public void PlaySFX(AudioClip clip)
    {
        sfxPlayer.clip = clip;
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlayBackgroundLoop(AudioClip clip)
    {
        musicPlayer.loop = true;
        musicPlayer.clip = clip;
        musicPlayer.Play();
    }
}
