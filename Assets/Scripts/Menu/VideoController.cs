using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class VideoController : MonoBehaviour
{
    private AudioSource audio;
    private MovieTexture video;

    public MovieTexture VideoTexture {
        get
        {
            if (video == null)
                video = GetComponent<Renderer>().material.mainTexture as MovieTexture;
            return video;
        }
    }

    public AudioSource Audio
    {
        get
        {
            if (audio == null)
                audio = GetComponent<AudioSource>();
            return audio;
        }
    }


    void Start () {
    }

    private void OnEnable()
    {
        PlayVideo();
    }

    private void OnDisable()
    {
        StopAudio();
        if (!IsVideoPlaying(VideoTexture))
            RestartVideo(VideoTexture);            
    }

    void Update () {
		
	}

    public void PlayVideo()
    {
        StopAudio();
        MovieTexture movie = VideoTexture;
        if (!IsVideoPlaying(movie))
        {
            RestartVideo(movie);
            PlayAudio();
        }
        else if (!movie.isPlaying)
        {
            movie.Stop();
            PlayAudio();
        }
        movie.Play();
    }

    public void PlayAudio()
    {
        var vCont = IsVideoPlaying(VideoTexture);
        if (vCont)
        {
            vCont.Audio.Stop();
            vCont.Audio.clip = null;
        }

        Audio.clip = VideoTexture.audioClip;
        Audio.Play();
    }

    private void RestartVideo(MovieTexture movie)
    {
        if (movie.isPlaying)
            movie.Stop();
    }

    private void StopAudio()
    {
        if (Audio.clip != null)
        {
            Audio.Stop();

            var vController = IsVideoPlaying(video);
            if (vController != null && vController.Audio.clip == null)
            {
                vController.PlayAudio();
            }

            Audio.clip = null;
        }
        video = null;
    }

    private VideoController IsVideoPlaying(MovieTexture actMovie)
    {
        var vidoeList = GameObject.FindObjectsOfType<VideoController>();
        foreach (var video in vidoeList)
        {
            if (video == this) continue;
            if (!video.isActiveAndEnabled) continue;

            MovieTexture movie = video.VideoTexture;
            if (movie == actMovie)
            {
                return video;
            }
        }

        return null;
    }
}
