using UnityEngine;
using System.Collections;

namespace GameSelection
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : AManager<AudioManager> {

		public AudioSource m_musicAudioSource;
		public AudioSource m_sfxAudioSource;
		public AudioClip m_countdownBeepSfx;
		public AudioClip m_countdownEndSfx;

		[Range(0,1)]
		public float m_maxMusicVolume = 1;
		[Range(0,10)]
		public float m_musicFadeTime = 2;

		private float m_musicVolumeUpLerpProgress = 0.0f;
		private float m_musicVolumeDownLerpProgress = 0.0f;

		private void Start()
		{
			if(!m_musicAudioSource.isPlaying)
			{
				m_musicAudioSource.Play();
			}
		}

		private void Update()
		{
			if(!(GameManager.Instance as GameManager).NextGameDecided)
				FadeUpMusicVolume ();
			else
				FadeDownMusicVolume ();
		}

		public void FireCountdownEndSfx()
		{
			m_sfxAudioSource.PlayOneShot(m_countdownEndSfx);
		}

		public void FireCountdownSfx ()
		{
			m_sfxAudioSource.PlayOneShot(m_countdownBeepSfx);
		}

		private void FadeUpMusicVolume()
		{
			if (m_musicVolumeUpLerpProgress < 1.0f) {
				m_musicVolumeUpLerpProgress += (Time.deltaTime/m_musicFadeTime);
				m_musicAudioSource.volume = Mathf.Lerp (0,m_maxMusicVolume,m_musicVolumeUpLerpProgress);
			}
		}
		
		private void FadeDownMusicVolume()
		{
			if (m_musicVolumeDownLerpProgress < 1.0f) {
				m_musicVolumeDownLerpProgress += (Time.deltaTime/m_musicFadeTime);
				m_musicAudioSource.volume = Mathf.Lerp (m_maxMusicVolume,0,m_musicVolumeDownLerpProgress);
			}
		}
	}
}