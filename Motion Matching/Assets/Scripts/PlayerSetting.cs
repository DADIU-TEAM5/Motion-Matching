using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSetting
{
    //Bone Factors
    public float BoneRotFactor;
    public float BonePosFactor;

    //
    public float RootMotionCostFactor;

    //trajectory things
    public float trajectoryPosFactor;
    public float trajectoryRotFactor;
    public float trajectoryVelFactor;
}

