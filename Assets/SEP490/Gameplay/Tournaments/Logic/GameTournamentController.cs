namespace SEP490G69.Tournament
{
    using UnityEngine;

    public class GameTournamentController : MonoBehaviour, ISceneContext
    {
        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        public void LoadTournamentData()
        {

        }
    }
}