using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    Quaternion start_rotation;
    Quaternion target_rotation;

    bool rotating = false;
    float rotation_duration;
    float rotation_timer;

    public void Rotate(float clockwise_angle, float time)
    {
        start_rotation = transform.rotation;
        target_rotation = start_rotation * Quaternion.Euler(0.0f, clockwise_angle, 0.0f);
        rotating = true;
        rotation_duration = time;
        rotation_timer = 0.0f;
    }


    public void SetDuration(float a_Duration)
    {
        Debug.Log("SetDuration() called " + a_Duration.ToString());
        rotation_duration = a_Duration;
    }

    public void Rotate(float clockwise_angle)
    {
        Rotate(clockwise_angle, rotation_duration);
    }


    private void Update()
    {
        if (rotating)
        {
            rotation_timer += Time.deltaTime;

            float ratio = 1 - ((Mathf.Cos(rotation_timer / rotation_duration * Mathf.PI) + 1) * 0.5f);

            transform.rotation =  Quaternion.Slerp(start_rotation, target_rotation, ratio);
            if (rotation_timer >= rotation_duration)
            {
                rotating = false;
            }
        }
    }
}
