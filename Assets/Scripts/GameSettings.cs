using System;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Scene Settings")]
    public Vector3[] carPositions = {
        new Vector3(-7.28999996f,0.720000029f,0),
        new Vector3(-6.78000021f,0.409999996f,0),
        new Vector3(-6.21999979f,0.150000006f,0),
        new Vector3(-7.32000017f,-0.119999997f,0),
        new Vector3(-6.78000021f,-0.430000007f,0),
        new Vector3(-6.21999979f,-0.74000001f,0),
        new Vector3(-7.32000017f,-0.949999988f,0),
        new Vector3(-6.78000021f,-1.24000001f,0),
    };
    
    [Header("User Car Settings")]
    [Tooltip("0 means no movement")]
    public float accelerationFactor = 100.0f;

    [Tooltip("Range: [0, 1]. 0 = no drift. 1 = no traction")]
    public float driftFactor = 0.1f;

    [Tooltip("How fast to turn. 0 = no turning")]
    public float turnFactor = 3.0f;

    [Tooltip("Coefficient in the drag calculation. 0 = no drag")]
    public float dragFactor = 0.15f;

    [Tooltip("Base rolling resistance. 0 = slip and slide")]
    public float dragAdjustment = 5.0f;

    [Tooltip("Maximum Speed. 0 = no movement. May not apply as speed is also bound by drag")]
    public float maxSpeed = 5.0f;
    
    [Header("Game Settings")]
    [Tooltip("Time in seconds between loss of a life.")]
    public float crashCooldown = 2;
}