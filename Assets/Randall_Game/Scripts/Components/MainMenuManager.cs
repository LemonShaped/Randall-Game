using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Exit()
    {
        Application.Quit();
    }


}
