using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	// volume ints
	float SFX = 100, Music = 100, MasterVol = 100;
	bool MuteAll = false;

	// Sensitivity
	float mouseSen = 1, ADSSen = 0.5f, maxSen = 100;

	// Hpbaron ( off by default )
	bool HpBarON = false;

	int toggleHpVal, toggleMuteVal;


	// Sound sliders
	[Header("Sounds")]
	public Slider music_slider, SFX_slider, master_slider;

	[Header("Mouse Sensitivity")]
	public Slider Sensitivity, ADSSensitivity;

	[Header("Toggles")]
	public Toggle mute_toggle = null, Hp_toggle;


	private void Start()
	{
		Sensitivity.maxValue = maxSen;
		music_slider.maxValue = 100;
		SFX_slider.maxValue = 100;
		master_slider.maxValue = 100;
		ADSSensitivity.maxValue = 1;

		music_slider.value = Music;
		SFX_slider.value = SFX;
		master_slider.value = MasterVol;
		Sensitivity.value = mouseSen;
		ADSSensitivity.value = ADSSen;

		mute_toggle.isOn = MuteAll;
	 	Hp_toggle.isOn = HpBarON;
	}

	private void Update()
	{
		mouseSen = Sensitivity.value;
		ADSSen = ADSSensitivity.value;

		MasterVol = master_slider.value;
		Music = music_slider.value;
		SFX = SFX_slider.value;

		HpBarON = Hp_toggle.isOn;
		if (HpBarON == true)
		{
			toggleHpVal = 1;
		}
		else
		{
			toggleHpVal = 0;
		}

		MuteAll = mute_toggle;


		SavePlayerPrefs();
	}

	void SavePlayerPrefs()
	{
		PlayerPrefs.SetFloat("MouseSen", mouseSen);
		PlayerPrefs.SetFloat("ADSSen", ADSSen);

		PlayerPrefs.SetInt("HPValBool", toggleHpVal);

		PlayerPrefs.Save();
	}

	public void LoadScene(int scene_index)
    {
        SceneManager.LoadScene(scene_index);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
