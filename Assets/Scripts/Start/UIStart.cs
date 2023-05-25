using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStart : MonoBehaviour
{
    void Start()
    {
        StartMngr.Instance.ResetStats();
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit() 
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    public void SetMusic()
    {
        StartMngr.Instance.MusicPref = !StartMngr.Instance.MusicPref;

        if (!StartMngr.Instance.MusicPref)
        {
            StartMngr.Instance.MainSource.Stop();
        }
        else
        {
            StartMngr.Instance.MainSource.Play();
        }
    }

    public void SetView(int choice)
    {
        StartMngr.Instance.ViewType = choice;
    }

    public void SetDifficulty(int choice)
    {
        StartMngr.Instance.UserDifficulty = (Difficulty)choice;
    }
}

public enum Difficulty
{
    Normal, Hard, Extreme, Random
}
