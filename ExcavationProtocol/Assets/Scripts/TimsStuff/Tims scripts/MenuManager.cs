using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	// volume ints
	float MasterVol = 100;
	bool MuteAll = false;

	// Sensitivity
	float mouseSen = 1, ADSSen = 0.5f, maxSen = 10;

	// Hpbaron ( off by default )
	bool HpBarON = false;

	int toggleHpVal, toggleMuteVal;


	// Sound sliders
	[Header("Sounds")]
	public Slider master_slider;

	[Header("Mouse Sensitivity")]
	public Slider Sensitivity, ADSSensitivity;

	[Header("Toggles")]
	public Toggle mute_toggle = null, Hp_toggle;


	private void Start()
	{
		Sensitivity.maxValue = maxSen;
		master_slider.maxValue = 100;
		ADSSensitivity.maxValue = 1;
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


		HpBarON = Hp_toggle.isOn;
		if (HpBarON == true)
		{
			toggleHpVal = 1;
		}
		else
		{
			toggleHpVal = 0;
		}

		MuteAll = mute_toggle.isOn;

		if (MuteAll == true)
		{
			MasterVol = 0;
		}
		else
		{
			MasterVol = master_slider.value;
		}
		

		SavePlayerPrefs();
	}

	void SavePlayerPrefs()
	{
		PlayerPrefs.SetFloat("MouseSen", mouseSen);
		PlayerPrefs.SetFloat("ADSSen", ADSSen);

		PlayerPrefs.SetFloat("Sound", MasterVol);

		PlayerPrefs.SetInt("HPValBool", toggleHpVal);
	}

	public void LoadScene(int scene_index)
    {
		PlayerPrefs.Save();
        SceneManager.LoadScene(scene_index);
    }

    public void Quit()
    {
		PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
