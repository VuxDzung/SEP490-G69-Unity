namespace SEP490G69.Calendar
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRewardPreview : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;

        [SerializeField] private TextMeshProUGUI m_NumericTmp;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Button m_DetailsBtn;

        private string _id;

        public void Spawn()
        {
            if (m_DetailsBtn != null) m_DetailsBtn.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            if (m_DetailsBtn != null) m_DetailsBtn.onClick.RemoveListener(Click);
        }

        public UIRewardPreview SetContent(string id, ERewardType rewardType, int amount)
        {
            _id = id;
            string amountStr = $"{amount.ToString()} {DeterminePostfix(rewardType)}";
            m_NumericTmp.text = amountStr;

            return this;
        }

        public UIRewardPreview SetClickDetailsCallback(Action<string> callback)
        {
            _onClick = callback;
            return this;
        }

        private string DeterminePostfix(ERewardType rewardType)
        {
            switch(rewardType)
            {
                case ERewardType.Gold:
                    return "G";
                case ERewardType.ReputationPoint:
                    return "RP";
                default:
                    return "";
            }
        }

        private void Click()
        {
            _onClick?.Invoke(_id);
        }
    }
}