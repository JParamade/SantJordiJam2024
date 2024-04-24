using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public List<Camera> cameraList = new List<Camera>();
    public List<Canvas> canvasList = new List<Canvas>();

    public void SelectCamera(int index) {
        foreach (Camera c in cameraList) {
            if (cameraList[index] == c) { 
                c.gameObject.SetActive(true);
                c.enabled = true;
            }
            else { 
                c.enabled = false;
                c.gameObject.SetActive(false);
            }
        }

        foreach (Canvas c in canvasList) {
            if (canvasList[index] == c) { c.gameObject.SetActive(true); }
            else { c.gameObject.SetActive(false); }
        }
    }
}
