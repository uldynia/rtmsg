using UnityEngine;
using System.Collections;
using System;

public class AudioBgmManager : MonoBehaviour
{
    public static AudioBgmManager m_instance;

    [Header("Default BGM type")]
    [SerializeField] protected AUDIO_BGM_TYPE default_bgm = AUDIO_BGM_TYPE.MENU;

    //References
    [Header("2 Audio Sources only for Mixing Fade Effects")]
    [SerializeField] protected AudioSource[] audio_source;

    public AudioClip[] ambient_audio;
    public AudioClip[] action_audio;
    public AudioClip[] menu_audio;

    //Vars
    protected int audio_source_current = 0;
    protected const float fade_duration = 3f; //Const for optimisation during calculation n it shouldnt need to change anyway
    protected AUDIO_BGM_TYPE audio_type_current = AUDIO_BGM_TYPE.NUM_BGM_TYPES;


    //CHOICE of BGM
    [SerializeField] public bool randomise_music_ambient;
    [SerializeField] public bool randomise_music_action;
    [SerializeField] public bool randomise_music_menu;
    public int selected_ambient_index = -1;
    public int selected_action_index = -1;
    public int selected_menu_index = -1;

    int random_ambient_index = 0;
    int random_action_index = 0;
    int random_menu_index = 0;
    float clip_length_remaining = 0;
    bool fading_in_process = false; //Cannot change BGM when alr being changed

    public enum AUDIO_BGM_TYPE
    {
        AMBIENT = 0,
        ACTION,
        MENU,
        NUM_BGM_TYPES,
    }

    AudioClip custom_bgm = null;
    float fading_duration_inverse;

    private void Awake()
    {
        ////set up routines
        //fadingout_routine = FadeOut(audio_source[audio_source_current], fade_duration);
        //fadingin_routine = FadeIn(audio_source[audio_source_current], fade_duration);

        //set up random music
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        random_ambient_index = UnityEngine.Random.Range(0, ambient_audio.Length);
        random_action_index = UnityEngine.Random.Range(0, action_audio.Length);
        random_menu_index = UnityEngine.Random.Range(0, menu_audio.Length);

        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);

            ////Disable both first so the OnAwake can be called when one turns active
            //foreach (AudioSource audio in audio_source)
            //    audio.enabled = false;

            //PlayRandom(default_bgm);
        }
        else
        {
            Destroy(gameObject);
        }

        fading_duration_inverse = 1 / fade_duration;
    }


    private void Update()
    {
        //time the end of a song
        if (clip_length_remaining > 0)
        {
            clip_length_remaining -= Time.unscaledDeltaTime;

        }
        //Play the next one
        else
        {
            if (custom_bgm != null)
                PlaySelected(custom_bgm);
            else
                PlayRandom(audio_type_current);
        }

        //Coroutines are unreliable when changes are made frequently such as if scene changes too fast
        //Update is more optimised than Coroutines anyway
        if (fading_in_process)
        {
            //Adjust both
            if (audio_source_current == 0) {
                audio_source[0].volume += Time.deltaTime * fading_duration_inverse; 
                audio_source[1].volume -= Time.deltaTime * fading_duration_inverse;

                //Check if reached the end
                if (audio_source[0].volume == 1 &&
                    audio_source[1].volume == 0) {
                    fading_in_process = false;
                }
            }
            else {
                audio_source[0].volume -= Time.deltaTime * fading_duration_inverse;
                audio_source[1].volume += Time.deltaTime * fading_duration_inverse;

                //Check if reached the end
                if (audio_source[0].volume == 0 &&
                    audio_source[1].volume == 1) {
                    fading_in_process = false;
                }
            }
        }
    }

    public void PlayBgmType(AUDIO_BGM_TYPE audio_type)
    {
        //Find the selected index
        bool type_is_random = audio_type switch
        {
            AUDIO_BGM_TYPE.AMBIENT => randomise_music_ambient,
            AUDIO_BGM_TYPE.ACTION => randomise_music_action,
            AUDIO_BGM_TYPE.MENU => randomise_music_menu,
            _ => true,
        };

        if (type_is_random)
            PlayRandom(audio_type);
        else
            PlaySelected(audio_type);
    }
    public void PlayAmbient()
    {
        if (randomise_music_ambient)
            PlayRandom(AUDIO_BGM_TYPE.AMBIENT);
        else
            PlaySelected(AUDIO_BGM_TYPE.AMBIENT);
    }
    public void PlayAction()
    {
        if (randomise_music_action)
            PlayRandom(AUDIO_BGM_TYPE.ACTION);
        else
            PlaySelected(AUDIO_BGM_TYPE.ACTION);
    }
    public void PlayMenu()
    {
        if (randomise_music_menu)
            PlayRandom(AUDIO_BGM_TYPE.MENU);
        else
            PlaySelected(AUDIO_BGM_TYPE.MENU);
    }
    public void PlayRandom(AUDIO_BGM_TYPE audio_type)
    {
        //if (fading_in_process)
        //{
        //    Debug.LogWarning("BGM Change failed, Already changing");
        //    return;
        //}

        //dont override the same category of music
        if (audio_type == audio_type_current)
            return;

        custom_bgm = null;

        //Play the new audio in the other source first to fade in
        int new_index = 0;
        switch (audio_type)
        {
            case AUDIO_BGM_TYPE.AMBIENT:
                random_ambient_index++;
                if (random_ambient_index >= GetAudioType(audio_type).Length)
                    random_ambient_index = 0;
                new_index = random_ambient_index;
                break;
            case AUDIO_BGM_TYPE.ACTION:
                random_action_index++;
                if (random_action_index >= GetAudioType(audio_type).Length)
                    random_action_index = 0;
                new_index = random_action_index;
                break;
            case AUDIO_BGM_TYPE.MENU:
                random_menu_index++;
                if (random_menu_index >= GetAudioType(audio_type).Length)
                    random_menu_index = 0;
                new_index = random_menu_index;
                break;
        }

        if (GetAudioType(audio_type).Length == 0)
            return;

        //Set new current
        audio_type_current = audio_type;

        //Fade out the old audio source 
        //StopCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));

        //Switch source
        audio_source_current++;
        if (audio_source_current >= audio_source.Length)
            audio_source_current = 0;

        audio_source[audio_source_current].clip =
            GetAudioType(audio_type)[new_index];
        clip_length_remaining = audio_source[audio_source_current].clip.length;
        //StopCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        FadeInOut();

    }
    public void SetVolume(float volume)
    {
        foreach (AudioSource audio in audio_source)
            audio.volume = volume;
    }
    public void PlaySelected(AUDIO_BGM_TYPE audio_type)
    {
        //if (fading_in_process)
        //{
        //    Debug.LogWarning("BGM Change failed, Already changing");
        //    return;
        //}

        //Find the selected index
        int index = audio_type switch
        {
            AUDIO_BGM_TYPE.AMBIENT => selected_ambient_index,
            AUDIO_BGM_TYPE.ACTION => selected_action_index,
            AUDIO_BGM_TYPE.MENU => selected_menu_index,
            _ => -1,
        };
        //NOT SELECTED OR NOT FOUND
        if (index == -1)
        {
            Debug.LogWarning("Music not selected");
            return;
        }

        //Set new current, dont override the same category of music
        audio_type_current = audio_type;

        ////Fade out the old audio source 
        ////StopCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));

        //Switch source
        audio_source_current++;
        if (audio_source_current >= audio_source.Length)
            audio_source_current = 0;

        //Play the new audio in the other source first to fade in
        audio_source[audio_source_current].clip =
            GetAudioType(audio_type)[index];
        clip_length_remaining = audio_source[audio_source_current].clip.length;
        ////StopCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        FadeInOut();

    }
    public void PlaySelected(AudioClip custom_bgm)
    {
        //if (fading_in_process)
        //{
        //    Debug.LogWarning("BGM Change failed, Already changing");
        //    return;
        //}

        this.custom_bgm = custom_bgm;

        ////Fade out the old audio source 
        ////StopCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeOut(audio_source[audio_source_current], fade_duration));

        //Switch source
        audio_source_current++;
        if (audio_source_current >= audio_source.Length)
            audio_source_current = 0;

        //Play the new audio in the other source first to fade in
        audio_source[audio_source_current].clip =
            custom_bgm;
        clip_length_remaining = custom_bgm.length;
        ////StopCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        //StartCoroutine(FadeIn(audio_source[audio_source_current], fade_duration));
        FadeInOut();
    }
    public void StopPlayCustom()
    {
        custom_bgm = null;
    }
    public void Skip()
    {
        clip_length_remaining = 0;

    }

    protected AudioClip[] GetAudioType(AUDIO_BGM_TYPE audio_type)
    {
        return audio_type switch
        {
            AUDIO_BGM_TYPE.AMBIENT => ambient_audio,
            AUDIO_BGM_TYPE.ACTION => action_audio,
            AUDIO_BGM_TYPE.MENU => menu_audio,
            _ => null,
        };
    }


    void FadeInOut()
    {
        if (!audio_source[0].isPlaying) audio_source[0].Play(); //incase they werent playing before
        if (!audio_source[1].isPlaying) audio_source[1].Play(); //incase they werent playing before
        fading_in_process = true;
    }
    //IEnumerator FadeOut(AudioSource audio_source, float duration)
    //{
    //    fading_in_process = true;
    //    for (int i = 0; i < 10; ++i)
    //    {
    //        audio_source.volume -= 0.1f;
    //        yield return new WaitForSecondsRealtime(0.1f * duration);
    //    }
    //    audio_source.enabled = false;
    //    fading_in_process = false;
    //}

    //IEnumerator FadeIn(AudioSource audio_source, float duration)
    //{
    //    audio_source.enabled = true;
    //    audio_source.volume = 0;
    //    for (int i = 0; i < 10; ++i)
    //    {
    //        audio_source.volume += 0.1f;
    //        yield return new WaitForSecondsRealtime(0.1f * duration);
    //    }
    //    audio_source.enabled = true;
    //}
}
