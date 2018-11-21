using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    public AudioClip boopEffect;
    public AudioClip chingEffect;
    public AudioClip wooshEffect;

    public AudioClip music;

    private void Start()
    {
        PlayRepeating(music);
    }

    public void Mute()
    {
        gameObject.SetActive(!gameObject.active);
    }

    public AudioSource PlaySingle(AudioClip clip)
    {
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.Play();
        StartCoroutine(StopClip(clip, source));

        return source;
    }

    public void PlayRepeating(AudioClip clip)
    {
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.Play();
        source.loop = true;
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        AudioSource source = GetAudioSource();
        source.pitch = randomPitch;
        source.clip = clips[randomIndex];
        source.Play();

        StartCoroutine(StopClip(clips[randomIndex], source));
    }

    public void FocusVolume(AudioSource source, float amount, float fadeIn, float noFade, float fadeOut)
    {
        StartCoroutine(ChangeVolume(source, amount, fadeIn, noFade, fadeOut));
    }

    private IEnumerator ChangeVolume(AudioSource source, float amount, float fadeIn, float noFade, float fadeOut)
    {
        float time = 0;

        AudioSource[] sources = GetComponents<AudioSource>();

        while (time < fadeIn + noFade + fadeOut)
        {
            if (time < fadeIn)
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    if (sources[i] != source)
                        sources[i].volume = Mathf.Lerp(1, amount, time / fadeIn);
                }
            }
            else if (time < fadeIn + noFade)
            {

            }
            else
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    if (sources[i] != source)
                        sources[i].volume = Mathf.Lerp(amount, 1, (time - fadeIn - fadeOut) / fadeOut);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    private AudioSource GetAudioSource()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.clip == null)
            {
                return source;
            }
        }

        return gameObject.AddComponent<AudioSource>();
    }

    private IEnumerator StopClip(AudioClip clip, AudioSource source)
    {
        yield return new WaitForSeconds(clip.length);
        source.clip = null;
    }

    public void Boop()
    {
        PlaySingle(boopEffect);
    }

    public void Ching()
    {
        PlaySingle(chingEffect);
    }

    public void Woosh()
    {
        PlaySingle(wooshEffect);
    }
}
