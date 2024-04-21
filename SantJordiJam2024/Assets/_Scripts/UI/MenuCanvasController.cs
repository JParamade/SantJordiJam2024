using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MenuCanvasController : MonoBehaviour
{
    private Animator cmpAnimator;
    private Animator cmpCameraAnimator;

    public GameObject[] panels;
    private int panelSelector = 0;

    private bool startTimer = false;
    private float timerTime = 0.1f;

    private void Start() {
        cmpAnimator = GetComponent<Animator>();
        cmpCameraAnimator = Camera.main.GetComponent<Animator>();
    }

    private void Update() {
        if (startTimer) {
            timerTime -= Time.deltaTime;

            if (timerTime <= 0) {
                SetActiveUI();
            }
        }
    }

    public void GameState(bool gameState) {
        cmpAnimator.SetInteger("page", 0);
        cmpAnimator.SetBool("startGame", gameState);
    }

    public void SwapPage(int pageNumber) {
        cmpAnimator.SetInteger("page", pageNumber);
    }

    public void PanelSelector(int panel) { panelSelector = panel; }
    
    public void SetTimer() {
        startTimer = true;
    }

    public void SetActiveUI() {
        startTimer = false;
        timerTime = 0.1f;

        if (cmpAnimator.GetCurrentAnimatorStateInfo(0).speed < 0) {
            foreach (GameObject panel in panels) {
                panel.SetActive(false);
            }
        }
        else { panels[panelSelector].SetActive(true); }
    }

    public void BookOpened() {
        cmpCameraAnimator.SetBool("zoom", cmpAnimator.GetCurrentAnimatorStateInfo(0).speed > 0 ? true : false);
    }

    
}