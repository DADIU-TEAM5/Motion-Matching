using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MotionManager : MonoBehaviour
{
    public PlayerInput PlayerInput;

    // List of all the recorded mo-cap animation
    public List<AnimClip> AnimationClips;

    // List of all frames from the animations, now more fit to motion matching
    public List<MotionClipData> MotionClips;

    public string RootName;

    public MotionFrameVariable NextFrame;
    
    private MotionFrame PlayerMotionFrame;
    
    public float CostWeightPosition, CostWeightVelocity, CostWeightAngle;
    public PlayerSetting playerSetting;

    private float timer;
    private string MotionName = "dash";
    private MotionClipType NowClipType;

    public bool isJump;
    public bool isDash;


    void Awake()
    {
        MotionClips = new List<MotionClipData>();
        for (int i = 0; i < AnimationClips.Count; i++) {
            var clip = AnimationClips[i];
            ExtractMotionClips(clip);
        }
    }

    void Start() {
        //NextFrame.Value = MotionFrames[NextIndex];
        //GoalFrame.Value = MotionFrames[GoalIndex];
        PlayerMotionFrame =new MotionFrame();
    }

    void Update()
    {
        timer += Time.deltaTime;
        // TODO: Update next frame 
        GetNextFrame();
    }



    public void GetNextFrame()
    {
        var calCulateCost = new CalculateCost();
        float bestScore = float.MaxValue;
        int bestScoreClipIndex = 0;
        int bestScoreFrameIndex = 0;
        float normalizedTime = 0;
        MotionClipData motionClipData = MotionClips[0];
        int frameIndex = 0;

        GetPlayerMotion(PlayerInput, timer);

        for (int i =0; i < MotionClips.Count; i++)
        {
            //var normalizedTime = (timer % MotionClips[i].MotionClipLengthInMilliseconds) / MotionClips[i].MotionClipLengthInMilliseconds;
            if (MotionClips[i].ClipType != NowClipType)
                continue;

            for (int j = 0; j < MotionClips[i].MotionFrames.Length; j++)
            {        
                var thisMotionScore = calCulateCost.CalculateAllCost(MotionClips[i].MotionFrames[j],
                                                                     PlayerMotionFrame, 
                                                                     playerSetting);

               
                if (thisMotionScore < Mathf.Epsilon)// Mathf.Epsilon
                    continue;
                if (thisMotionScore < bestScore)
                {
                    bestScore = thisMotionScore;
                    bestScoreClipIndex = i;
                    bestScoreFrameIndex = j;
                    normalizedTime = (timer % MotionClips[i].MotionClipLengthInMilliseconds) / MotionClips[i].MotionClipLengthInMilliseconds;
                    frameIndex = Mathf.FloorToInt(MotionClips[i].MotionFrames.Length * normalizedTime);
                    motionClipData = MotionClips[i];
                }
                
            }
        }
        // Debug.Log("best frame");
        // Debug.Log(bestScoreFrameIndex);
        //PlayerMotionFrame = MotionClips[bestScoreClipIndex].MotionFrames[bestScoreFrameIndex];
        // NextFrame.Value = MotionClips[bestScoreClipIndex].MotionFrames[bestScoreFrameIndex];
        test_playAnimation(frameIndex, motionClipData);
        MotionName = motionClipData.Name;
    }

    private void test_playAnimation(int frameIndex, MotionClipData motionClipData)
    {
        if (MotionName == motionClipData.Name)
        {

            NextFrame.Value = motionClipData.MotionFrames[frameIndex];
        }
        else
        {
            timer = 0;
            NextFrame.Value = motionClipData.MotionFrames[0];
        }
        
    }


    public void GetClipTrajectoryData(MotionFrame frame) {
        frame.TrajectoryDatas = new MotionTrajectoryData[MotionTrajectoryData.Length()];

        for (var i = 0; i < MotionTrajectoryData.Length(); i++) {
            var timeStamp = 1f / (float)(MotionTrajectoryData.Length() - i);

            var trajectoryData = new MotionTrajectoryData();
            //trajectoryData.LocalPosition = 1000000f * frame.Velocity * frame.Direction * timeStamp;
            //trajectoryData.Velocity = 1000000f * frame.Velocity * frame.Direction;
            trajectoryData.LocalPosition = 1000f* frame.Velocity * frame.Direction * timeStamp;
            trajectoryData.Velocity = 1000f * frame.Velocity * frame.Direction;

            if (frame.AngularVelocity != 0f) {
                trajectoryData.Direction = (Quaternion.Euler(0, frame.AngularVelocity * timeStamp, 0) * Vector3.forward).normalized;
            }

            frame.TrajectoryDatas[i] = trajectoryData;
        }
    }

    public void GetPlayerMotion(PlayerInput playerInput, float normalizedTime) {
        var motionFrame = GetBakedMotionFrame(playerInput, normalizedTime);
        PlayerMotionFrame.Velocity = PlayerInput.Velocity;
        //PlayerMotionFrame.Joints = motionFrame.Joints;
        PlayerMotionFrame.Joints = motionFrame.Joints;

        PlayerMotionFrame.TrajectoryDatas = new MotionTrajectoryData[MotionTrajectoryData.Length()];

        for (var i = 0; i < MotionTrajectoryData.Length(); i++) {
            //var timeStamp = 1f / (float) (i+1);  // it is wired
            var timeStamp = 1f / (float)(MotionTrajectoryData.Length() - i);//non-linear

            var trajectoryData = new MotionTrajectoryData();
            //?? local = float velocity * (Vector 3) direction??
            //player trajectory Data
            trajectoryData.LocalPosition = PlayerInput.Velocity * PlayerInput.Direction * timeStamp;//mark the localPosition always a straight line
            trajectoryData.Velocity = PlayerInput.Velocity * PlayerInput.Direction; // velocity for each trajectory is always the same

            //direction based on AngularyVelocity
            if (PlayerInput.AngularVelocity != 0f) {
                trajectoryData.Direction = Quaternion.Euler(0, PlayerInput.AngularVelocity * timeStamp, 0) * Vector3.forward.normalized;
            }

            PlayerMotionFrame.TrajectoryDatas[i] = trajectoryData;
        }
    }

    private MotionFrame GetBakedMotionFrame(PlayerInput playerInput,
                                            float timer)//, MotionClipType motionClipType)
    {
        //does motionFrame have cliptype?
        MotionFrame motionFrame = null;
        for (int i = 0; i < MotionClips.Count; i++)
        {
            MotionClipData motionClipData = MotionClips[i];
            var normalizedTime = (timer % motionClipData.MotionClipLengthInMilliseconds) / motionClipData.MotionClipLengthInMilliseconds;
            if (isJump || isDash)
            {
                if (isJump && motionClipData.Name.Contains("jump"))
                {
                    MotionName = motionClipData.Name;
                    motionFrame = motionClipData.MotionFrames[0];
                    NowClipType = motionClipData.ClipType;
                    break;
                }
                if (isDash && motionClipData.Name.Contains("dash"))
                {
                    MotionName = motionClipData.Name;
                    motionFrame = motionClipData.MotionFrames[0];
                    NowClipType = motionClipData.ClipType;
                    break;
                }
            }
            else if(motionClipData.Name == MotionName)
            {
                int frame = Mathf.FloorToInt(motionClipData.MotionFrames.Length * normalizedTime);
                motionFrame = motionClipData.MotionFrames[frame];//PlayerMotionFrame;
                NowClipType = motionClipData.ClipType;
                break;
            }
    
        }
        return motionFrame;

        /*
         else if (playerInput.Crouch && motionClipData.Name.Contains("crouch"))
        {
            MotionName = motionClipData.Name;
            motionFrame = motionClipData.MotionFrames[0];
            break;
        }
         */

    }



    /*
    private MotionFrame GetBakedMotionFrame(string motionName, float normalizedTime, MotionClipType clipType)
    {
        for (int i = 0; i < MotionClips.Count; i++) {
            var clip = MotionClips[i];
            
            if (clipType != null) {
                if (clipType == clip.ClipType) {
                    return clip.MotionFrames[0];
                }
            } else if (clip.Name.Equals(motionName)) {
                int frameBasedOnTime = Mathf.FloorToInt(clip.MotionFrames.Length * normalizedTime);

                return clip.MotionFrames[frameBasedOnTime];
            }
        }

        return null;
    }
    */

    public void ExtractMotionClips(AnimClip animationClip) {
        var motionClip = new MotionClipData();
        motionClip.Name = animationClip.name;
        //motionClip.MotionClipLengthInMilliseconds = animationClip.ClipLengthInMilliseconds; //no value for animationClip.ClipLengthInMilliseconds
        motionClip.ClipType = animationClip.ClipType;
        motionClip.MotionFrames = new MotionFrame[animationClip.Frames.Count - 10];

        // The very first frame
        var firstMotionFrame = new MotionFrame();
        var firstFrame = animationClip.Frames[0];
        var stubAnimationjointPoint = new AnimationJointPoint { Position = Vector3.zero };

        firstMotionFrame.Joints = (from jp in firstFrame.JointPoints
                                    select MakeMotionJoint(jp, stubAnimationjointPoint)).ToArray();
        foreach (var jt in firstMotionFrame.Joints) {
            jt.BaseRotation = jt.Rotation;
        }

        var rootMotionJoint = firstMotionFrame.Joints.First(x => x.Name.Equals(RootName));
        firstMotionFrame.AngularVelocity = Vector3.Angle(Vector3.forward, rootMotionJoint.Velocity) / 180f;
        firstMotionFrame.Velocity = rootMotionJoint.Velocity.sqrMagnitude;
        firstMotionFrame.Direction = rootMotionJoint.Velocity.normalized;
        firstMotionFrame.Time = firstFrame.Time;
        GetClipTrajectoryData(firstMotionFrame);

        //motionClip.MotionFrames[0] = firstMotionFrame;
        
        // All the other ones
        for (int i = 10; i < animationClip.Frames.Count; i++) {
            var frame = animationClip.Frames[i];
            var lastFrame = animationClip.Frames[i - 1]; 
            var motionFrame = new MotionFrame();

            motionFrame.Time = frame.Time;
            
            var joints = (from jp in frame.JointPoints 
                          from jp2 in lastFrame.JointPoints
                          where jp.Name.Equals(jp2.Name)
                          select MakeMotionJoint(jp, jp2)).ToArray();
            
            foreach (var jt in joints) {
                var firstJt = firstMotionFrame.Joints.First(x => x.Name.Equals(jt.Name));
                jt.BaseRotation = firstJt.Rotation;
            }

            motionFrame.Joints = joints;
        
            var root = joints.First(x => x.Name.Equals(RootName));
            motionFrame.AngularVelocity = Vector3.Angle(Vector3.forward, root.Velocity) / 180f;
            motionFrame.Velocity = root.Velocity.sqrMagnitude;
            motionFrame.Direction = root.Velocity.normalized;
            GetClipTrajectoryData(motionFrame);

            motionClip.MotionFrames[i-10] = motionFrame;
        }

        motionClip.MotionClipLengthInMilliseconds = animationClip.Frames.Last().Time;

        MotionClips.Add(motionClip);
    }

    private MotionJointPoint MakeMotionJoint(AnimationJointPoint current, AnimationJointPoint last) {
        var motionJointPoint = new MotionJointPoint { 
            LocalPosition = current.Position,
            LocalRotation = current.Rotation,
            Rotation = current.Rotation,
            Name = current.Name, 
            Position = current.Position, 
            Velocity = current.Position - last.Position 
        };

        return motionJointPoint;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var trajectoryMagnifier = 2f;

        for (var i = 0; i < PlayerMotionFrame.TrajectoryDatas.Length; i++) {
            var trajectoryData = PlayerMotionFrame.TrajectoryDatas[i];

            Gizmos.DrawCube(PlayerInput.Position + trajectoryData.LocalPosition * trajectoryMagnifier, new Vector3(.1f, .1f, .1f));
        }

        Gizmos.color = Color.red;

        Debug.Log(NextFrame.Value.TrajectoryDatas.Length);
        for (var i = 0; i < NextFrame.Value.TrajectoryDatas.Length; i++) {
            var trajectoryData = NextFrame.Value.TrajectoryDatas[i];

            Debug.Log($"Frame: {trajectoryData.LocalPosition}");

            Gizmos.DrawCube(PlayerInput.Position + new Vector3(trajectoryData.LocalPosition.x, 0, trajectoryData.LocalPosition.z) * trajectoryMagnifier, new Vector3(.1f, .1f, .1f));
        }
        
    }
}
