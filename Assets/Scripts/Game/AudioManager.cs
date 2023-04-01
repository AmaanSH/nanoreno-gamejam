using Nanoreno.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource bgmSource;

    public AudioSource baseSource;
    public Transform audioParent;
    public AudioMixer bgmMixer;

    private List<AudioSource> audioSources = new List<AudioSource>();

    private const string VOLUME_BGM_MIXER = "volume";
    private const float FADE_DURATION = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetupAudioForSection(ControlNode controlNode)
    {
        if (controlNode.stopAllLayers)
        {
            StopAllLayers();
        }
        else if (controlNode.layeredAudio.Count > 0)
        {
            foreach(LayerAudio audio in controlNode.layeredAudio)
            {
                if (audio.Play)
                {
                    StartCoroutine(PlayLayer(audio.audioClip));
                }
                else
                {
                    StartCoroutine(StopLayer(audio.audioClip));
                }
            }
        }

        if (controlNode.stopBGM)
        {
            ResetAudio();
        }
        else if (controlNode.BGM)
        {
            StartCoroutine(PlayBGM(controlNode.BGM));
        }

        if (controlNode.SFX)
        {
            PlaySFX(controlNode.SFX);
        }
    }

    public void AddAudioToLayers(AudioClip clip)
    {
        AudioSource foundSource = audioSources.Find(x => x.clip == clip);
        if (foundSource != null) { return; }

        var source = Instantiate(baseSource, audioParent);

        source.clip = clip;
        source.volume = 0;
        source.loop = true;
        source.Play();

        audioSources.Add(source);
    }

    public IEnumerator PlayLayer(AudioClip clip)
    {
        AudioSource source = audioSources.Find(x => x.clip == clip);
        if (source != null)
        {
            yield return FadeAudio(source);
        }
    }

    public IEnumerator StopLayer(AudioClip clip)
    {
        AudioSource source = audioSources.Find(x => x.clip == clip);
        if (source != null)
        {
            yield return FadeAudio(source, true);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }

        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public IEnumerator PlayBGM(AudioClip clip)
    {
        if (bgmSource.isPlaying)
        {
            if (bgmSource.clip == clip)
                yield return null;

            yield return StartFade(bgmMixer, VOLUME_BGM_MIXER, FADE_DURATION, 0);
        }

        bgmSource.loop = true;
        bgmSource.clip = clip;
        bgmSource.Play();

        yield return StartFade(bgmMixer, VOLUME_BGM_MIXER, FADE_DURATION, 1);
    }

    public void ResetAudio()
    {
        StartCoroutine(StartFade(bgmMixer, VOLUME_BGM_MIXER, FADE_DURATION, 0));

        sfxSource.Stop();
    }

    public void ClearAllLayers()
    {
        foreach(AudioSource source in audioSources)
        {
            source.Stop();
            Destroy(source.gameObject);
        }

        bgmSource.Stop();
        sfxSource.Stop();

        audioSources.Clear();
    }

    public void StopAllLayers()
    {
        foreach (AudioSource source in audioSources)
        {
            StartCoroutine(FadeAudio(source, true));
        }
    }

    private IEnumerator FadeAudio(AudioSource source, bool fadeOut = false)
    {
        float end = fadeOut ? 0 : 1;

        if (!source.isPlaying)
        {
            source.volume = 0;
            source.Play();
        }

        while (source.volume != end)
        {
            source.volume = Mathf.MoveTowards(source.volume, end, Time.deltaTime * 0.7f);
            yield return null;
        }

        // just in case
        source.volume = end;
    }

    private IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }
}
