using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagerMenu : MonoBehaviour
{
    public AudioClip backgroundAudio;
    public TextMeshProUGUI mazeSizeText = null;
    
    void Start()
    {
        AudioManager.Instance.PlayBackgroundLoop(backgroundAudio);
        mazeSizeText.text = "7";
        GameManager.Instance.mazeSize = 7;
        GameManager.Instance.difficulty = 1;
    }

    public void mazeSizeChange(float value)
    {   
        mazeSizeText.text = $"{value}";
        GameManager.Instance.mazeSize = (int)value;
    }

    public void DifficultyChange(int value)
    {
        GameManager.Instance.difficulty = value + 1;
    }

    public void onStartGame()
    {
        SceneManager.LoadScene("Maze");
    }
}
