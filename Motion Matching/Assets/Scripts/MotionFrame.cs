using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MotionFrame  
{
    public float Velocity;

    public float Time;

    public float AngularVelocity;
    
    public Vector3 Direction;

    public MotionJointPoint[] Joints;
    //trajectory data ---
    public MotionTrajectoryData[] TrajectoryDatas;
}

