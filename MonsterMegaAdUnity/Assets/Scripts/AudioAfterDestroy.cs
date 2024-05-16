using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAfterDestroy : MonoBehaviour
{
    static public void PlayAudio(AudioClip clip, Vector3 pos, float lifetime = 3f)
    {
        GameObject audioObject = new GameObject("AudioAfterDestroy");
        var audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.loop = false;

        var selfDeleter = audioObject.AddComponent<DelayedSelfDeleter>();
        selfDeleter.m_delay = lifetime;

        Instantiate(audioObject, pos, Quaternion.identity);
    }
}
