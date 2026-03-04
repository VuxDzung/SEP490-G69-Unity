using SEP490G69;
using UnityEngine;

public class SceneAudioSetter : MonoBehaviour
{
    [SerializeField] private AudioClip m_BGM;

    void Start()
    {
        var audioManager = ContextManager.Singleton.ResolveGameContext<AudioManager>();

        // Load saved music volume
        int musicIndex = PlayerPrefs.GetInt("Music", 5);
        float musicVolume = musicIndex / 10f;

        audioManager.SetBGMVolume(musicVolume);
        audioManager.SetBG(m_BGM);
    }
}