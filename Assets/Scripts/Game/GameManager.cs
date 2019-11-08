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
		[SerializeField] private Color defaultSaturationAndValue;
		[SerializeField] private Color backgroundTint;

		private Image playButtonImage;
		private Image stopButtonImage;
		private float currentHue;
		private float defaultColorSaturation;
		private float defaultColorValue;

		private void Awake()
		{
			playButton.onClick.AddListener(PlayRandomMusic);
			stopButton.onClick.AddListener(StopMusic);

			playButtonImage = playButton.GetComponent<Image>();
			stopButtonImage = stopButton.GetComponent<Image>();
			currentHue = Random.value;
			Color.RGBToHSV(defaultSaturationAndValue, out var hue, out defaultColorSaturation, out defaultColorValue);

			StopMusic();
		}

		private void PlayRandomMusic()
		{
			stopButton.gameObject.SetActive(true);
			playButton.gameObject.SetActive(false);

			StopAllCoroutines();

			var clip = musicConfig.GetRandomSong();
			audioSource.clip = clip;

			var randomSecond = Random.Range(0, clip.length);
			audioSource.time = randomSecond;
			audioSource.Play();

			var randomPitch = Random.Range(0.5f, 2f);
			audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitch", randomPitch);
			// pitch is an exposed parameters in audio mixer. it is a parameter in pitch
			// effect which lets you change pitch without speeding up audio

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

		private void PaintInterface(Color color)
		{
			backgroundImage.color = color * backgroundTint;
			playButtonImage.color = color;
			stopButtonImage.color = color;
		}

		private void WarpColor(float hueDelta)
		{
			if (float.IsNaN(hueDelta)) hueDelta = 0f;
			var color = Color.HSVToRGB(Mathf.Repeat(currentHue, 1f), defaultColorSaturation, defaultColorValue);
			color.a = 1f;
			currentHue += hueDelta;
			PaintInterface(color);
		}

		private IEnumerator PlayColorsWithMusic()
		{
			var soundAnalyzer = new SoundAnalyzer(audioSource);
			while (audioSource.isPlaying)
			{
				var value = soundAnalyzer.AnalyzeSound();
				var normalizedValue = value / musicConfig.NormalizationValue;
				var finalValue = 
					normalizedValue * normalizedValue;
//					Mathf.Tan(normalizedValue);
				WarpColor(finalValue);
				yield return null;
			}

			StopMusic();
		}

		private IEnumerator ShowColorWarping()
		{
			const float hueDelta = 1 / 255f * 0.2f;
			while (true)
			{
				WarpColor(hueDelta);
				yield return null;
			}
		}
	}
}