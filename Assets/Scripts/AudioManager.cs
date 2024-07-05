using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public SoundEffects[] _soundEffects;
    public AudioSource _soundSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySFX(string name)
    {
        SoundEffects s = Array.Find(_soundEffects, x => x.name == name);
        _soundSource.PlayOneShot(s.clip);
    }
}
