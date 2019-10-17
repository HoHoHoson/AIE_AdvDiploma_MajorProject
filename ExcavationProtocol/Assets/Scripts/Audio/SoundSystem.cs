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

    private SoundClip   m_loaded_clip;
    private int         m_loop_index = -1;

    void Update()
    {
        if (m_soundClips.Length == 0)
        {
            Debug.Log("ERROR: SoundSystem contains no audio clips.");
            return;
        }

        switch (m_loopSetting)
        {
            case LoopMode.None:
                return;

            case LoopMode.Loop:
                {
                    if (IsPlaying() == true)
                        return;

                    m_loop_index = ((++m_loop_index % m_soundClips.Length) + m_soundClips.Length) % m_soundClips.Length;

                    m_loaded_clip = m_soundClips[m_loop_index];
                    PlayLoaded();

                    break;
                }

            case LoopMode.ShuffleLoop:
                {
                    if (IsPlaying() == true)
                        return;

                    m_loaded_clip = m_soundClips[Random.Range(0, m_soundClips.Length)];
                    PlayLoaded();

                    break;
                }

            default:
                Debug.Log("ERROR: SoundSystem switch has defaulted.");
                break;
        }
    }

    public void PlayClip(int index = 0)
    {
        int wrapped_index = ((index % m_soundClips.Length) + m_soundClips.Length) % m_soundClips.Length;
        SoundClip sound_clip = m_soundClips[wrapped_index];

        sound_clip.audioClip.PlayDelayed(Random.Range(sound_clip.minDelay, sound_clip.maxDelay));
    }

    public void PlayRandom()
    {
        SoundClip random_clip = m_soundClips[Random.Range(0, m_soundClips.Length)];

        random_clip.audioClip.PlayDelayed(Random.Range(random_clip.minDelay, random_clip.maxDelay));
    }

    private void PlayLoaded()
    {
        m_loaded_clip.audioClip.PlayDelayed(Random.Range(m_loaded_clip.minDelay, m_loaded_clip.maxDelay));
    }

    private void ClearPlaying()
    {
        if (m_loaded_clip == null)
            return;

        m_loaded_clip.audioClip.Stop();
        m_loaded_clip = null;
    }

    private bool IsPlaying()
    {
        return (m_loaded_clip != null && m_loaded_clip.audioClip.isPlaying);
    }

    [System.Serializable]
    private class SoundClip
    {
        public AudioSource audioClip;

        public float minDelay = 0;
        public float maxDelay = 0;
    }
}
