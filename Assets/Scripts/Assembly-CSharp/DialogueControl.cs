using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour
{
	public enum dialogue_type
	{
		other_speak = 0,
		option = 1
	}

	public static DialogueControl Instance;

	public GameObject dialogue_header;

	public GameObject dialogue_other;

	public GameObject next_button;

	public Text txt_dialogue_other;

	public GameObject parent_twoOptions;

	public GameObject parent_oneOption;

	public Text txt_optionL;

	public Text txt_optionR;

	public Text txt_optionSingle;

	public Text txt_NPC_name;

	private Dictionary<int, other_dialogue_type> other_dialogue = new Dictionary<int, other_dialogue_type>();

	private Dictionary<int, self_options_type> self_options = new Dictionary<int, self_options_type>();

	[HideInInspector]
	public Dictionary<int, string> NPC_names = new Dictionary<int, string>();

	[HideInInspector]
	public int curr_NPC;

	private IEnumerator show_text_t;

	private other_dialogue_type curr_other_dialogue;

	private self_options_type curr_self_options;

	private IEnumerator delayed_initial_blobble_t;

	private void Awake()
	{
		Instance = this;
	}

	public void OtherSpeak(int key, string str, string important_bit, int go_to)
	{
		str = TranslationControl.Instance.Translate(str);
		important_bit = TranslationControl.Instance.Translate(important_bit);
		other_dialogue_type other_dialogue_type2 = new other_dialogue_type();
		other_dialogue_type2.go_to = go_to;
		other_dialogue_type2.text = str;
		if (important_bit != "" && str.Contains(important_bit))
		{
			other_dialogue_type2.important_start = str.IndexOf(important_bit);
			other_dialogue_type2.important_end = other_dialogue_type2.important_start + important_bit.Length;
		}
		other_dialogue.Add(key, other_dialogue_type2);
	}

	public void MY_OPTION(int key, string str, int go_to)
	{
		if (self_options.ContainsKey(key))
		{
			self_options_type obj = self_options[key];
			obj.optionB = TranslationControl.Instance.Translate(str);
			obj.optionB_goto = go_to;
		}
		else
		{
			self_options_type self_options_type2 = new self_options_type();
			self_options_type2.optionA = TranslationControl.Instance.Translate(str);
			self_options_type2.optionA_goto = go_to;
			self_options.Add(key, self_options_type2);
		}
	}

	public void EnterDialogue(int NPC_id)
	{
		curr_NPC = NPC_id;
		other_dialogue.Clear();
		self_options.Clear();
		switch (NPC_id)
		{
		case 1:
			Fetch_VitaminApe();
			break;
		case 2:
			Fetch_PineapplePig();
			break;
		case 3:
			Fetch_SnailCrab();
			break;
		case 4:
			Fetch_OctoKitty();
			break;
		}
		dialogue_header.SetActive(true);
		dialogue_header.GetComponent<Animation>().Play("dialoguebobble");
		txt_NPC_name.text = TranslationControl.Instance.Translate(NPC_names[NPC_id]);
		delayed_initial_blobble_t = delayed_initial_blobble();
		StartCoroutine(delayed_initial_blobble_t);
		GameController.Instance.SetBackground_NPC();
	}

	private IEnumerator show_text(string goaltext, int imporant_start, int important_end)
	{
		int i = 0;
		string curr_text = "";
		txt_dialogue_other.text = "";
		do
		{
			yield return new WaitForSeconds(0.02f);
			if (imporant_start != -1)
			{
				if (i == 0)
				{
					curr_text = "<color=#ffffff>";
					curr_text += goaltext[i];
				}
				else if (i < imporant_start - 1)
				{
					curr_text += goaltext[i];
					txt_dialogue_other.text = curr_text + "</color>";
				}
				else if (i == imporant_start - 1)
				{
					curr_text += "</color>";
					curr_text += goaltext[i];
					txt_dialogue_other.text = curr_text;
				}
				else if (i > imporant_start - 1 && i < important_end)
				{
					curr_text += goaltext[i];
					txt_dialogue_other.text = curr_text;
				}
				else if (i == important_end)
				{
					curr_text += "<color=#ffffff>";
					curr_text += goaltext[i];
					txt_dialogue_other.text = curr_text + "</color>";
				}
				else
				{
					curr_text += goaltext[i];
					txt_dialogue_other.text = curr_text + "</color>";
				}
			}
			else
			{
				curr_text += goaltext[i];
				txt_dialogue_other.text = "<color=#ffffff>" + curr_text + "</color>";
			}
			i++;
			if (i % 5 == 0)
			{
				NewAudioControl.Instance.PlayRand(NewAudioControl.Instance.sfx_speak, 0.4f, 0.6f, 0.17f);
			}
		}
		while (i < goaltext.Length);
		yield return new WaitForSeconds(0.1f);
		next_button.SetActive(true);
	}

	private void temp_close_NPC()
	{
		GameController.Instance.focus_npc = null;
		GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject.SetActive(true);
		Instance.ExitDialogue();
		WindowControl.Instance.curr_window = WindowControl.window_type_t.none;
		WindowControl.Instance.close_button.SetActive(false);
	}

	public void click_option_A()
	{
		parent_twoOptions.SetActive(false);
		parent_oneOption.SetActive(false);
		if (other_dialogue.ContainsKey(curr_self_options.optionA_goto))
		{
			curr_other_dialogue = other_dialogue[curr_self_options.optionA_goto];
			BlobbleCurrText();
		}
		else if (curr_self_options.optionA_goto == -88)
		{
			inventory_ctr.Instance.open_buy(curr_NPC);
			temp_close_NPC();
		}
		else if (curr_self_options.optionA_goto == -99)
		{
			inventory_ctr.Instance.open_sell(curr_NPC);
			temp_close_NPC();
		}
		else
		{
			WindowControl.Instance.PressClose();
		}
	}

	public void click_option_B()
	{
		parent_twoOptions.SetActive(false);
		parent_oneOption.SetActive(false);
		if (other_dialogue.ContainsKey(curr_self_options.optionB_goto))
		{
			curr_other_dialogue = other_dialogue[curr_self_options.optionB_goto];
			BlobbleCurrText();
		}
		else if (curr_self_options.optionB_goto == -99)
		{
			inventory_ctr.Instance.open_sell(curr_NPC);
			temp_close_NPC();
		}
		else
		{
			WindowControl.Instance.PressClose();
		}
	}

	public void ExitDialogue()
	{
		if (delayed_initial_blobble_t != null)
		{
			StopCoroutine(delayed_initial_blobble_t);
			delayed_initial_blobble_t = null;
		}
		if (show_text_t != null)
		{
			StopCoroutine(show_text_t);
			show_text_t = null;
		}
		dialogue_header.GetComponent<Animation>().Stop();
		dialogue_header.SetActive(false);
		dialogue_other.SetActive(false);
		next_button.SetActive(false);
		parent_twoOptions.SetActive(false);
		parent_oneOption.SetActive(false);
		GameController.Instance.SetBackground_Explore();
	}

	public void PressNext()
	{
		next_button.SetActive(false);
		if (other_dialogue.ContainsKey(curr_other_dialogue.go_to))
		{
			curr_other_dialogue = other_dialogue[curr_other_dialogue.go_to];
			BlobbleCurrText();
		}
		else if (self_options.ContainsKey(curr_other_dialogue.go_to))
		{
			dialogue_other.SetActive(false);
			curr_self_options = self_options[curr_other_dialogue.go_to];
			if (curr_self_options.optionB == "")
			{
				parent_oneOption.SetActive(true);
				txt_optionSingle.text = curr_self_options.optionA;
				txt_optionSingle.transform.parent.GetComponent<Animation>().Play("dialoguebobble 2");
				if (curr_self_options.optionA == "[SELL ITEMS]")
				{
					txt_optionSingle.transform.parent.Find("sell-ico").gameObject.SetActive(true);
					txt_optionSingle.text = "      [SELL ITEMS]";
				}
				else
				{
					txt_optionSingle.transform.parent.Find("sell-ico").gameObject.SetActive(false);
				}
				return;
			}
			parent_twoOptions.SetActive(true);
			txt_optionL.text = curr_self_options.optionA;
			txt_optionL.transform.parent.GetComponent<Animation>().Play("dialoguebobble 2");
			txt_optionR.text = curr_self_options.optionB;
			txt_optionR.transform.parent.GetComponent<Animation>().Play("dialoguebobble 2");
			if (curr_self_options.optionA == "[BUY ITEMS]")
			{
				txt_optionL.transform.parent.Find("buy-ico").gameObject.SetActive(true);
				txt_optionL.text = "      [BUY ITEMS]";
			}
			else
			{
				txt_optionL.transform.parent.Find("buy-ico").gameObject.SetActive(false);
			}
			if (curr_self_options.optionB == "[SELL ITEMS]")
			{
				txt_optionR.transform.parent.Find("sell-ico").gameObject.SetActive(true);
				txt_optionR.text = "      [SELL ITEMS]";
			}
			else
			{
				txt_optionR.transform.parent.Find("sell-ico").gameObject.SetActive(false);
			}
		}
		else
		{
			WindowControl.Instance.PressClose();
		}
	}

	public void BlobbleCurrText()
	{
		dialogue_other.SetActive(true);
		dialogue_other.GetComponent<Animation>().Stop();
		dialogue_other.GetComponent<Animation>().Play("dialoguebobble 2");
		if (show_text_t != null)
		{
			StopCoroutine(show_text_t);
			show_text_t = null;
		}
		show_text_t = show_text(curr_other_dialogue.text, curr_other_dialogue.important_start, curr_other_dialogue.important_end);
		StartCoroutine(show_text_t);
	}

	private IEnumerator delayed_initial_blobble()
	{
		yield return new WaitForSeconds(1f);
		curr_other_dialogue = other_dialogue[0];
		BlobbleCurrText();
	}

	private void Fetch_OctoKitty()
	{
		OtherSpeak(0, "Meow Meow", "", 1);
		OtherSpeak(1, "Snack is love, snack is life.", "", 2);
		MY_OPTION(2, "[BUY ITEMS]", -88);
		MY_OPTION(2, "[SELL ITEMS]", -99);
	}

	private void Fetch_SnailCrab()
	{
		OtherSpeak(0, "One crab's junk is another crab's treasure!", "", 1);
		OtherSpeak(1, "I'll buy anything!", "", 2);
		MY_OPTION(2, "[SELL ITEMS]", -99);
	}

	private void Fetch_PineapplePig()
	{
		OtherSpeak(0, "I love spikey things!!", "", 1);
		OtherSpeak(1, "Want some spikey things?!", "", 2);
		MY_OPTION(2, "[BUY ITEMS]", -88);
		MY_OPTION(2, "[SELL ITEMS]", -99);
	}

	private void Fetch_VitaminApe()
	{
		OtherSpeak(0, "Who dares to disturb the great 'Vitamin Ape'?!", "", 1);
		MY_OPTION(1, "[What are you?]", 3);
		MY_OPTION(1, "[Who are you?]", 105);
		OtherSpeak(3, "I am half Carrot, half Gorilla!", "", 4);
		MY_OPTION(4, "[Who are you?]", 5);
		OtherSpeak(5, "I am the keeper of all the world's secrets!", "", 6);
		OtherSpeak(105, "I am the keeper of all the world's secrets!", "", 106);
		MY_OPTION(106, "[Tell me a secret]", 7);
		MY_OPTION(106, "[What are you?]", 107);
		OtherSpeak(107, "I am half Carrot, half Gorilla!", "", 6);
		MY_OPTION(6, "[Tell me a secret]", 7);
		OtherSpeak(7, "Certainly.", "", 8);
		OtherSpeak(8, "If you play 'Hybrid Animals' on Wednesdays", "Wednesdays", 9);
		OtherSpeak(9, "you can test all premium creatures for free!", "all premium creatures for free!", 10);
		OtherSpeak(10, "However..", "", 11);
		OtherSpeak(11, "they will disappear the next day, unless you buy them.", "", 12);
		OtherSpeak(12, "You can test the premium creatures every Wednesday!", "every Wednesday!", 13);
		MY_OPTION(13, "[Tell me another secret]", 14);
		OtherSpeak(14, "I have no new secrets for now.", "", -1);
	}
}
