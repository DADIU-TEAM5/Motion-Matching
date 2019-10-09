using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class csvReader2 : MonoBehaviour
{
    public Transform rokokoSkeleton;
    List<string> rokokoBones = new List<string>();
    public List<Frame2> animations = new List<Frame2>();
    public bool done2 = false;


    float timer;
    static int currentFrame = 0;
    List<Transform> listOfAllBones = new List<Transform>();

    List<Transform> startBones = new List<Transform>();
    string csvfile =
        @"C:\Users\dadiu\Documents\DADIU\Git\MiniGame1_New\scene-1\take-1_MIXAMO_I99.csv";

    // Start is called before the first frame update
    void Start()
    {
        GetAllChildren(rokokoSkeleton, listOfAllBones);
        streamReader();

        
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        Frame2 thisframe = new Frame2();

        if(done2 && (currentFrame < animations.Count))
        {
            thisframe = animations[currentFrame++];
            //could be updated by using an easy way
            //here just find the joint name and give its values
            giveJointValues(thisframe);
        }
        
    }

    private void giveJointValues(Frame2 thisframe)
    {
        for (int i = 0; i < listOfAllBones.Count; i++)
        {
            for (int j = 0; j < thisframe.joints.Length; j++)
            {
                if (thisframe.joints[j].JointName.Equals(listOfAllBones[i].name))
                {
                    listOfAllBones[i].position = thisframe.joints[j].position;
                    var newRot = vec4ToQuaternion(thisframe.joints[j].rotation);
                    listOfAllBones[i].rotation = Quaternion.identity;
                }
            }
        }
    }

    void GetAllChildren(Transform parent, List<Transform> childList)
    {


        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                childList.Add(parent.GetChild(i));
                //121 points
                GetAllChildren(parent.GetChild(i), childList);
            }
        }


    }

    void streamReader()
    {
        StreamReader reader = new StreamReader(csvfile);

        readNames(reader);
        readstream(reader);
        done2 = true;
    }


    void readstream(StreamReader reader)
    {
        while(!reader.EndOfStream)
        {
            Frame2 temp = new Frame2();
            temp.joints = new Joint2[rokokoBones.Count];
            var line = reader.ReadLine();
            var values = line.Split(',');
            float[] floatValues = new float[values.Length];

            // Convesion from string to float
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains("."))
                    {
                        int valueLength = values[i].Length;
                        floatValues[i] = float.Parse(values[i]);

                        int start = 0;
                        if (values[i].Contains("-"))
                        {
                            start++;
                        }

                        for (int j = start; j < valueLength - 2; j++)
                        {
                            floatValues[i] *= 0.1f;
                        }
                    }

            }

            //read float to the joints
            temp.frameNumber = (int)floatValues[0];

            for (int i = 0; i < rokokoBones.Count; i++)
            {

                Vector3 tempvect = new Vector3(floatValues[i*7+8], floatValues[i * 7 + 9], floatValues[i * 7 + 10]);
                temp.joints[i].position = tempvect;

                Vector4 tempvec4 = new Vector4(floatValues[i * 7 + 11], floatValues[i * 7 + 12],
                                                floatValues[i * 7 + 13], floatValues[i * 7 + 14]);
                temp.joints[i].rotation = tempvec4;
                temp.joints[i].JointName = rokokoBones[i];
                
            }

            animations.Add(temp);
        }
    }



    void readNames(StreamReader reader)
    {
        var nameLine = reader.ReadLine();
        var boneName = nameLine.Split(',');

        //former 8 no use
        
        for (int j = 0; j < listOfAllBones.Count; j++)
        {
            for (int i = 8; i < boneName.Length; i++)
            {
                if (rokokoBones.Contains(listOfAllBones[j].name))
                    break;

                //hard code
                if (listOfAllBones[j].name == "R_Eye" && boneName[i].Contains("RightEye"))
                {
                    rokokoBones.Add(listOfAllBones[j].name);
                    continue;
                }
                if (listOfAllBones[j].name == "L_Eye" && boneName[i].Contains("LeftEye"))
                {
                    rokokoBones.Add(listOfAllBones[j].name);
                    continue;
                }

                if (isBoneName(boneName[i], listOfAllBones[j].name))
                {
                    //hard code
                    if (listOfAllBones[j].name == "Neck1")
                        continue;
                   
                    rokokoBones.Add(listOfAllBones[j].name);
                }
            }
        }


    }

    //since some names do not contain extract same names
    bool isBoneName(string boneName,string unityName)
    {
        int len = boneName.Length;
        string realname = boneName.Substring(10, len - 14);
        var realnames = realname.Split('_');
        for (int i = 0; i < realnames.Length; i++)
            if (!unityName.Contains(realnames[i]))
                return false;
        return true;
    }

    Quaternion vec4ToQuaternion(Vector4 vec)
    {
        Quaternion qt = new Quaternion(vec.x, vec.y, vec.z, vec.w);
        return qt;
    }

}


public struct Joint2
{
    public Vector3 position;
    public Vector4 rotation;
    public string JointName;

}

public struct Frame2
{
    public Joint2[] joints;
    public int frameNumber;
}