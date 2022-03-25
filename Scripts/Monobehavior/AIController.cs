using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] PlayerState aiState;
    PlayerAnimationController animScript;

    void Start()
    {
        animScript = GetComponentInChildren<PlayerAnimationController>();

        if(animScript != null)
        {
            //animScript.PlayIdle();
            var speed = Random.Range(0.8f, 1.8f);
            //Debug.Log(speed);
            animScript.PlayGiveHand();
            animScript.anim.speed = speed;
        }
    }


    void Update()
    {
        //animScript.PlayGiveHand();
    }
}
