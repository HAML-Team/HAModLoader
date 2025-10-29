using UnityEngine;

public class ResultsAnm : MonoBehaviour
{
	public static ResultsAnm Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void anm_sound_mother()
	{
		NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_mother);
	}

	public void anm_sound_father()
	{
		NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_father);
	}

	public void anm_mutants_merge()
	{
		NewBreedControl.Instance.mutants_merge();
	}

	public void anm_mutants_complete()
	{
		NewBreedControl.Instance.mutant_complete();
	}

	public void anm_lerp_first()
	{
		NewBreedControl.Instance.anm_lerp_first();
	}

	public void anm_lerp_second()
	{
		NewBreedControl.Instance.anm_lerp_second();
	}

	public void anm_lerp_result()
	{
		NewBreedControl.Instance.anm_lerp_result();
	}

	public void anm_result_emerge()
	{
		NewBreedControl.Instance.anm_result_emerge();
	}
}
