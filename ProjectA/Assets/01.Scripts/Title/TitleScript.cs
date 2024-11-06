using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class TitleScript : MonoBehaviour
{
    public bool isBgm = false;
    public GameObject controllerPanel;
    public GameObject optionPanel;
    private void Start()
    {
        if (isBgm) 
            SoundManager.Play("TitleBGM", SoundType.Background);

        Time.timeScale = 1;
    }

    public void GameSceneLoad()
    {
        FadeInOut.FadeStart(LoadScene);
    }
    public void OpenControlPanel()
    {
        controllerPanel.SetActive(true);
    }
    public void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void CloseControlPanel()
    {
        controllerPanel.SetActive(false);
    }
    private void LoadScene()
    {
        SoundManager.Play("GameBGM", SoundType.Background);

        SceneManager.LoadScene("Game");
    }
    public void ExitButton()
    {/*
#if UNITY_WEBGL && !UNITY_EDITOR
    CloseTab(); // �� ���� �ÿ��� JavaScript �Լ� ȣ��
#else*/
    Application.Quit(); // �����ͳ� �ٸ� ȯ�濡���� �Ϲ� ����
//#endif
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (!optionPanel.activeSelf)
            {
                OpenOptionPanel();
            }
            else
            {
                CloseOptionPanel();
            }
        }
    }
}