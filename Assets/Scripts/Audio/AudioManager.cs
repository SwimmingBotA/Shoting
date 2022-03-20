using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] AudioSource xFXPlayer;


    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    //UI“Ù–ß
    public void PlaySFX(AudioData audioData)
    {
        xFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    //◊”µØ“Ù–ß
    public void PlayRandomSFX(AudioData audioData)
    {
        xFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioData)
    {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }
}

[System.Serializable]public class AudioData
{
    public AudioClip audioClip;
    public float volume;
}
