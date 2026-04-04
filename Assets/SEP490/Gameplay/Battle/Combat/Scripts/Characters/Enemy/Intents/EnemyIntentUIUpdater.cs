namespace SEP490G69.Battle.Combat
{
    using TMPro;
    using UnityEngine;

    public class EnemyIntentUIUpdater : MonoBehaviour
    {
        [SerializeField] private Transform m_IntentUIPrefab;
        [SerializeField] private Transform m_IntentContainer;

        public void ClearIntents()
        {
            Debug.Log($"Clear UI intents.");

            if (PoolManager.Pools[GameConstants.POOL_ENEMY_UI_INTENT].Count > 0)
            {
                PoolManager.Pools[GameConstants.POOL_ENEMY_UI_INTENT].DespawnAll();
            }
        }

        public void MakeIntent(string content, Color textColor, Sprite typeSprite)
        {
            Debug.Log($"Make UI intent: {content}");
            Transform intentUITrans = PoolManager.Pools[GameConstants.POOL_ENEMY_UI_INTENT].Spawn(m_IntentUIPrefab, m_IntentContainer);
            UIIntentElement intentUI = intentUITrans.GetComponent<UIIntentElement>();
            if (intentUI != null)
            {
                intentUI.SetContent(content, textColor, typeSprite);
            }
        }
    }
}