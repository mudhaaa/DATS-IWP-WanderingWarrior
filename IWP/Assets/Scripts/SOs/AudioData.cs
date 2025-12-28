using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu()]
public class AudioData : ScriptableObject
{
    [Header("Audio Clip")]
    public AudioClip clip;
    public AudioMixerGroup mixergroup;

    [Header("Audio Settings")]
    public float volume = 1.0f;
    public float pitch = 1.0f;
    public bool loop = false;
}