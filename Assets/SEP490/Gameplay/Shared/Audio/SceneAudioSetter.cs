using SEP490G69;
using UnityEngine;

public class SceneAudioSetter : MonoBehaviour
{
    [SerializeField] private AudioClip m_BGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ContextManager.Singleton.ResolveGameContext<AudioManager>().SetBG(m_BGM);
    }

}
