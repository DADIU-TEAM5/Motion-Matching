using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimationJointPoint 
{
    public Vector3 Position;
    public Quaternion Rotation;
    public string Name;

    public Quaternion BaseRotation;
}
