using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AnimClipCSV : AnimClip
{
    public TextAsset CSVFile;

    void OnEnable() {
        ImportData();
    }

    public void ImportData() {
        if (Frames == null || Frames.Count == 0) {
            var importer = new AnimationCSVImporter(CSVFile);

            Frames = importer.ParseCSV();
        }
    }
}
