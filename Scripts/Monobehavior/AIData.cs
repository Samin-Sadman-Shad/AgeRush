using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//this script is responsible for interacting with player and change it's state
public class AIData : MonoBehaviour
{
    public Transform model;
    public Vector3 scale, position;
    public Color color;
    public PlayerState state;
    public PlayerStateData stateData;
    public int stateUpdate = 1;
    public bool allowedToTouch, allowedToPick, allowedToCollide;
    public bool alreadyChecked;
    public bool isMoving = false;
    public float moveSpeed;
    public ParticleSystem effect;
    public TextMeshProUGUI number;
    public TextMeshProUGUI stateName;

    public float moveRange;

    private void Start()
    {
        if(effect == null)
        {
            effect = GetComponentInChildren<ParticleSystem>();
        }

        if(number != null && stateName != null)
        {
            number.SetText(stateData.stateValue.ToString());
            stateName.SetText(state.ToString());
        }

    }

    private void Update()
    {
        if (isMoving)
        {
            //move the ai
            transform.position = new Vector3(Mathf.PingPong(Time.time * 4f, (transform.position.x+moveRange) - (transform.position.x - moveRange)) , transform.position.y, transform.position.z);
        }
    }
}
