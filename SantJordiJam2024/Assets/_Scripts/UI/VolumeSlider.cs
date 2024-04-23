using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] VolumeType volumeType;

    private Slider volumeSlider;
    private AudioManager audioManager;

    private void Awake() {
        volumeSlider = this.GetComponent<Slider>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update() {
        if (volumeType == VolumeType.MASTER) { volumeSlider.value = audioManager.masterVolume; }
        else if (volumeType == VolumeType.MUSIC) { volumeSlider.value = audioManager.musicVolume; }
        else if (volumeType == VolumeType.SFX) { volumeSlider.value = audioManager.sfxVolume; }
    }

    public void OnSliderValueChanged() {
        if (volumeType == VolumeType.MASTER) { audioManager.masterVolume = volumeSlider.value; }
        else if (volumeType == VolumeType.MUSIC) { audioManager.musicVolume = volumeSlider.value; }
        else if (volumeType == VolumeType.SFX) { audioManager.sfxVolume = volumeSlider.value; }
    }
}
