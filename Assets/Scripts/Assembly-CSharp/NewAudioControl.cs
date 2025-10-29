using UnityEngine;

public class NewAudioControl : MonoBehaviour
{
	public static NewAudioControl Instance;

	private AudioSource source_music;

	private AudioSource source_effects;

	private AudioSource source_rand;

	private string music = "";

	public AudioClip sfx_genericClick;

	public AudioClip sfx_music_game;

	public AudioClip sfx_music_breed;

	public AudioClip sfx_bubble;

	public AudioClip sfx_game_Start;

	public AudioClip sfx_mutate;

	public AudioClip sfx_mother;

	public AudioClip sfx_father;

	public AudioClip sfx_unlock_chest;

	public AudioClip sfx_chest_whoosh;

	public AudioClip sfx_speak;

	public AudioClip sfx_closedoor;

	public AudioClip sfx_opendoor;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (this == Instance)
		{
			source_music = base.gameObject.AddComponent<AudioSource>();
			source_effects = base.gameObject.AddComponent<AudioSource>();
			source_rand = base.gameObject.AddComponent<AudioSource>();
			play_menu_and_breed_music();
		}
	}

	public void Play(AudioClip clip, float volume = 1f)
	{
		source_effects.PlayOneShot(clip, volume);
	}

	public void PlayRand(AudioClip clip, float low, float high, float vol = 1f)
	{
		source_rand.pitch = Random.Range(low, high);
		source_rand.PlayOneShot(clip, vol);
	}

	public void play_generic_click(float volume = 1f)
	{
		source_effects.PlayOneShot(sfx_genericClick, volume);
	}

	public void play_gameplay_music()
	{
		if (!(music == "game"))
		{
			if (source_music.isPlaying)
			{
				source_music.Stop();
			}
			source_music.clip = sfx_music_game;
			source_music.Play();
			music = "game";
		}
	}

	public void play_menu_and_breed_music()
	{
		if (!(music == "breed"))
		{
			if (source_music.isPlaying)
			{
				source_music.Stop();
			}
			source_music.clip = sfx_music_breed;
			source_music.Play();
			music = "breed";
		}
	}
}
