using UnityEngine;

namespace Game
{
	public sealed class SoundAnalyzer
	{
		private const int SAMPLE_SIZE = 1024;

		private readonly AudioSource source;
		private readonly float[] samples;
		private readonly float[] spectrum;
		private readonly float sampleRate;
		private float rmsValue;
		private float dbValue;
		private float pitchValue;
		private float threshold;

		public SoundAnalyzer(AudioSource source)
		{
			this.source = source;
			samples = new float[SAMPLE_SIZE];
			spectrum = new float[SAMPLE_SIZE];
			sampleRate = AudioSettings.outputSampleRate;
			threshold = 0f;
		}

		public float AnalyzeSound()
		{
			source.GetOutputData(samples, 0);
			var sum = 0f;
			for (var i = 0; i < SAMPLE_SIZE; i++)
			{
				sum += samples[i] * samples[i];
			}

			rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
			dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);
			if (dbValue < -160) dbValue = -160;
			source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
			var maxV = 0f;
			var maxN = 0;
			for (var i = 0; i < SAMPLE_SIZE; i++)
			{
				if (spectrum[i] > maxV && spectrum[i] > threshold)
				{
					maxV = spectrum[i];
					maxN = i;
				}
			}

			var freqN = (float)maxN;
			if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
			{
				var dL = spectrum[maxN - 1] / spectrum[maxN];
				var dR = spectrum[maxN + 1] / spectrum[maxN];
				freqN += 0.5f * (dR * dR - dL * dL);
			}

			pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
			var average = (rmsValue + dbValue + pitchValue) / 3;

			return average;
		}
	}
}