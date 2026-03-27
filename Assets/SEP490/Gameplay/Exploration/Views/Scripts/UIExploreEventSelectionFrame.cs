namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UIExploreEventSelectionFrame : GameUIFrame
    {
        [SerializeField] private Transform m_EventContainer;
        [SerializeField] private Transform m_EventUIPrefab;

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
            if (PoolManager.Pools["UIEvent"].Count > 0)
            {
                PoolManager.Pools["UIEvent"].DespawnAll();
            }
        }

        public void DisplayEvents(List<ExploreEventSO> eventList)
        {
            foreach (var eventSO in eventList)
            {
                Transform eventUITrans = PoolManager.Pools["UIEvent"].Spawn(m_EventUIPrefab, m_EventContainer);
                UIExploreEventElement eventUI = eventUITrans.GetComponent<UIExploreEventElement>();
                if (eventUI != null)
                {
                    eventUI.SetOnClickCallback(SelectEventType).SetContent(eventSO.ExploreEventType, LocalizeManager);
                }
            }
        }

        private void SelectEventType(EExploreEventType eventType)
        {
            ExploreController.SelectEventType(eventType);
        }
    }
}