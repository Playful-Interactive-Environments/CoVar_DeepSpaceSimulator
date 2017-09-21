using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class VideoController : MonoBehaviour
{

	void Start () {
        MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = movie.audioClip;

        audio.Play();
        movie.Play();
	}
	
	void Update () {
		
	}
}
