using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public int idle, walk, giveHand, run, win, lose;
    public bool giveHandComplete, giveHandPlaying;
    public Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        idle = Animator.StringToHash("IDLE");
        walk = Animator.StringToHash("WALK");
        giveHand = Animator.StringToHash("GIVEHAND");
        run = Animator.StringToHash("RUN");
        win = Animator.StringToHash("WIN");
        lose = Animator.StringToHash("SAD");
    }

    public void PlayWalk()
    {
        anim.Play(walk);
    }

    public void PlayRun()
    {
        anim.Play(run);
    }

    public void PlayEndAnimation(WinState winAgent)
    {
        switch (winAgent)
        {
            case WinState.win:
                anim.Play(win);
                break;
            case WinState.lose:
                anim.Play(lose);
                break;
        }
    }

    public void PlayGiveHand2()
    {
        if (!giveHandPlaying)
        {
            giveHandPlaying = true;
            anim.Play(giveHand);
            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    giveHandComplete = true;
                    break;
                }
                else
                {
                    Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }

            }
            giveHandPlaying = false;
        }
        
    }

    public void PlayGiveHand()
    {
        anim.Play(giveHand);
    }

    public void PlayIdle()
    {
        anim.Play(idle);
    }
}
