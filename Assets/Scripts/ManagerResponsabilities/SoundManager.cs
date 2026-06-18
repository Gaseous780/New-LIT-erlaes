using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip[] musics;

    [SerializeField] private const float volumeMusicOnMinigames = 0.1f;
    [SerializeField] private float defaultMusicValue = 1f;

    [SerializeField] private AudioClip[] audiosToListen;

    private Dictionary<int, IEnumerator> soundsReproducing;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundsReproducing = new Dictionary<int, IEnumerator>();
    }

    private void OnDisable()
    {
        audioSource.Stop();
        musicSource.Stop();
        StopAllCoroutines();
        soundsReproducing.Clear();
    }

    public void ReproduceMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();

        musicSource.loop = true;
    }

    public void ReproduceMusic (int indexMusic)
    {
        musicSource.clip = musics[indexMusic];
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ChangeVolumeMusic(float volume = volumeMusicOnMinigames)
    {
        musicSource.volume = volume;
    }

    public void SetDefaultVolumeMusic()
    {
        musicSource.volume = defaultMusicValue;
    }


    public void DefineMusicOfScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.buildIndex)
        {
            case 1:
                ReproduceMusic(0);
                break;

            case 4:
                ReproduceMusic(0);
                break;

            default:
                StopMusic();
                break;
        }
    }


    public void ReproduceSound(int soundToReproduce) //Cada int es un sonido a reproducir
    {
        audioSource.PlayOneShot(audiosToListen[soundToReproduce]);
    }

    public void ReproduceSound(AudioClip audio)
    {
        audioSource.clip = audio;

        audioSource.PlayOneShot(audio);
    }

    public void ReproduceChainSounds(int firstSound, int lastSound, float interval = 0)
    {
        if (soundsReproducing.ContainsKey(firstSound) == true || audiosToListen.Length <= firstSound)
        {
            return;
        }

        soundsReproducing.Add(firstSound, ReChain(firstSound, lastSound, interval));
        StartCoroutine(soundsReproducing[firstSound]);
    }

    public void ReproduceChainSounds(AudioClip[] soundsToReproduce, float interval = 0)
    {
        int numberOfIndex = ObtainIndex(soundsToReproduce[0]);

        if (soundsToReproduce.Length < 1 || numberOfIndex == -1) { return; }

        if (soundsReproducing.ContainsKey(numberOfIndex) == true) { return; }

        soundsReproducing.Add(numberOfIndex, ReChain(soundsToReproduce, numberOfIndex, interval));
        StartCoroutine(soundsReproducing[numberOfIndex]);
    }

    private IEnumerator ReChain(int firstSound, int lastSound, float interval = 0)
    {
        for (int i = firstSound; i <= lastSound; i++)
        {
            audioSource.PlayOneShot(audiosToListen[i]);

            yield return new WaitForSeconds(interval);
        }

        soundsReproducing.Remove(firstSound);
    }

    private IEnumerator ReChain(AudioClip[] soundsToReproduce, int index, float interval = 0)
    {
        for (int i = 0; i < soundsToReproduce.Length - 1; i++)
        {
            audioSource.PlayOneShot(soundsToReproduce[i]);

            yield return new WaitForSeconds(interval);
        }

        soundsReproducing.Remove(index);
    }

    public void CancelChain (int firstAudio)
    {
        if (soundsReproducing.ContainsKey (firstAudio) == false) { return; }
        StopCoroutine(soundsReproducing[firstAudio]);
        soundsReproducing.Remove(firstAudio);
    }

    public void CancelChain (AudioClip firstSound)
    {
        if (firstSound == null) { return; }

        int firstAudio = ObtainIndex(firstSound);

        if (soundsReproducing.ContainsKey(firstAudio) == false || firstAudio == -1) { return; }
        StopCoroutine(soundsReproducing[firstAudio]);
        soundsReproducing.Remove(firstAudio);
    }

    private int ObtainIndex (AudioClip audioHint)
    {
        for (int i = 0; i < audiosToListen.Length - 1; i++)
        {
            if (audiosToListen[i] == audioHint)
            {
                return i;
            }
        }

        return -1;
    }
}
