using UnityEngine;
using System.Collections;

namespace SwarmDefender
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : AManager<AudioManager> {

		public AudioSource m_musicAudioSource;
		public AudioSource m_sfxAudioSource;
		public AudioClip m_gameOverSfx;
		public AudioClip m_gameWonSfx;
		public AudioClip m_fadeSfx;
		public AudioClip m_missionBriefingStartSfx;
		public AudioClip m_missionBriefingInvaderStartSfx;
		public AudioClip m_missionBriefingWonSfx;
		[Range(0,1)]
		public float m_maxMusicVolume = 0.2f;
		[Range(0,10)]
		public float m_musicFadeTime = 4;

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
			if(FadeManager.Instance.PlayMusic)
			{
				FadeUpMusicVolume ();
			}
			else if(m_musicAudioSource.volume > 0)
			{
				FadeDownMusicVolume ();
			}
		}
		
		public void FireGameWonSfx()
		{
			m_sfxAudioSource.PlayOneShot(m_gameWonSfx);
		}

		public void FireMissionBriefingStart()
		{
			m_sfxAudioSource.PlayOneShot(m_missionBriefingStartSfx);
		}
		
		public void FireMissionBriefingInvaderStart()
		{
			m_sfxAudioSource.PlayOneShot(m_missionBriefingInvaderStartSfx);
		}

		public void FireMissionBriefingWon()
		{
			m_sfxAudioSource.PlayOneShot(m_missionBriefingWonSfx);
		}

		public void FireGameOverSfx()
		{
			m_sfxAudioSource.PlayOneShot(m_gameOverSfx);
		}

		public void FireFadeSfx()
		{
			m_sfxAudioSource.PlayOneShot(m_fadeSfx);
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