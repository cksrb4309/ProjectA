using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    public GameObject controllerPanel;
    public void GameSceneLoad()
    {
        FadeInOut.FadeStart(LoadScene);
    }
    public void OpenControlPanel()
    {
        controllerPanel.SetActive(true);
    }
    public void CloseControlPanel()
    {
        controllerPanel.SetActive(false);
    }
    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }
}