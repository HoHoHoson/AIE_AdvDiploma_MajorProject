using UnityEngine;

[System.Serializable]
public class SoundClip
{
    [SerializeField] private AudioSource    m_audioClip;
    [SerializeField] private float          m_minDelay = 0;
    [SerializeField] private float          m_maxDelay = 0;

    private AudioSource m_instantiated_sound;

    public float GetMinDelay() { return m_minDelay; }
    public float GetMaxDelay() { return m_maxDelay; }
    public AudioSource GetAudioSource() { return m_instantiated_sound; }

    public void InstantiateAudioSource(Transform parent_transform)
    {
        m_instantiated_sound = Object.Instantiate(m_audioClip, parent_transform);
    }

    public void PlayAudio()
    {
        m_instantiated_sound.PlayDelayed(Random.Range(m_minDelay, m_maxDelay));
    }
}
