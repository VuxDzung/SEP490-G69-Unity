namespace SEP490G69.Exploration
{
    using TMPro;
    using UnityEngine;

    public class UIEventChoiceSelectFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_OutcomeTmp;
        [SerializeField] private Transform m_ChoicePrefab;
        [SerializeField] private Transform m_ChoiceContainer;
        private GameExploreController _exploreController;
        private GameExploreController ExploreController
        {
            get
            {
                if (_exploreController == null)
                {
                    _exploreController = ContextManager.Singleton.GetSceneContext<GameExploreController>();
                }
                return _exploreController;
            }
        }
        protected override void OnFrameShown()
        {
            base.OnFrameShown();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            if (PoolManager.Pools["UIChoiceEvent"].Count > 0)
            {
                PoolManager.Pools["UIChoiceEvent"].DespawnAll();
            }
        }

        public UIEventChoiceSelectFrame LoadChoices(ExploreEventSO eventSO)
        {
            m_OutcomeTmp.text = eventSO.Description;

            for (int i = 0; i < eventSO.Choices.Count; i++)// choice in eventSO.Choices)
            {
                Transform choiceUITrans = PoolManager.Pools["UIChoiceEvent"].Spawn(m_ChoicePrefab, m_ChoiceContainer);
                UIChoiceElement choiceUI = choiceUITrans.GetComponent<UIChoiceElement>();
                if (choiceUI != null)
                {
                    choiceUI.SetContent(SelectContent, eventSO.EventId, i, eventSO.Choices[i].ChoicesName);
                }
            }
            return this;
        }

        private void SelectContent(string eventId, int choiceIndex)
        {
            ExploreController.SelectChoiceOfEvent(eventId, choiceIndex);
        }
    }
}