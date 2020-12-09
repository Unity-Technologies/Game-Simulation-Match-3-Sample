using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: 0. Import Unity Game Simulation package by uncommenting the line below
//using Unity.Simulation.Game

public class Match3GameSimulation : MonoBehaviour {

    [SerializeField] private List<LevelSO> levelList;
    [SerializeField] private Match3 match3;

    private LevelSO levelSO;

    private void Awake() {
        // TODO: 1. Load parameters for grid search
        // Reference: https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-3-load-parameters-for-grid-search
        // To load the parameters, call GameSimManager.Instance.FetchConfig(Action<GameSimConfigResponse>) here.
        // Action<GameSimConfigResponse> is a call back method, and we will name it OnFetchConfig, which will be implemented in TODO: 2a.

        // 5. Add event handler when winning/losing the game
        match3.OnWin += Match3_OnWin;
        match3.OnOutOfMoves += Match3_OnOutOfMoves;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        // TODO: 3a. Enable tracking metric Win_Moves_Used when the game is won
        // Reference: https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-4-enable-metrics-tracking
        // Game Simulation uses a counter to track metrics. You can set, increase, and reset a counter's value at any point of your game code.
        // When the the bot won the game, we want to get the number of moves used from "match3.GetUsedMoveCount()"
        // and set the value to a counter named "Win_Moves_Used"
        // You can set a counter's value by calling GameSimManager.Instance.SetCounter("counterName", counterValue);

        // TODO: 3b. Call method EndGameSimulation()
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        // TODO: 4a. Enable tracking metric Lose when the game is lost
        // Reference: https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-4-enable-metrics-tracking

        // TODO: 4b. Call method EndGameSimulation()
    }

    /* // TODO: 2a. Uncomment this method
    private void OnFetchConfig(GameSimConfigResponse gameSimConfigResponse) {
        // TODO: 2b. Load string type parameter "level" for grid search and set it to a string variable "levelName"
        // Reference: https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html#step-3-load-parameters-for-grid-search
        // Parameter values are stored in GameSimConfigResponse object.
        // You can access the individual variable by calling GameSimConfigResponse.Get[parameter type]("parameter name");

        LevelSO loadLevel = null;

        foreach (LevelSO levelSO in levelList) {
            if (levelSO.name == levelName) {
                loadLevel = levelSO;
                break;
            }
        }

        if (loadLevel == null) {
            Debug.Log("Could not find level with name: " + levelName);
            loadLevel = levelList[0];
        }

        this.levelSO = loadLevel;
        match3.SetLevelSO(loadLevel);
    }*/

    // 6. Gracefully end the simulation
    private void EndGameSimulation() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
