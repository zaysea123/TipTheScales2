using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackEntry
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "MusicLibrary", menuName = "Sound/MusicLibrary")]
public class MusicLibrary : ScriptableObject
{
    public List<TrackEntry> entries;

    public AudioClip GetTrackFromName(string name)
    {
        return entries.Find(x => x.name == name).clip;
    }
}
