using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

public class AnimationCSVImporter
{
    // The CSV file to parse
    private readonly string[] fileLines;

    private Regex JointNameRegex = new Regex(@".+:(\w+)_\w_\w");

    private int numberOfColumns = 0;

    private Dictionary<string, int> namesToHeader = new Dictionary<string, int>();

    private List<AnimationFrame> Frames;

    public AnimationCSVImporter (TextAsset csvFile) {
        fileLines = TextToLines(csvFile);

        Debug.Log($"Number of lines in CSV: {fileLines?.Length}");
    }

    public List<AnimationFrame> ParseCSV()
    {
        if (Frames != null) return Frames;
        Frames = new List<AnimationFrame>();

        if (fileLines == null) return Frames;
        
        // Read header to generate dictionary between joint names to index in csv line
        ParseHeader(); 
        
        for (var i = 1; i < fileLines.Length; i++) {
            ParseLine(i);
        }
        
        return Frames;
    }

    private void ParseHeader() {
        var header = fileLines[0];
        var headerSplit = header.Split(',');
        numberOfColumns = headerSplit.Length;        

        Debug.Log($"Number of columns in CSV: {numberOfColumns}"); 

        for (var i = 0; i < numberOfColumns; i++) {
            var headerFragment = headerSplit[i]; 

            // Match the fragment against the regex, and go to next fragment if it fails
            var match = JointNameRegex.Match(headerFragment);
            if (!match.Success) continue; 

            var groups = match.Groups;
            var jointName = groups[1].Value;

            // If we already added the joint name, go to next fragment.
            if (namesToHeader.ContainsKey(jointName)) continue;

            //Debug.Log($"Found joint name: {jointName}");
            namesToHeader.Add(jointName, i);
        }

        Debug.Log($"Found a total of {namesToHeader.Count} joints");
    }
   
    private void ParseLine(int index) {
        var line = fileLines[index]; 
        if (String.IsNullOrWhiteSpace(line)) return;

        var lineSplit = line.Split(',');
        Assert.AreEqual(numberOfColumns, lineSplit.Length, $"Line does not contain {numberOfColumns} columns... It is line #{index}: \n {line}");

        var timeString = lineSplit[0];
        var time = CheckForDotAndParse(timeString);

        var frame = new AnimationFrame { Time = time, JointPoints = new List<AnimationJointPoint>() };
        foreach (var nameIndex in namesToHeader) {
            var jointPoint = new AnimationJointPoint();
            jointPoint.Name = nameIndex.Key;

            ParseJointPoint(lineSplit, nameIndex.Value, ref jointPoint);

            frame.JointPoints.Add(jointPoint);
        }

        Frames.Add(frame);
    } 

    private void ParseJointPoint(string[] lineSplit, int jointIndex, ref AnimationJointPoint jointPoint) {
        var pxString = lineSplit[jointIndex];
        var pyString = lineSplit[jointIndex + 1];
        var pzString = lineSplit[jointIndex + 2];

        var rxString = lineSplit[jointIndex + 3];
        var ryString = lineSplit[jointIndex + 4];
        var rzString = lineSplit[jointIndex + 5];
        var rwString = lineSplit[jointIndex + 6];

        var px = CheckForDotAndParse(pxString); 
        var py = CheckForDotAndParse(pyString);
        var pz = CheckForDotAndParse(pzString);

        var rx = CheckForDotAndParse(rxString);
        var ry = CheckForDotAndParse(ryString);
        var rz = CheckForDotAndParse(rzString);
        var rw = CheckForDotAndParse(rwString);




        /*
        Assert.AreEqual(pxString, px.ToString());
        Assert.AreEqual(pyString, py.ToString());
        Assert.AreEqual(pzString, pz.ToString());
        Assert.AreEqual(rxString, rx.ToString());
        Assert.AreEqual(ryString, ry.ToString());
        Assert.AreEqual(rzString, rz.ToString());
        Assert.AreEqual(rwString, rw.ToString());
        */
        jointPoint.Position = new Vector3(px, py, pz);
        jointPoint.Rotation = new Quaternion(rx, ry, rz, rw);

        if (Frames.Count < 1) {
            jointPoint.BaseRotation = jointPoint.Rotation;
        } else {
            var firstFrame = Frames[0];
            var jpName = jointPoint.Name;
            var baseJoint = firstFrame.JointPoints.First(x => x.Name.Equals(jpName));

            jointPoint.BaseRotation = baseJoint.Rotation;
        }
    }
     
    private string[] TextToLines(TextAsset text) {
        var lines = text?.text.Split(new [] {
            "\r\n", "\r", "\n" 
        }, StringSplitOptions.None);

        return lines;
    } 

    float CheckForDotAndParse(string value)
    {
        float parsedValue;
        if (value.Contains("."))
        {
            int valueLength = value.Length;
            parsedValue = float.Parse(value);

            int start = 0;
            if (value.Contains("-"))
            {
                start++;
            }

            for (int j = start; j < valueLength - 2; j++)
            {
                parsedValue *= 0.1f;
            }

        }
        else
        {
            parsedValue = float.Parse(value);
        }

        return parsedValue;
    }

}
