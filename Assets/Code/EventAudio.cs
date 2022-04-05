using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAudio : MonoBehaviour
{
    public AudioClip lose_sound;
    public AudioClip cabbage_sound;
    public AudioClip next_day_sound;
    public AudioClip splicing_sound;
    public AudioClip sell_sound;
    public AudioClip sellout_sound;
    public void PlayLoseSound()
    {
        GetComponent<AudioSource>().clip = lose_sound;
        GetComponent<AudioSource>().Play();
    }

    public void PlayCabbagePlaceSound()
    {
        GetComponent<AudioSource>().clip = cabbage_sound;
        GetComponent<AudioSource>().Play();
    }

    public void PlayNextDaySound()
    {
        GetComponent<AudioSource>().clip = next_day_sound;
        GetComponent<AudioSource>().Play();
    }

    public void PlaySplicingSound()
    {
        GetComponent<AudioSource>().clip = splicing_sound;
        GetComponent<AudioSource>().Play();
    }

    public void PlaySellSound()
    {
        GetComponent<AudioSource>().clip = sell_sound;
        GetComponent<AudioSource>().Play();
    }

    public void PlaySelloutSound()
    {
        GetComponent<AudioSource>().clip = sellout_sound;
        GetComponent<AudioSource>().Play();
    }


}
