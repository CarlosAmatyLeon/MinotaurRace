using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagerMaze : MonoBehaviour
{   
    public static SceneManagerMaze Instance;

    [HideInInspector] public int mazeSize;
    [HideInInspector] public int difficulty;
    [HideInInspector] public bool GameStarted = false;
    [HideInInspector] public int playerScore = 0;
    [HideInInspector] public int minotaurScore = 0;

    [SerializeField] TextMeshProUGUI playerScoreText;
    [SerializeField] TextMeshProUGUI minotaurScoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] TextMeshProUGUI GameOverText;    

    [SerializeField] AudioClip backgroundAudio;
    [SerializeField] MiniMapCameraController MiniMapCameraController;
    [SerializeField] OverviewCameraController OverviewCameraController;
    [SerializeField] MazeManager MazeManager;
    [SerializeField] SpawnManager SpawnManager;
    [SerializeField] MinotaurController MinotaurController;
    [SerializeField] PlayerController PlayerController;
    [SerializeField] GameObject MinotaurCamera;
    
    float countdownSeconds = 10.0f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (GameManager.Instance != null)
            {
                mazeSize = GameManager.Instance.mazeSize;
                difficulty = GameManager.Instance.difficulty;
            }
            else //The else is only for debugging purposes since the size is set in the menu scene
            {
                mazeSize = 8;
                difficulty = 3;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        AudioManager.Instance.PlayBackgroundLoop(backgroundAudio);
        LevelSetup();
    }
    private void Update()
    {
        if (GameStarted)
        {
            countdownSeconds -= Time.deltaTime;
            if (countdownSeconds <= 0)
            {
                countdownSeconds = 0;
                GameStarted = false;
                if (playerScore > minotaurScore)
                {
                    GameOverText.text = "You Win";
                }
                else if (playerScore < minotaurScore)
                {
                    GameOverText.text = "You Lose";
                }
                else
                {
                    GameOverText.text = "It's a draw";
                }
                PlayerController.GameOver();
                MinotaurController.GameOver();
                GameOverPanel.SetActive(true);
            }            
            timerText.text = FormatTime(countdownSeconds);
        }
    }

    void LevelSetup()
    {
        timerText.text = FormatTime(countdownSeconds);
        GameOverPanel.SetActive(false);

        //CamerasInitialization
        OverviewCameraController.MoveCamera();
        MiniMapCameraController.SetupCamera();

        //Maze initialization
        MazeManager.SetGameOver(false);
        MazeManager.SetMazeSize(mazeSize);
        MazeManager.SetMazeDelay(30.0f);
        MazeManager.SetWallDelay(3.0f/(mazeSize*mazeSize));
        MazeManager.SetWallSpeed(5.0f);
        MazeManager.StartMaze();

        //Spawner initialization
        SpawnManager.SetInterval(3.0f);
        SpawnManager.SetLifeTime(10.0f);

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(5);
        OverviewCameraController.gameObject.SetActive(false);

        yield return new WaitForSeconds(4);
        MazeManager.SetWallDelay(0.1f);
        MinotaurController.BeginSequence();

        yield return new WaitForSeconds(3);
        MinotaurCamera.SetActive(false);
        GameStarted = true;
        SpawnManager.StartGeneratingCoins();
        PlayerController.SetIsMoving(true);
    }


    public void PlayerScoreUp()
    {
        playerScore++;
        playerScoreText.text = $"{playerScore}";
    }

    public void MinotaurScoreUp()
    {
        minotaurScore++;
        minotaurScoreText.text = $"{minotaurScore}";
    }
    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
