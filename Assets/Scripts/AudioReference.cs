using UnityEngine;

public class AudioReference : MonoBehaviour
{
    [SerializeField] string audioclip_key;

    //This is for buttons to reference
    public void PlayAudio()
    {
        AudioSfxManager.m_instance.OnPlayNewAudioClip(audioclip_key);
    }
}
