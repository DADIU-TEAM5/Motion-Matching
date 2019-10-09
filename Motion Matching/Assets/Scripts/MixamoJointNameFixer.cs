using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixamoJointNameFixer : MonoBehaviour
{
    public Transform MixamoSkeleton;
    // Start is called before the first frame update
    void Awake()
    {
        FixAllChildren(MixamoSkeleton); 
    }

    public void FixAllChildren(Transform trans) {
        foreach (Transform child in trans) {
            if (child.childCount > 0) FixAllChildren(child);
            if (child.name.Contains("mixamorig:")) {
                child.name = child.name.Replace("mixamorig:", "");
            }
        }
    }
}
