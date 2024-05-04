using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip soundtrack;
    public AudioClip[] deathSounds;
    public AudioClip[] footsteps;

    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public AudioClip GetDeathSound()
    {
        return deathSounds[Random.Range(0, deathSounds.Length)];
    }

    public AudioClip GetFootstepSound()
    {
        return footsteps[Random.Range(0, footsteps.Length)];
    }

    /// <summary>
    /// Plays an Audio Clip with adjusted pitch values
    /// </summary>
    /// <param name="_clip">The clip to play</param>
    /// <param name="_source">The audio source to play from</param>
    public void PlaySound(AudioClip _clip, AudioSource _source, float _time = 0f, float _volume = 1f)
    {
        if (_source == null || _clip == null)
            return;

        Debug.Log("SoundPlayed");

        _source.clip = _clip;
        _source.time = _time;
        _source.volume = _volume;
        _source.Play();
    }

    public void PlayDeathSound(AudioSource _source, float _volume = 1f)
    {
        PlaySound(GetDeathSound(), _source, 0f, _volume);
    }
}
