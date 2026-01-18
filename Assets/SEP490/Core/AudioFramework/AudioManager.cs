namespace SEP490G69
{
    using UnityEngine;

    public class AudioManager : GlobalSingleton<AudioManager>
    {
        [SerializeField] private AudioSource m_BgSource;
        [SerializeField] private AudioSource m_AudioSource;

        protected override void CreateNewInstance()
        {
            AddAudioInstances();
        }

        public static void PlaySFX(AudioClip clip)
        {
            Singleton.m_AudioSource.PlayOneShot(clip, 1);
        }

        public void SetBG(AudioClip clip, bool repeat = true)
        {
            Singleton.m_BgSource.clip = clip;
            Singleton.m_BgSource.loop = repeat;
            Singleton.m_BgSource.Play();
        }

        private void AddAudioInstances()
        {
            GameObject sfxGO = new GameObject("[Audio] SFX");
            sfxGO.transform.SetParent(this.transform);
            m_AudioSource = sfxGO.AddComponent<AudioSource>();

            GameObject bgGO = new GameObject("[Audio] BG");
            bgGO.transform.SetParent(this.transform);
            m_BgSource = bgGO.AddComponent<AudioSource>();
        }

        public void SetBGMVolume(float volume)
        {
            if (m_BgSource != null) m_BgSource.volume = volume;
        }
        public void SetSFXVolume(float volume)
        {
            if (m_AudioSource != null) m_AudioSource.volume = volume;
        }
    }
}