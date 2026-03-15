namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CharacterVFXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_AtkVFX;

        public void PlayAtkVFX()
        {
            m_AtkVFX.Play();
        }
    }
}