using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class AnimationManager : MonoBehaviour
{

    public Transform Skeleton;

    private Dictionary<string, Transform> SkeletonJoints = new Dictionary<string, Transform>();


    public MotionFrameVariable NextFrame;

    void Awake() {
        
    }

    void Update() {
        //ApplyFrameToJoints(NextFrame.Value.AnimationFrame);
        if (NextFrame.Value != null) ApplyFrameToJoints(NextFrame.Value);
    }

    void Start() {
        GetAllChildren(Skeleton);
    }

    public void ApplyFrameToJoints(MotionFrame frame) {
        //Debug.Log(frame.Velocity);
        foreach (var jointPoint in frame.Joints) {
            if (!SkeletonJoints.Keys.Contains(jointPoint.Name)) {
                //Debug.LogError($"{jointPoint.Name} is not in the {Skeleton.name}");
                continue;
            }

            var joint = SkeletonJoints[jointPoint.Name];
            ApplyJointPointToJoint(jointPoint, joint);             
        } 
    }


    private void ApplyJointPointToJoint(MotionJointPoint jointPoint, Transform joint) {
        // Based on negative joint
        var newEulerRot = jointPoint.Rotation * Quaternion.Inverse(jointPoint.BaseRotation);
        //var newEulerRot = jointPoint.Rotation * jointPoint.BaseRotation;
        //joint.rotation = newEulerRot;
        joint.rotation = Skeleton.rotation * jointPoint.LocalRotation;
        //joint.rotation = Skeleton.rotation * (newEulerRot);
        joint.position = Skeleton.position + jointPoint.Position;

        //joint.SetPositionAndRotation(jointPoint.Position, jointPoint.Rotation);
    }

    private void GetAllChildren(Transform trans) {
        foreach (Transform child in trans) {
            if (child.childCount > 0) GetAllChildren(child);
            SkeletonJoints.Add(child.name, child);
        }
    }
}
