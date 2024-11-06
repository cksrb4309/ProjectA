using UnityEngine;

public class MagicButton : MonoBehaviour
{
    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }
    public void GameClear()
    {
        GameManager.Instance.GameClear();
    }
}
