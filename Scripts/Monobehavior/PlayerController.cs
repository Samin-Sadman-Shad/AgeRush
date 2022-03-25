using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InputController inputScript;

    public PlayerState currentState;
    public PlayerStateData currentStateData;
    public PlayerAnimationController animScript;

    [SerializeField] Transform _tr, levelEnd, character, rayPoint;
    Rigidbody rb;

    [SerializeField]
    float maxAngle, angleScale;

    [SerializeField]
    float forwardVel, maxRayDist;

    [SerializeField]
    float keyBoardSide, sideDelta, sideDeltaMul, sideSpeedMul, sideLerpMul, radius;

    [SerializeField]
    float forwardLerpMul, verticalVel, lookAtSpeed;

    [SerializeField] Quaternion Qt;

    [SerializeField]
    bool isLevelStart, isLevelPaused, isLevelEnd;

    [SerializeField]
    int currentPoint, maxPoint;

    [SerializeField] TextMeshProUGUI stateText, playerNumber;
    [SerializeField] Slider slider;
    [SerializeField] ParticleSystem playerEffect, pointUpEffect;

    Vector3 primaryScale;
    Material mat;
    Color matColor;

    public bool isPlayerDead, playWalkingAnimation;
    public Dictionary<PlayerState, PlayerStateData> stateToDataMap = new Dictionary<PlayerState, PlayerStateData>();
    public List<PlayerStateData> stateDataList = new List<PlayerStateData>();

    void Start()
    {
        if(_tr == null)
        {
            _tr = transform; //if transform is not assigned from eitor
        }
        
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }

        GameManager.game.LevelStartEv += LevelStarter;
        GameManager.game.LevelPauseEV += LevelPauser;
        GameManager.game.LevelEndEV += LevelEnder;
        CreateMap();

        animScript = GetComponent<PlayerAnimationController>();
        if(animScript == null)
        {
            animScript = GetComponentInChildren<PlayerAnimationController>();
        }
        animScript.PlayIdle();
        if(stateText != null)
        {
            stateText.SetText(currentState.ToString());
        }

        ChangeStateData(currentState);

        currentPoint = 1;
        primaryScale = _tr.localScale;
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        matColor = currentStateData.color;
        mat.color = matColor;

        climb = GetComponent<WalkingOnStair>().walk;
    }

    void CreateMap()
    {
        if(stateDataList != null && stateDataList.Count > 0)
        {
            for (int i = 0; i < stateDataList.Count; i++)
            {
                var key = stateDataList[i].state;
                var value = stateDataList[i];
                //KeyValuePair<PlayerState, PlayerStateData> pair = new KeyValuePair<PlayerState, PlayerStateData>(key, value);
                if (!stateToDataMap.ContainsKey(key))
                {
                    stateToDataMap.Add(key, value);
                }
                
            }
        }
    }

    //private void OnEnable()
    //{
    //    if (inputScript == null)
    //    {
    //        inputScript = GetComponent<InputController>();
    //    }
    //    //inputScript.responseEvent += PlayerResponseToInput;
    //}

    RaycastHit hit;
    bool climb;
    void FixedUpdate()
    {
        climb = GetComponent<WalkingOnStair>().walk;
        if (isLevelStart && !isLevelEnd && !isLevelPaused && !climb)
        {
            if (!isPlayerDead)
            {
                InputSystem();
                ResponseToInputSystem();

                if(animScript.anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animScript.walk && playWalkingAnimation)
                {
                    //animScript.PlayWalk();
                    animScript.PlayRun();
                }

                //raycast to detect other ai
                Ray ray = new Ray(_tr.position, _tr.forward);
                Debug.DrawRay(rayPoint.position, _tr.forward * maxRayDist, Color.red, 1);

                //if (Physics.Raycast(ray, out hit, maxRayDist))
                if(Physics.SphereCast(ray, radius, out hit, maxRayDist))
                {
                    AIData currentAI = hit.transform.GetComponent<AIData>();
                    if (currentAI == null)
                    {
                        currentAI = hit.transform.GetComponentInParent<AIData>();
                    }
                    if (currentAI == null) { return; }
                    //play touching animation for player
                    //player acquires the ai's data
                    ChangeState(currentAI);
                }

                //if(_tr.position.z > levelEnd.position.z)
                //{
                //    LevelEnder();
                //}
            }
            else
            {
                Debug.Log("player is dead");
                GameManager.game.result = WinState.lose;
                isLevelEnd = true;
                //LevelEnder();
            }

            if (currentState < 0)
            {
                isPlayerDead = true;
            }

        }

        if (isLevelPaused)
        {
            rb.velocity = Vector3.zero;
        }

        //if (isLevelEnd && !isPlayerDead)
        //{
        //    if(currentState > PlayerState.Infant)
        //    {
        //        GameManager.game.result = WinState.win;
        //    }
        //    else
        //    {
        //        GameManager.game.result = WinState.lose;
        //    }
            
        //    isLevelEnd = true;
        //    //LevelEnder();
        //}
    }

    void InputSystem()
    {
        //For PC
        sideDelta = 0.0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            sideDelta = -keyBoardSide;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            sideDelta = keyBoardSide;
        }

        //For Mobile
        foreach(var touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Moved)
            {
                var delta = touch.deltaPosition;
                sideDelta = delta.x / (float)Screen.width;
                sideDelta *= sideDeltaMul;
                break;
            }
        }
    }

    void ResponseToInputSystem()
    {
        //forward and transverse velocity
        var velWrld = rb.velocity;
        //Debug.Log("world velocity " + velWrld);
        var vel = transform.InverseTransformVector(velWrld); // velocity is converted to local
        //Debug.Log("local velocity " + vel);
        vel.x = Mathf.Lerp(vel.x, sideSpeedMul * sideDelta, Time.deltaTime * sideLerpMul);
        vel.z = Mathf.Lerp(vel.z, forwardVel, forwardLerpMul * Time.deltaTime);
        if(Mathf.Approximately(sideDelta, 0))
        {
            vel.x = 0f;
        }

        vel.y = rb.velocity.y ;
        var convertVel = transform.TransformVector(vel);
        //convertVel.y = verticalVel;
        //convertVel.y = rb.velocity.y;
        rb.velocity = convertVel;

        //rotation part
        var tDir = vel;
        tDir.x = Mathf.Lerp(tDir.x, 0, 7f * Time.deltaTime); // make transverse velocity zero gradually and create target rotation

        var dir = transform.TransformVector(tDir);
        dir.y = 0;
        dir = dir.normalized; // the x and y component of dir is zero
        if (Mathf.Approximately(dir.magnitude, 0.0f)) { return; }
        var targetRot = Quaternion.LookRotation(dir); 
        _tr.rotation = Quaternion.Slerp(_tr.rotation, targetRot, lookAtSpeed * Time.deltaTime); //make a rotation from current rotation to target one by slerp
    }

    public void ChangeState(AIData ai)
    {
        if (!ai.alreadyChecked)
        {
            Debug.Log("touching an ai " + ai.gameObject.name);
            //ai.alreadyChecked = true;
            playWalkingAnimation = false;
            if(!ai.allowedToPick && !ai.allowedToCollide)
            {
                animScript.PlayGiveHand();
            }
            else
            {
                if (ai.allowedToPick)
                {
                    pointUpEffect.Play();
                }
            }

            //update the player appearance according to the state
            if (!ai.allowedToPick && !ai.allowedToCollide)
            {
                if (currentState == ai.state)
                {
                    Debug.Log("correct ai");
                    currentState = currentState + ai.stateUpdate;
                    Debug.Log("player improved to " + currentState);
                    slider.value = SliderController(currentState);
                   // playerNumber.SetText(Convert.ToInt32(ai.number.text).ToString());
                }
                else
                {
                    Debug.Log("wrong ai");
                    currentState = currentState - ai.stateUpdate;
                    Debug.Log("player downgraded to " + currentState);
                    slider.value = SliderController(currentState);
                }

                if (currentState < 0)
                {
                    isPlayerDead = true;
                }
                else
                {
                    ChangeStateData(currentState);
                }

                StartCoroutine(ResumeRun(ai));
                StartCoroutine(PlayerTextUpdate(ai));

                //animScript.PlayWalk();
                //playerNumber.SetText(Convert.ToInt32(ai.number.text).ToString());
                //playerNumber.SetText(Convert.ToInt32(ai.stateData.stateValue).ToString());
            }
            else
            {
                if (ai.allowedToPick)
                {
                    currentPoint *= 2;
                    if (currentPoint >= maxPoint)
                    {
                        currentState = currentState + ai.stateUpdate;
                        ChangeStateData(currentState);
                        StartCoroutine(PlayerTextUpdate(ai));
                        UpdateAppearance();
                        currentPoint = 0;
                    }

                    ai.gameObject.SetActive(false);
                }
                else if(ai.allowedToCollide)
                {
                    if (!ai.alreadyChecked)
                    {
                        currentState = currentState - ai.stateUpdate;
                        ChangeStateData(currentState);
                        StartCoroutine(PlayerTextUpdate(ai));
                        UpdateAppearance();
                        ai.alreadyChecked = true;
                    }
                    
                }
                
            }
            ai.alreadyChecked = true;
        }
    }

    IEnumerator ResumeRun(AIData ai)
    {
        LevelPauser();
        //playerEffect.Play();
        yield return new WaitForSeconds(1f);
        playWalkingAnimation = true;
        if(currentState >= PlayerState.Newborn)
        {
            StartCoroutine(MergingOperation(ai)); //the last ai before death will not disappear
        }
        yield return new WaitForSeconds(0.7f); // will be less than the effect duration, so that player starts to run before finishing the effect
        LevelRestarter();
    }

    IEnumerator MergingOperation(AIData ai)
    {
        //ai.effect.Play();
        playerEffect.Play();
        yield return new WaitForSeconds(0.2f);
        ai.gameObject.SetActive(false);
        character.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.6f); //0.4
        character.gameObject.SetActive(true);
        UpdateAppearance();
    }

    void UpdateAppearance()
    {
        _tr.localScale = currentStateData.scaleMul * primaryScale;
        matColor = currentStateData.color;
        mat.color = matColor;
    }

    IEnumerator PlayerTextUpdate(AIData ai)
    {
        if(!ai.allowedToCollide && !ai.allowedToPick)
        {
            yield return new WaitForSeconds(1.5f);
        }
        
        if (!(currentState > PlayerState.Centenarian) && !(currentState < PlayerState.Newborn))
        {
            stateText.SetText(currentState.ToString());
        }
        else if (currentState > PlayerState.Centenarian)
        {
            stateText.SetText("Reach Maximum");
        }
        else if (currentState < PlayerState.Newborn)
        {
            stateText.SetText("Dead!");
        }
        playerNumber.SetText(Convert.ToInt32(currentStateData.stateValue).ToString());
    }

    void ChangeStateData(PlayerState state)
    {
        if (stateToDataMap.ContainsKey(state))
        {
            var stateData = stateToDataMap[state];
            currentStateData = stateData;
        }
    }

    public float SliderController(PlayerState currentState)
    {
        float value = (float)currentState / 7.0f;
        return value;
    }

    void LevelStarter()
    {
        isLevelStart = true;
        playWalkingAnimation = true;
    }

    void LevelPauser()
    {
        isLevelPaused = true;
    }

    void LevelRestarter()
    {
        isLevelPaused = false;
    }

    void LevelEnder()
    {
        isLevelEnd = true;
        StartCoroutine(LevelEndOperation(GameManager.game.result));
    }

    IEnumerator LevelEndOperation(WinState winAgent)
    {
        rb.velocity = Vector3.zero;
        playWalkingAnimation = false;
        RotateAtEnd();
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(winAgent);
        animScript.PlayEndAnimation(winAgent);
    }

    void RotateAtEnd()
    {
        var targetRot = Quaternion.LookRotation(character.forward * (-1));
        //_tr.rotation = Quaternion.Slerp(_tr.rotation, targetRot, lookAtSpeed * Time.deltaTime);
        character.DORotateQuaternion(targetRot, 1f);
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rayPoint.position, radius);
    }
}
