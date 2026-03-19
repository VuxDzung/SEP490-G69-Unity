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
            if (m_AtkVFX != null)
            {
                m_AtkVFX.Play();
            }
            
        }

        public void PlayStunVFX()
        {
            if (m_StunVFX != null)
            {
                m_StunVFX.Play();
            }
        }
        public void StopStunVFX()
        {
            if(m_StunVFX != null)
            {
                m_StunVFX.Stop();
            }
        }

        public void PlayBuffVFX()
        {
            if(m_BuffEffectVFX != null) 
            {
                m_BuffEffectVFX.Play();
            }
        }
        public void StopBuffVFX()
        {
            if(m_BuffEffectVFX != null)
            {
                m_BuffEffectVFX.Stop();
            }
            
        }
    }
}