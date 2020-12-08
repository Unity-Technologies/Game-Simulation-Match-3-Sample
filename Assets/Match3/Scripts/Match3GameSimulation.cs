using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: 0. Import Unity Game Simulation package

public class Match3GameSimulation : MonoBehaviour {

    [SerializeField] private List<LevelSO> levelList;
    [SerializeField] private Match3 match3;

    private LevelSO levelSO;

    private void Awake() {
        // TODO: 1. Load parameters for grid search

        // 5. Add event handler when winning/losing the game
        match3.OnWin += Match3_OnWin;
        match3.OnOutOfMoves += Match3_OnOutOfMoves;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        // TODO: 3a. Enable tracking metric levelSO.name_Win_Moves_Used when the game is won
        // TODO: 3b. Call EndGameSimulation()
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        // TODO: 4a. Enable tracking metric levelSO.name_Lose when the game is lost
        // TODO: 4b. Call EndGameSimulation()
    }

    /* // TODO: 2b. Uncomment this method
    private void OnFetchConfig(GameSimConfigResponse gameSimConfigResponse) {
        // TODO: 2a. Load parameter "level" for grid search and set it to a string variable "levelName"

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
