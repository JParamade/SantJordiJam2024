using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)] public float masterVolume = 1.0f;
    [Range(0, 1)] public float musicVolume = 1.0f;
    [Range(0, 1)] public float sfxVolume = 1.0f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private MenuCanvasController menuCanvasController;

    private void Awake() {        
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SoundFx");

        menuCanvasController = FindObjectOfType<MenuCanvasController>();
    }
}
