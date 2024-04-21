using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnSliderValueChanged : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake() {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    public float OnVolumeChanged()
    {
        return volumeSlider.value;

        // if (volumeType == VolumeType.MASTER) { return volumeSlider.value; }
        // else if (volumeType == VolumeType.MUSIC) { return volumeSlider.value; }
        // else if (volumeType == VolumeType.SFX) { return volumeSlider.value; }
    }
}
