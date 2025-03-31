using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    #region Audio Sources
    public AudioSource mainCameraSource;
    #endregion

    #region Audio Clips
    public AudioClip[] speakingSounds;
    
    public AudioClip buttonPress;
    public AudioClip counterUp;
    public AudioClip timerSound;
    public AudioClip smash;
    public AudioClip walking;
    public AudioClip pickup;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mainCameraSource = Camera.main.GetComponent<AudioSource>();
    }

    public void PlaySoundOnMainSource(AudioClip clip)
    {
        mainCameraSource.PlayOneShot(clip);
    }

    public void PlayRandomSoundOnMainSource(AudioClip[] clip)
    {
        int index = Random.Range(0, clip.Length);
        mainCameraSource.PlayOneShot(clip[index]);
    }

    public void PlaySoundOnSpecificSource(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlaySoundRandonPitchMain(AudioClip clip)
    {
        float index = Random.Range(-3, 3);
        mainCameraSource.pitch = index;
        mainCameraSource.PlayOneShot(clip);
        mainCameraSource.pitch = 1;
    }
}
