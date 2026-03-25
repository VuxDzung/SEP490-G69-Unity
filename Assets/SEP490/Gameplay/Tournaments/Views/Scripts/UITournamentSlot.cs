namespace SEP490G69
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UITournamentSlot : MonoBehaviour
    {
        [SerializeField] private Image m_AvatarImg;
        [SerializeField] private Image m_Border;

        public void SetSlot(Sprite characterImg, bool isPlayer)
        {
            m_AvatarImg.sprite = characterImg;
            m_Border.gameObject.SetActive(isPlayer);
        }
    }
}