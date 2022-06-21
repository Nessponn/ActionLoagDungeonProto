using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioClip Jump;
    public AudioClip Dash;
    public AudioClip Step;
    public AudioClip Smash;
    public AudioClip Damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpAudio()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Jump);
    }

    public void DashAudio()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Dash);
    }


    public void StepAudio()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Step);
    }

    public void SmashAudio()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Smash);
    }

    public void DamageAudio()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Damage);
    }
}
