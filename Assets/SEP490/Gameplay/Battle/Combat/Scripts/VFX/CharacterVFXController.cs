namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CharacterVFXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_AtkVFX;
        [SerializeField] private ParticleSystem m_StunVFX;
        [SerializeField] private ParticleSystem m_BuffEffectVFX;

        public void PlayAtkVFX()
        {
            m_AtkVFX.Play();
        }

        public void PlayStunVFX()
        {
            m_StunVFX.Play();
        }
        public void StopStunVFX()
        {
            m_StunVFX.Stop();
        }

        public void PlayBuffVFX()
        {
            m_BuffEffectVFX.Play();
        }
        public void StopBuffVFX()
        {
            m_BuffEffectVFX.Stop();
        }
    }
}