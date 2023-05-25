using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    Camera m_Camera;

    public GameObject Player;

    int stage;
    int score;
    int lastScore;

    float timer;
    float timeLimit = 60f;

    bool gameOver;
    bool stageComplete;

    public bool GamePaused { get { if (!gameOver && !stageComplete) return false; else return true; } }


    // Start is called before the first frame update
    void Start()
    {
        stage = 1;

        if (StartMngr.Instance != null)
        {
            timeLimit *= (int)StartMngr.Instance.UserDifficulty + 1;
        }

        timer = timeLimit;

        SetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GamePaused)
        {
            UpdateTimer();
        }
    }

    public void UpdateScore(int point = 10)
    {
        score += point;

        UIMngr.Instance.Score.text = score.ToString();
    }

    public void UpdateStage()
    {
        stage++;
        lastScore = score;

        UIMngr.Instance.Stage.text = stage.ToString();
    }

    public void UpdateTimer()
    {
        if (!GamePaused)
        {
            timer -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);
            UIMngr.Instance.Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (timer < 1)
        {
            UIMngr.Instance.Timer.text = string.Format("{0:00}:{1:00}", 0, 0);
            GameOver();
        }
    }

    public void SetStageComplete()
    {
        stageComplete = true;

        UIMngr.Instance.PanelStageComplete.gameObject.SetActive(true);
        UIMngr.Instance.Restart();
    }

    public void SetNextStage()
    {
        UIMngr.Instance.PanelStageComplete.gameObject.SetActive(false);

        gameOver = false;
        stageComplete = false;
        timer = timeLimit + 40f;

        UpdateStage();
        MazeSpawner.Instance.ResetMaze();

        Player.GetComponent<Player>().Restart();
        m_Camera.transform.position = new Vector3((int)(MazeSpawner.Instance.width * 0.5f), MazeSpawner.Instance.width, (int)(MazeSpawner.Instance.height * 0.5f));
    }

    public void GameOver()
    {
        gameOver = true;

        UIMngr.Instance.PanelGameOver.gameObject.SetActive(true);
        UIMngr.Instance.Restart();
    }

    public void ResetStage()
    {
        UIMngr.Instance.PanelGameOver.gameObject.SetActive(false);

        gameOver = false;
        stageComplete = false;

        timer = timeLimit;
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        UIMngr.Instance.Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        score = lastScore;
        UIMngr.Instance.Score.text = score.ToString();

        MazeSpawner.Instance.ResetMaze(false);

        Player.GetComponent<Player>().Restart();
    }

    void SetCamera()
    {
        m_Camera = Camera.main;

        // 2D View [3D is Default]
        if (StartMngr.Instance != null && StartMngr.Instance.ViewType == 1)
        {
            m_Camera.orthographic = true;
            m_Camera.orthographicSize = 3;
        }

        m_Camera.transform.position = new Vector3((int)(MazeSpawner.Instance.width * 0.5f), MazeSpawner.Instance.width, (int)(MazeSpawner.Instance.height * 0.5f));
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // Quit from Editor Mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit from Editor Mode
         Application.Quit();
#endif
    }
}
