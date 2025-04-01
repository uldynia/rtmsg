using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSfxManager : MonoBehaviour
{
    public static AudioSfxManager m_instance = null;

    [Header("References")]
    [SerializeField] Transform listener_transform;
    [SerializeField] UnityEngine.Audio.AudioMixerGroup output_group = null;

    [Header("Attributes")]
    [SerializeField] int audio_source_init_amount = 10;
    [SerializeField] float distance_per_fade_ratio = 2; //How many distance for every 10% volume fade
    [SerializeField] float distance_start_fade = 2; //How far till distance starts to fade

    List<AudioSource> audio_sources;

    //Store the current audio data
    SFX_TYPE sfx_type_current;
    Vector3 source_position_current;

    public void SetListener(Transform transform)
    {
        listener_transform = transform;
    }
    private void Awake()
    {
        audio_sources = new();
        if (m_instance == null)
            m_instance = this;

        //Create a set number of audio sources at the beginning first
        for (int i = 0; i < audio_source_init_amount; ++i)
        {
            CreateNewAudioSource();
        }

    }
    private void OnEnable()
    {
        //GameEvents.m_instance.playNewAudioClip.AddListener(OnPlayNewAudioClip);
        //GameEvents.m_instance.playNewAudioClip3D.AddListener(OnPlayNewAudioClip3D);
    }
    private void OnDisable()
    {
        //GameEvents.m_instance.playNewAudioClip.RemoveListener(OnPlayNewAudioClip);
        //GameEvents.m_instance.playNewAudioClip3D.RemoveListener(OnPlayNewAudioClip3D);
    }

    public enum SFX_TYPE
    {
        _2D = 0,
        _3D,
        NUM_TOTAL,
    }


    /// <summary>
    /// LISTENERS TO GAME EVENTS
    /// </summary>
    public void OnPlayNewAudioClip(AudioClip audio_clip)
    {
        //Set the attributes for new audio clip
        sfx_type_current = SFX_TYPE._2D;

        //Play the audio clip
        SearchAndPlayAudioClip(audio_clip);
    }
    public void OnPlayNewAudioClip3D(AudioClip audio_clip, Vector3 source_position)
    {
        //Set the attributes for new audio clip
        sfx_type_current = SFX_TYPE._3D;
        source_position_current = source_position;

        //Play the audio clip
        SearchAndPlayAudioClip(audio_clip);
    }
    /// <summary>
    /// LISTENERS TO GAME EVENTS END
    /// </summary>


    void SearchAndPlayAudioClip(AudioClip audio_clip)
    {
        //Loop thru all audiosources, find one thats not playing anything, use that to play the audio
        foreach (AudioSource audio_source in audio_sources)
        {
            //If this audio source is available
            if (!audio_source.isPlaying)
            {
                //Play clip
                PlayAudio(audio_source, audio_clip);
                return; //return earlu
            }
        }

        //An available audio source is not found, create a new one
        PlayAudio(CreateNewAudioSource(), audio_clip);
    }



    //Play Sfx
    void PlayAudio(AudioSource audio_source, AudioClip audio_clip)
    {
        //Check which audio type it is
        switch (sfx_type_current)
        {
            //Play 2D Audio
            case SFX_TYPE._2D:
                audio_source.clip = audio_clip;
                audio_source.volume = 1;
                audio_source.Play();
                break;

            //Play 3D audio
            case SFX_TYPE._3D:
                //Calculate distance
                float distance = (listener_transform.position - source_position_current).magnitude;

                //Calculate volume 
                float volume = 1;
                distance -= distance_start_fade;
                while (distance > distance_per_fade_ratio)
                {
                    volume -= 0.1f;
                    distance -= distance_per_fade_ratio;
                }
                if (volume < 0)
                    volume = 0;

                //Play
                audio_source.clip = audio_clip;
                audio_source.volume = volume;
                audio_source.Play();
                break;
        }

        return;
    }


    AudioSource CreateNewAudioSource()
    {
        //Instantiate new audio source and add to list
        AudioSource new_audio_source = gameObject.AddComponent<AudioSource>();
        new_audio_source.playOnAwake = false;
        new_audio_source.loop = false;
        new_audio_source.outputAudioMixerGroup = output_group;
        
        //Add to list
        audio_sources.Add(new_audio_source);

        //Return
        return new_audio_source;
    }

}
