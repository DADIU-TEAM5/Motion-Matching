using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInput : ScriptableObject
{
    public float Velocity;
    public bool Jump;
    public bool Dash;

    public Vector3 Direction;
    public float AngularVelocity;

    public Vector3 Position;
}
