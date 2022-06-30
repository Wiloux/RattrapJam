using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> AC = new List<AudioClip>();
    private AudioSource AS;

    public static SoundManager instance; 
    void Awake()
    {
        instance = this;
        AS = GetComponent<AudioSource>();
    }

    public void PlayClip(int nb)
	{
        AS.pitch = Random.Range(0.8f, 1.2f);
        AS.PlayOneShot(AC[nb]);
	}
}
