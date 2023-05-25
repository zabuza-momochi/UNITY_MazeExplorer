using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMngr : MonoBehaviour
{
    public static StartMngr Instance;

    public Difficulty UserDifficulty = Difficulty.Normal;
    public bool MusicPref;
    public int ViewType;

    public AudioSource MainSource;
    public List<AudioClip> ThemesList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        UserDifficulty = Difficulty.Normal;
        ViewType = 0;
    }

    public void ResetStats()
    {
        UserDifficulty = Difficulty.Normal;
        MusicPref = true;
        MainSource.Play();

        ViewType = 0;
    }

    // called first
    void OnEnable()
    {
        MainSource = GetComponent<AudioSource>();
        MusicPref = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MainSource.clip = ThemesList[scene.buildIndex];

        if (MusicPref)
        {
            MainSource.Play();
        }
    }
}