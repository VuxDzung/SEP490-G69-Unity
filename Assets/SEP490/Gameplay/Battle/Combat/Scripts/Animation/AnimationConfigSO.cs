namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AnimationConfig.", menuName = OrganizationConstants.NAMESPACE + "/Combat/Animation config")]
    public class AnimationConfigSO : ScriptableObject
    {
        [SerializeField] private List<AnimationData> m_Animations;

        public IReadOnlyList<AnimationData> Animations => m_Animations;

        public AnimationData GetAnimationByName(string animName)
        {
            if (string.IsNullOrEmpty(animName)) return null;

            return m_Animations.FirstOrDefault(a => a.AnimationName.Equals(animName));
        }
    }

    [System.Serializable]
    public class AnimationData
    {
        [SerializeField] string animationName;
        [SerializeField] bool invert;
        //[SerializeField] Sprite poseSprite;
        [SerializeField] float duration;
        [SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] Vector2 velocity;

        public string AnimationName => animationName;
        //public Sprite PoseSprite => poseSprite;
        public float Duration => duration;
        public AnimationCurve Curve => curve;
        public Vector2 Velocity
        {
            get
            {
                return new Vector2(invert ? -1 : 1 * velocity.x, velocity.y);
            }
        }
    }
}