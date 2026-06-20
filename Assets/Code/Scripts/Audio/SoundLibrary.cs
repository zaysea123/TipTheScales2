using AudioSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEntry
{
    public string name;
    public SoundData data;
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Sound/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundEntry> entries;

    public SoundData GetDataFromName(string name)
    {
        return entries.Find(x => x.name == name).data;
    }
}
