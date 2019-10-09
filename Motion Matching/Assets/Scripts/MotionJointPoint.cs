using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MotionJointPoint 
{
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;

    public Vector3 Velocity;


    // DEBUG
    public string Name;
    public Vector3 Position;
    public Quaternion Rotation;
    public Quaternion BaseRotation;
}
