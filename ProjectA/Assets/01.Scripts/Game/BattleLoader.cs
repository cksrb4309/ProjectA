using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleLoader : MonoBehaviour
{
    public AnimationCurve speedCurve;

    public SpriteRenderer blinkSr;
    public GameObject portal;
    public GameObject lobby;
    public float max = 0.8f;
    public float min = 0.6f;
    public float alphaSpeed;

    public TMP_Text stageTextUI;
    public RectTransform left;
    public RectTransform right;
    public Image leftImage;
    public Image rightImage;
    public RectTransform centerRect;

    Vector2 pos;

    int stage = -1;
    bool isStageEntered = false;

    float Alpha
    {
        get { return alpha; }
        set { alpha = value; blinkSr.color = new Color(1, 1, 1, alpha); }
    }
    float alpha;
    float baseSizeY;
    bool isCheck = false;
    bool dir = false;
    // dir는 값의 방향으로 true일 때는 증가 false일 때는 알파값을 감소시킴


    private void Awake()
    {
        left.anchoredPosition = new Vector2(-Screen.width / 2f, 0);
        right.anchoredPosition = new Vector2(Screen.width / 2f, 0);

        centerRect.anchoredPosition = new Vector2(Screen.width * 1.6f, 0);
    }

    public void BattleLoad()
    {
        pos.Set(Screen.width * 1.6f, 0);

        centerRect.anchoredPosition = pos;

        if (++stage == 10)
            stageTextUI.text = "빨간 망토";
        else
            stageTextUI.text = "Stage [" + stage.ToString() + ']';

        StartCoroutine(LoadCoroutine(HideLobby, Battle.instance.StartBattle));
    }
    public void ClearLoad()
    {
        pos.Set(Screen.width * 1.6f, 0);

        centerRect.anchoredPosition = pos;

        stageTextUI.text = "Clear";

        if (stage != 10)
        {
            StartCoroutine(
                LoadCoroutine(ShowLobby,
                Inventory.instance.selectPanelGroup.StartSelectItem));
        }

        isCheck = true;
        dir = false;
    }
    public void ActivePortal()
    {
        isStageEntered = false;
    }
    void ShowLobby()
    {
        lobby.SetActive(true);

        portal.SetActive(true);
    }
    void HideLobby()
    {
        lobby.SetActive(false);

        portal.SetActive(false);
    }

    IEnumerator LoadCoroutine(Action mid = null, Action end = null)
    {
        float t = 0;

        Vector2 startPos = pos;

        Vector2 midPos = Vector2.zero;

        Vector2 endPos = -pos;

        leftImage.enabled = true;
        rightImage.enabled = true;

        while (t < 5f)
        {
            t += Time.deltaTime * speedCurve.Evaluate(t/5);
            
            centerRect.anchoredPosition = Vector2.Lerp(startPos, midPos, t / 5f);

            yield return null;
        }

        centerRect.anchoredPosition = midPos;

        if (mid != null) mid.Invoke();

        t = 0;

        startPos = midPos;

        while (t < 5f)
        {
            t += Time.deltaTime * speedCurve.Evaluate(t/5);

            centerRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t / 5);

            yield return null;
        }

        centerRect.anchoredPosition = endPos;

        leftImage.enabled = false;
        rightImage.enabled = false;

        if (end != null) end.Invoke();
    }

    private void Update()
    {
        if (!isStageEntered && isCheck)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isStageEntered = true;

                isCheck = false;

                BattleLoad();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isStageEntered && isCheck)
        {
            if (dir)
            {
                if (Alpha < max) Alpha += Time.fixedDeltaTime * alphaSpeed;
                else dir = false;
            }
            else
            {
                if (Alpha > min) Alpha -= Time.fixedDeltaTime * alphaSpeed;
                else dir = true;
            }
        }
        else if (Alpha > 0) Alpha -= Time.fixedDeltaTime * alphaSpeed;
    }
    public void StartAnimation()
    {
        Alpha = 0;
        isCheck = true;
        dir = true;
    }
    public void EndAnimation()
    {
        isCheck = false;
        dir = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCheck = true;
            StartAnimation();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCheck = false;
            EndAnimation();
        }
    }
}
