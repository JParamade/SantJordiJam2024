using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimController : MonoBehaviour
{
    private Animator dragonAnim;

    private void Awake()
    {
        dragonAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Random.value > 0.9995f) {
            if (Random.value > 0.5f) { dragonAnim.SetTrigger("Roar"); }
            else { dragonAnim.SetTrigger("Shake"); }
        }
    }
}
