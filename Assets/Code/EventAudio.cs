using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAudio : MonoBehaviour
{
    public AudioClip lose_sound;
    public void PlayLoseSound()
    {
        GetComponent<AudioSource>().clip = lose_sound;
        GetComponent<AudioSource>().Play();
    }


}
