using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public event EventHandler CameraAnimationEnded;

    public void AnimationEnded() {
        CameraAnimationEnded.Invoke(this, EventArgs.Empty);
    }
}
