using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.NAME.Game
{
    public class GameSceneManager : MonoBehaviour
    {
        [Tooltip("The Scene to load when the player goes back to the Introduction Menu")]
        public SceneAsset IntroScene;
        [Tooltip("The Scene to load when the player wins")]
        public SceneAsset SuccessScene;
        [Tooltip("The Scene to load when the player loses")]
        public SceneAsset FailureScene;
        [Tooltip("All the levels of the game")]
        public List<SceneAsset> MainLevels;

        private static GameSceneManager s_Instance;

        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);

                EventManager.AddListener<GameStateChangeEvent>(OnGameStateChange);
                EventManager.AddListener<UserMenuActionEvent>(OnUserMenuAction);
            }
            else if (s_Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void OnGameStateChange(GameStateChangeEvent evt)
        {
            if (evt.NewGameState == GameState.Menu)
            {
                switch (evt.CurrentGameState)
                {
                    case GameState.Success: SceneManager.LoadScene(SuccessScene.name); break;
                    case GameState.Failure: SceneManager.LoadScene(FailureScene.name); break;
                    default: SceneManager.LoadScene(IntroScene.name); break;
                }
            }
        }

        void OnUserMenuAction(UserMenuActionEvent evt)
        {
            switch (evt.UserMenuAction)
            {
                case UserMenuAction.Play: SceneManager.LoadScene(MainLevels[0].name); break;
                case UserMenuAction.ReturnToIntroMenu: SceneManager.LoadScene(IntroScene.name); break;
            }
        }

        void OnDestroy()
        {
            if (s_Instance == this)
            {
                EventManager.RemoveListener<GameStateChangeEvent>(OnGameStateChange);
                EventManager.RemoveListener<UserMenuActionEvent>(OnUserMenuAction);
            }
        }
    }
}
