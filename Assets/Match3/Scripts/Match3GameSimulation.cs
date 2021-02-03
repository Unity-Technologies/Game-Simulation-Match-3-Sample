using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Simulation.Games;
// TODO: Game Simulation uses the following namespace: Unity.Simulation.Games
// Reference: https://docs.unity3d.com/Packages/com.unity.simulation.games@0.4/manual/ImplementationGuide.html

public class Match3GameSimulation : MonoBehaviour {

    [SerializeField] private List<LevelSO> levelList;
    [SerializeField] private Match3 match3;

    private LevelSO levelSO;

    private void Awake() {
        // TODO: Load parameters for grid search
        // GameSimManager.Instance.FetchConfig(Action<GameSimConfigResponse>) gets configuration information and takes
        // a callback you can use to modify game state based on the configuration of parameters for the run
        //
        // You'll need to call this method and provide a callback to initialize your state.
        // GameSimConfigResponse has a number of methods to get the value of some parameter.
        // In this example, the level is parameterized as a string, so you'll need to call the GetString(key) method on
        // the response object.
        //
        // This application loads level data by calling the SetLevelSO(LevelSO) method on the Match3 object.
        // Read the level from the game sim manager, find it in your levelList, and pass along the appropriate level to
        // your match3 instance.
        GameSimManager.Instance.FetchConfig(OnFetchConfig);

        // TODO: Add event handler when winning/losing the game
        // Match3 provides event handlers OnWin and OnOutOfMoves

        match3.OnWin += Match3_OnWin;
        match3.OnOutOfMoves += Match3_OnOutOfMoves;
        match3.OnGameEnd += Match3_OnGameEnd;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        // TODO: Enable tracking metric Win_Moves_Used when the game is won
        // GameSimulation manages result data in 'counters'. They have an integral value type, and you may have any number
        // of them.
        //
        // GameSimManager.Instance.SetCounter(string name, long value)
        GameSimManager.Instance.SetCounter(levelSO.name + "_Win_MovesUsed", match3.GetUsedMoveCount());
        // GameSimManager.Instance.SetCounter(levelSO.name + "_Score", match3.GetScore());
        // TODO: Gracefully exit
        EndGameSimulation();
    }

    private void Match3_OnGameEnd(object sender, System.EventArgs e) {
        // GameSimManager.Instance.SetCounter(levelSO.name + "_Win_MovesUsed", match3.GetUsedMoveCount());
        GameSimManager.Instance.SetCounter(levelSO.name + "_End_Score", match3.GetScore());
        EndGameSimulation();
    }


    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        // TODO: Enable tracking metric Lose when the game is lost
        // As above, you'll want to use SetCounter to track losing.
        GameSimManager.Instance.SetCounter(levelSO.name + "_Lose", 1);
        // TODO: Gracefully exit
        EndGameSimulation();
    }

    private void OnFetchConfig(GameSimConfigResponse gameSimConfigResponse) {
        string levelName = gameSimConfigResponse.GetString("level");

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
    }

    // Gracefully end the simulation
    private void EndGameSimulation() {
    // It's convenient for this to function in the expected manner in editor..
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

}
