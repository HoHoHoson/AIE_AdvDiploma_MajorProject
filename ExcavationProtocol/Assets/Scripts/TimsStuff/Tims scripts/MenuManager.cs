using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	// volume ints
	float SFX, Music, MasterVol;
	bool MuteAll = false;

	// Sensitivity
	float mouseSen, ADSSen, maxSen = 100;

	// Hpbaron ( off by default )
	bool HpBarON = false;

	int toggleHpVal, toggleMuteVal;


	// Sound sliders
	[Header("Sounds")]
	public Slider music_slider = null, SFX_slider = null, master_slider = null;

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
