using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class AudioManager : MonoBehaviour

{

    //singleton declaration

    #region

    private static AudioManager _instance;

    public static AudioManager Instance

    {

        get

        {

            if (_instance == null) Debug.Log("The AudioManager is NULL");



            return _instance;

        }

    }

    #endregion



    [SerializeField] private List<AudioClip> soundEffects;

    [SerializeField] private List<AudioClip> bgm;



    private Dictionary<string, AudioClip> sfxLookup = new Dictionary<string, AudioClip>();

    private Dictionary<string, AudioClip> bgmLookup = new Dictionary<string, AudioClip>();



    public AudioSource sfxSource, bgmSource, starSource;

    string currBgm = "";

    [SerializeField] public float sfxVolume, bgmVolume;



    private void Awake()

    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;



        foreach (var sfx in soundEffects)

        {

            sfxLookup.Add(sfx.name, sfx);

        }



        foreach (var bg in bgm)

        {

            bgmLookup.Add(bg.name, bg);

        }



        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource = gameObject.AddComponent<AudioSource>();

        starSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume = sfxVolume;

        bgmSource.volume = bgmVolume;

        starSource.volume = sfxVolume;
    }



    private void Start()

    {
        bgmSource.clip = bgmLookup["Menu"];

        bgmSource.Play();

        bgmSource.loop = true;
    }

    public AudioClip GetBGM(string name)
    {
        return bgmLookup[name];
    }



    public AudioClip GetSFX(string name)

    {

        return sfxLookup[name];

    }

    public float PlaySFX(string name, int volume = 1, AudioSource source = null)

    {

        if (!sfxLookup.ContainsKey(name)) return 0;


        AudioClip clip = sfxLookup[name];

        if (source != null) source.PlayOneShot(clip, volume * sfxVolume);
        else sfxSource.PlayOneShot(clip, volume * sfxVolume);

        return clip.length;

    }

    public void PlayBGM(string name)
    {
        if (currBgm == name) return;
        bgmSource.Stop();
        bgmSource.clip = GetBGM(name);
        bgmSource.Play();
        bgmSource.loop = true;
        currBgm = name;
    }
}

