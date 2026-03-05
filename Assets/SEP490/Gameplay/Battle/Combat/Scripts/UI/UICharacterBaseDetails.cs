namespace SEP490G69.Battle.Combat
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterBaseDetails : MonoBehaviour
    {
        [SerializeField] private Image m_CharImg;
        [SerializeField] private UITextSlider m_VitSlider;
        [SerializeField] private UITextSlider m_DefSlider;
        [SerializeField] private UITextSlider m_SpeedSlider;
        [SerializeField] private Transform m_StatEffectContainer;

        private string _characterId;

        public void SetContent(string characterId, Sprite avatar)
        {
            _characterId = characterId;
            m_CharImg.sprite = avatar;
        }

        public void SetVit(float cur, float max)
        {
            m_VitSlider.SetValue(cur, max);
        }
        public void SetStamina(float cur, float max)
        {
            m_DefSlider.SetValue(cur, max);
        }
        public void SetSpeed(float cur, float max)
        {
            m_SpeedSlider.SetValue(cur, max);
        }
    }
}