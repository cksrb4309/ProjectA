using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialUI : MonoBehaviour
{
    public RawImage videoRenderImage;
    public VideoPlayer videoPlayer;

    public GameObject[] blocks;    // �̵�, ����, ������, ����, ���ϰ���, ��ȣ�ۿ� ����
    public VideoClip[] videoClips; // ��

    GameObject beforeBlock = null;
    private void OnEnable()
    {
        videoRenderImage.enabled = false;
        videoPlayer.Stop();
    }
    private void OnDisable()
    {
        videoRenderImage.enabled = false;

        if (beforeBlock != null)
            beforeBlock.SetActive(false);
    }
    public void Play(int index)
    {
        videoRenderImage.enabled = true;
        videoPlayer.clip = videoClips[index];

        if (beforeBlock != null)    
            beforeBlock.SetActive(false);

        beforeBlock = blocks[index];
        blocks[index].SetActive(true);

        videoPlayer.Play();
    }
}
