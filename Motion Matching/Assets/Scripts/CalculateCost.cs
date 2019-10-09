using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CalculateCost 
{

    public float CalculateAllCost(MotionFrame motionFrame, MotionFrame currentFrame,
                                    PlayerSetting playerSetting)
    {
        float allCost = 0;
        for(int j = 0; j< motionFrame.Joints.Length; j ++)
        {
            allCost += BoneCost(motionFrame.Joints[j], currentFrame.Joints[j], playerSetting);
        }
        allCost += RootMotionCost(motionFrame, currentFrame, playerSetting);
        allCost += TrajectoryCost(motionFrame, currentFrame, playerSetting);
        
        return allCost;
    }
       // the frame is from clips
    private float RootMotionCost(MotionFrame frame, MotionFrame current, 
        PlayerSetting playerSetting)
    {
        var velocity = Mathf.Abs(frame.Velocity - current.Velocity);
        return (playerSetting.RootMotionCostFactor * velocity);
    }


    //frameBone is the bone we look at, which is from animation clips
    private float BoneCost(MotionJointPoint frameBone, MotionJointPoint currentBone, 
                            PlayerSetting playerSetting)
    {
        var rotationCost = RotationCost(frameBone, currentBone);
        var posCost = PosCost(frameBone, currentBone);
        return playerSetting.BoneRotFactor * rotationCost + playerSetting.BonePosFactor * posCost;
    }

    private float PosCost(MotionJointPoint frameBone, MotionJointPoint currentBone)
    {
        var posCost = (frameBone.LocalPosition - currentBone.LocalPosition).sqrMagnitude;
        return posCost;
    }

    private float RotationCost(MotionJointPoint frameBone, MotionJointPoint currentBone)
    {
        var bonePosRotation = Quaternion.Inverse(frameBone.LocalRotation) * currentBone.LocalRotation;
        var rotationCost = Mathf.Abs(bonePosRotation.x) + Mathf.Abs(bonePosRotation.x)
                        + Mathf.Abs(bonePosRotation.y) + (1 - Mathf.Abs(bonePosRotation.w));
        return rotationCost;
    }

    
    private float TrajectoryCost(MotionFrame frame, MotionFrame current,
     PlayerSetting playerSetting)
    {
        float trajectoryCost = 0;
        for(int i = 0; i < frame.TrajectoryDatas.Length; i++)
        {
            //position cost
            var traPos = frame.TrajectoryDatas[i].LocalPosition - current.TrajectoryDatas[i].LocalPosition;
            var traPosCost = traPos.sqrMagnitude * playerSetting.trajectoryPosFactor;

            //rotation cost
            var traRot = Vector3.Dot(frame.TrajectoryDatas[i].Direction,current.TrajectoryDatas[i].Direction);
            var traRotCost = traRot * playerSetting.trajectoryRotFactor;

            //velocity cost
            var traVel = frame.TrajectoryDatas[i].Velocity - current.TrajectoryDatas[i].Velocity;
            var traVelCost = traVel.sqrMagnitude * playerSetting.trajectoryVelFactor;

            trajectoryCost += (traPosCost + traRotCost + traVelCost);
        }

        return trajectoryCost;
    }
}
