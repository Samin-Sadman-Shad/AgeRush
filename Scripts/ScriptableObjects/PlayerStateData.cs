using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateData", menuName = "StateData/PlayerData")]
public class PlayerStateData : ScriptableObject
{
    public PlayerState state;
    public Transform model;
    public Color color;
    public Vector3 Scale;
    public float scaleMul;
    public int stateValue;
}
