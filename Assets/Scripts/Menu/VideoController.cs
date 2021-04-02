using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VideoPlayer))]
public class VideoController : MonoBehaviour
{
    private AudioSource audio;

    private VideoPlayer videoPlayer;

    public VideoPlayer VideoPlayer
    {
        get
        {
            if (this.videoPlayer == null)
            {
                this.videoPlayer = this.GetComponent<VideoPlayer>();
            }

            return this.videoPlayer;
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


    void Start()
    {
        var videoPlayer = GetComponent<VideoPlayer>();
        var audioSource = GetComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    private void OnEnable()
    {
        PlayVideo();
    }

    private void OnDisable()
    {
        if (!IsVideoPlaying(this.VideoPlayer))
            RestartVideo(this.VideoPlayer);
    }

    void Update()
    {
    }

    public void SetVideoClip(VideoClip clip)
    {
        if (this.VideoPlayer.isPlaying)
        {
            this.VideoPlayer.Stop();
        }

        this.VideoPlayer.clip = clip;
    }

    public void PlayVideo()
    {
        if (!IsVideoPlaying(this.VideoPlayer))
        {
            RestartVideo(this.VideoPlayer);
        }
        else if (!this.VideoPlayer.isPlaying)
        {
            this.VideoPlayer.Stop();
        }
        this.VideoPlayer.Play();
    }

    private void RestartVideo(VideoPlayer player)
    {
        if (player.isPlaying)
            player.Stop();
    }


    private VideoController IsVideoPlaying(VideoPlayer player)
    {
        var videoList = GameObject.FindObjectsOfType<VideoController>();
        foreach (var video in videoList)
        {
            if (video == this) continue;
            if (!video.isActiveAndEnabled) continue;

            VideoPlayer movie = this.videoPlayer;
            if (movie == player)
            {
                return video;
            }
        }

        return null;
    }
}
