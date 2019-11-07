using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "MusicConfig", menuName = "Configs/Music config")]
	public sealed class MusicConfig : ScriptableObject
	{
		[SerializeField] private AudioClip[] songs;

		public AudioClip GetRandomSong()
		{
			var randomIndex = Random.Range(0, songs.Length);
			return songs[randomIndex];
		}
	}
}