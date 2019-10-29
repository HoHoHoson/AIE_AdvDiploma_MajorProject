using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    private enum LoopMode
    {
        None = 0,
        Loop,
        ShuffleLoop
    };

    [SerializeField] private LoopMode       m_loopSetting = 0;
    [SerializeField] private SoundClip[]    m_soundClips;

    private SoundClip       m_loaded_clip;
    private int             m_loop_index = -1;

    void Start()
    {
        if (IsEmpty())
            return;

        foreach (SoundClip sound_clip in m_soundClips)
            sound_clip.InstantiateAudioSource(transform);
    }

    void Update()
    {
        if (IsEmpty())
            return;

        switch (m_loopSetting)
        {
            case LoopMode.None:
                return;

            case LoopMode.Loop:
                {
                    if (IsLooping() == true)
                        return;

                    m_loop_index = ((++m_loop_index % m_soundClips.Length) + m_soundClips.Length) % m_soundClips.Length;

                    m_loaded_clip = m_soundClips[m_loop_index];
                    m_loaded_clip.PlayAudio();

                    break;
                }

            case LoopMode.ShuffleLoop:
                {
                    if (IsLooping() == true)
                        return;

                    m_loaded_clip = m_soundClips[Random.Range(0, m_soundClips.Length)];
                    m_loaded_clip.PlayAudio();

                    break;
                }

            default:
                Debug.Log("ERROR: SoundSystem switch has defaulted.");
                break;
        }
    }

    public SoundClip GetClip(int index = 0)
    {
        int wrapped_index = ((index % m_soundClips.Length) + m_soundClips.Length) % m_soundClips.Length;
        SoundClip sound_clip = m_soundClips[wrapped_index];

        return sound_clip;
    }

    public void PlayRandom()
    {
        SoundClip random_clip = m_soundClips[Random.Range(0, m_soundClips.Length)];

        random_clip.PlayAudio();
    }

    private void ClearAllPlaying()
    {
        foreach (SoundClip clip in m_soundClips)
            clip.GetAudioSource().Stop();
    }

    private bool IsLooping()
    {
        return (m_loaded_clip != null && m_loaded_clip.GetAudioSource().isPlaying);
    }

    private bool IsEmpty()
    {
        if (m_soundClips.Length == 0)
        {
            Debug.Log("ERROR: SoundSystem contains no audio clips.");
            return true;
        }

        return false;
    }
}
