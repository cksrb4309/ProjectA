using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    public GameObject showItemPanel;

    public void ShowItem()
    {
        showItemPanel.SetActive(!showItemPanel.activeSelf);
    }
    public void LoadScene()
    {
        FadeInOut.FadeStart(TryLoad);
    }
    void TryLoad()
    {
        SceneManager.LoadScene("Title");
    }
}
