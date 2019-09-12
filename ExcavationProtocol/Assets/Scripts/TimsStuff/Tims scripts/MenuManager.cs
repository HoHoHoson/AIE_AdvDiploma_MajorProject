using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadScene(int scene_index)
    {
        SceneManager.LoadScene(scene_index);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
