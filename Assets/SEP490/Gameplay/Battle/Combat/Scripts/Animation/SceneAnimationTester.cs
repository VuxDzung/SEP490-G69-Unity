namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class SceneAnimationTester : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationController m_Player;
        [SerializeField] private CharacterAnimationController m_Enemy;

        public void PlayAnimations()
        {
            if (m_Player) m_Player.PlayAnimation();
            if (m_Enemy) m_Enemy.PlayAnimation();
            CombatCameraController.Singleton.ZoomCamera(true);
            CombatCameraController.Singleton.ShakeCamera();
        }
    }
}