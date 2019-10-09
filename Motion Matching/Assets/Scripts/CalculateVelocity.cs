using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateVelocity : MonoBehaviour
{
    public float CostP;
    public float CostV;
    public float CostV2;
    public float CostTheta;
    float Pointangle(Transform PositionCurrent, Transform PositionLast)
    {
        float angle = Vector3.Angle(PositionCurrent.forward, PositionLast.forward);
        //Debug.Log(angle);
        return angle;
    }

    Vector3 PointVelocity(Vector3 PositionCurrent, Vector3 PositionLast)
    {
        Vector3 velocity = PositionCurrent - PositionLast;
        return velocity;
    }

    float CalculateCost(MotionFrame CurrentFrame, MotionFrame GoalFrame)
    {
        return 0f;
    }

    float CalculateOneJointCost(Vector3 CurrentP, Vector3 GoalP, 
        Vector3 CurrentV, Vector3 GoalV, float CurrentTheta, float GoalTheta)
    {
        CostP = (CurrentP - GoalP).sqrMagnitude;
        CostV = Vector3.Angle(CurrentV, GoalV);
        //do we need this?
        CostV2 = Vector3.Angle(CurrentP - GoalP, GoalV);
        CostTheta = Mathf.Abs(CurrentTheta - GoalTheta);
        return (CostP+CostV+CostV2+CostTheta)/4;
    }
}
