using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
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
        var videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        var audioSource = gameObject.AddComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";
        videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    private void OnEnable()
    {
        PlayVideo();
    }

    private void OnDisable()
    {
        if (!IsVideoPlaying(this.videoPlayer))
            RestartVideo(this.videoPlayer);
    }

    void Update()
    {
    }

    public void PlayVideo()
    {
        VideoPlayer player = this.videoPlayer;
        if (!IsVideoPlaying(player))
        {
            RestartVideo(player);
        }
        else if (!player.isPlaying)
        {
            player.Stop();
        }
        player.Play();
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
