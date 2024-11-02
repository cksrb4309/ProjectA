using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    public GameObject player;
    public Monster lastHitMonster = null;

    public GameObject deadPanel;
    public GameObject clearPanel;

    private void Awake()
    {
        instance = this;
    }

    public void ClearGame()
    {
        clearPanel.SetActive(true);
    }
    public void Die()
    {
        deadPanel.SetActive(true);
    }
}
