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

    private SoundClip m_loaded_clip;
    //private SoundClip m_loaded_clip;

    void Update()
    {
        switch (m_loopSetting)
        {
            case LoopMode.None:
                return;

            case LoopMode.Loop:
                break;

            case LoopMode.ShuffleLoop:
                {
                    if (m_loaded_clip == null || m_loaded_clip.audioClip.isPlaying == false)
                        PlayRandom();

                    break;
                }

            default:
                Debug.Log("ERROR: SoundSystem switch has defaulted.");
                break;
        }
    }

    public void PlayRandom()
    {
        ClearPlaying();

        m_loaded_clip = m_soundClips[Random.Range(0, m_soundClips.Length)];

        m_loaded_clip.audioClip.PlayDelayed(Random.Range(m_loaded_clip.minDelay, m_loaded_clip.maxDelay));
    }

    private void ClearPlaying()
    {
        if (m_loaded_clip == null)
            return;

        m_loaded_clip.audioClip.Stop();
        m_loaded_clip = null;
    }

    [System.Serializable]
    private class SoundClip
    {
        public AudioSource audioClip;

        public float minDelay = 0;
        public float maxDelay = 0;
    }
}
