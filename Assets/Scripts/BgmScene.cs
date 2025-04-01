using UnityEngine;

public class BgmScene : MonoBehaviour
{
    [SerializeField] AudioBgmManager.AUDIO_BGM_TYPE bgm_type;

    void Start()
    {
        AudioBgmManager.m_instance.PlayBgmType(bgm_type);

        if (AudioBgmManager.m_instance == null)
        {
            Debug.Log("What the");
        }
        else
        {
            Debug.Log("Found the");

        }
    }

}
