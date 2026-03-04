namespace SEP490G69.Exploration
{
    using UnityEngine;

    public class GameExploreController : MonoBehaviour, ISceneContext
    {
        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }
    }
}