namespace SEP490G69.Authentication
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Shared;
    using UnityEngine;

    public class AuthSceneManager : MonoBehaviour, ISceneContext
    {
        [SerializeField] private GameUIManager m_UIManager;

        private GameAuthManager _authManager;
        
        public GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        private void OnEnable()
        {
            _authManager.FirebaseAuthService.OnAutoLoginStarted += FirebaseAuthService_OnAutoLoginStarted;
            _authManager.FirebaseAuthService.OnAutoLoginSuccess += FirebaseAuthService_OnAutoLoginSuccess; ;
            _authManager.FirebaseAuthService.OnAutoLoginFailed += FirebaseAuthService_OnAutoLoginFailed;
        }

        private void OnDisable()
        {
            _authManager.FirebaseAuthService.OnAutoLoginStarted -= FirebaseAuthService_OnAutoLoginStarted;
            _authManager.FirebaseAuthService.OnAutoLoginSuccess -= FirebaseAuthService_OnAutoLoginSuccess; ;
            _authManager.FirebaseAuthService.OnAutoLoginFailed -= FirebaseAuthService_OnAutoLoginFailed;
        }

        private void FirebaseAuthService_OnAutoLoginStarted()
        {
            m_UIManager.ShowFrame(GameConstants.FRAME_ID_LOADING).AsFrame<UILoadingScreen>().SetText("Auto login...");
        }

        private void FirebaseAuthService_OnAutoLoginSuccess(Firebase.Auth.FirebaseUser user)
        {
            
        }

        private void FirebaseAuthService_OnAutoLoginFailed(AuthErrorInfo info)
        {
            m_UIManager.HideFrame(GameConstants.FRAME_ID_LOADING);

            m_UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }
    }
}