using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public string name;

        [Range(0.1f, 1f)]
        public float volume;
    }

    public AudioMixer audioMixer;
    [SerializeField] AudioClip _menuMusic;
    [SerializeField] AudioClip _gameMusic;

    public AudioData[] sounds;
    public AudioSource soundsAudioSource;

    [SerializeField]
    private AudioSource musicAudioSource;

    private static AudioController _instance;
    public static AudioController Instance => _instance;

    [SerializeField]
    private float volume;

    [SerializeField]
    private float generalVolume;

    private float musicVolume;
    private float sfxVolume;


    [Header("Music")]
    //Fade time.
    public float fadeTime = 2f;

    //Pitch change time.
    public float pitchTime = 1f;

    //Min pitch value to change the sound when displaying the end-game menu.
    public float pitchSlow = .6f;

    private Coroutine fadeCorutine;
    private Coroutine pitchCorutine;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            generalVolume = PlayerPrefs.GetFloat("GeneralMusic", 1f);
            musicVolume = PlayerPrefs.GetFloat("Music", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);

            SetAudioMixerVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
        SetMusicForScene(SceneManager.GetActiveScene().name);
        musicAudioSource.Play();
    }

    private void SetAudioMixerVolumes()
    {
        float gVolume = Mathf.Max(generalVolume, 0.0001f);
        float mVolume = Mathf.Max(musicVolume, 0.0001f);
        float sVolume = Mathf.Max(sfxVolume, 0.0001f);

        audioMixer.SetFloat("GeneralMusic", Mathf.Log10(gVolume) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(mVolume) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(sVolume) * 20);
    }

    public void PlaySound(string name)
    {
        if (TryGetAudioDataWithName(out AudioData audioData, name))
        {
            soundsAudioSource.PlayOneShot(audioData.clip, audioData.volume);
        }
        else
        {
            Debug.LogWarning("No audio clip found with name: " + name);
        }
    }

    /// <summary>
    /// Devuelve el AudioData cuyo nombre coincida con el indicado 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool TryGetAudioDataWithName(out AudioData audioData, string name)
    {
        audioData = new AudioData();
        foreach (AudioData data in sounds)
        {
            if (data.name == name)
            {
                audioData = data;
                return true;
            }
        }
        return false;
    }


    public void SetMusicForScene(string sceneName)
    {
        if (musicAudioSource == null) Debug.LogError("musicAudioSource no asignado en AudioController");
        if (_menuMusic == null || _gameMusic == null) Debug.LogWarning("Clips de música no asignados");
        if (sceneName == "MainMenu")
        {
            Debug.Log("Entró aquí");
            if (musicAudioSource.clip == _menuMusic) return;
            if (fadeCorutine != null) StopCoroutine(fadeCorutine);
            fadeCorutine = StartCoroutine(FadeAndChangeClip(_menuMusic));
            // musicAudioSource.clip = _menuMusic;
        }
        else if (sceneName == "Game")
        {
            if (musicAudioSource.clip == _gameMusic) return;
            if (fadeCorutine != null) StopCoroutine(fadeCorutine);
            fadeCorutine = StartCoroutine(FadeAndChangeClip(_gameMusic));
            // musicAudioSource.clip = _gameMusic;
        }

        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.Play();
        }
    }

    public void SetMusicVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("Music", _volume);
        audioMixer.SetFloat("Music", volume);
        PlayerPrefs.Save();
    }

    public void SetGeneralVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("GeneralMusic", _volume);
        audioMixer.SetFloat("GeneralMusic", volume);
        PlayerPrefs.Save();
    }

    public void SetEffectVolume(float _volume)
    {
        volume = Mathf.Log10(_volume) * 20;
        PlayerPrefs.SetFloat("SFX", _volume);
        audioMixer.SetFloat("SFX", volume);
        PlayerPrefs.Save();
    }

    private IEnumerator FadeAndChangeClip(AudioClip clip)
    {
        //We will use the counter with half the time, since we have to fade in/out.
        float time = fadeTime / 2f;
        float counter = time;
        while (counter > 0)
        {
            musicAudioSource.volume = counter / time;
            counter -= Time.deltaTime;
            yield return null;
        }
        //Music clip change.
        musicAudioSource.clip = clip;
        //We start playback as it stops with the clip change
        musicAudioSource.Play();
        counter = time;
        //We turn up the volume.
        while (counter > 0)
        {
            musicAudioSource.volume = 1 - (counter / time);
            counter -= Time.deltaTime;
            yield return null;
        }
        musicAudioSource.volume = 1f;
    }

    [ContextMenu("Slow")]
    public void PitchSlow()
    {
        if (pitchCorutine != null)
        {
            StopCoroutine(pitchCorutine);
        }
        pitchCorutine = StartCoroutine(SlowMode(true));
    }

    [ContextMenu("Regular")]
    public void PitchRegular()
    {
        if (pitchCorutine != null)
        {
            StopCoroutine(pitchCorutine);
        }
        pitchCorutine = StartCoroutine(SlowMode(false));
    }

    /// <summary>
    /// To reduce the playback pitch.
    /// </summary>
    /// <param name="isSlow"></param>
    /// <returns></returns>
    private IEnumerator SlowMode(bool isSlow)
    {
        float target = isSlow ? pitchSlow : 1;
        float initial = musicAudioSource.pitch;
        float count = 0f;
        while (count < pitchTime)
        {
            musicAudioSource.pitch = Mathf.Lerp(initial, target, count / pitchTime);
            count += Time.deltaTime;
            yield return null;
        }
        musicAudioSource.pitch = target;
    }
}
