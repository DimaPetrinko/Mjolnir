using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game
{
	public sealed class GameManager : MonoBehaviour
	{
		[SerializeField] private MusicConfig musicConfig;
		[SerializeField] private Button playButton;
		[SerializeField] private Button stopButton;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Color backgroundTint;

		private Image playButtonImage;
		private Image stopButtonImage;
		private float currentHue;

		private void Awake()
		{
			playButton.onClick.AddListener(PlayRandomMusic);
			stopButton.onClick.AddListener(StopMusic);

			playButtonImage = playButton.GetComponent<Image>();
			stopButtonImage = stopButton.GetComponent<Image>();
			currentHue = Random.value;

			StopMusic();
		}

		private void PlayRandomMusic()
		{
			stopButton.gameObject.SetActive(true);
			playButton.gameObject.SetActive(false);

			StopAllCoroutines();

			// get random song
			var clip = musicConfig.GetRandomSong();
			audioSource.clip = clip;
			// get random time
			audioSource.Play();
			// set random pitch
			// start detecting peaks and changing colors
			StartCoroutine(PlayColorsWithMusic());
		}

		private void StopMusic()
		{
			stopButton.gameObject.SetActive(false);
			playButton.gameObject.SetActive(true);

			StopAllCoroutines();
			StartCoroutine(ShowColorWarping());

			audioSource.Stop();
		}

		private void SetColor(Color color)
		{
			backgroundImage.color = color * backgroundTint;
			playButtonImage.color = color;
			stopButtonImage.color = color;
		}
		
		private IEnumerator PlayColorsWithMusic()
		{
			// while audio source is playing
			while (audioSource.isPlaying)
			{
				// get value from audio source
				// detect is this value is right for color change
				// change to random color
				yield return null;
			}
		}

		private IEnumerator ShowColorWarping()
		{
			Color color;
			color.a = 1;
			const float hueDelta = 1 / 255f * 0.2f;
			while (true)
			{
				color = Color.HSVToRGB(Mathf.Repeat(currentHue, 1f), 1, 1);
				currentHue += hueDelta;
				SetColor(color);
				yield return null;
			}
		}
	}
}