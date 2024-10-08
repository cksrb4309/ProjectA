using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public void GameSceneLoad()
    {
        SceneManager.LoadScene("Game");
    }
}