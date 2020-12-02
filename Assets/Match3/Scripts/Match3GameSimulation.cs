using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Simulation.Games;

public class Match3GameSimulation : MonoBehaviour {

    [SerializeField] private List<LevelSO> levelList;
    [SerializeField] private Match3 match3;

    private LevelSO levelSO;

    private void Awake() {
        GameSimManager.Instance.FetchConfig(OnFetchConfig);

        match3.OnWin += Match3_OnWin;
        match3.OnOutOfMoves += Match3_OnOutOfMoves;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        GameSimManager.Instance.SetCounter(levelSO.name + "_Win_MovesUsed", match3.GetUsedMoveCount());
        EndGameSimulation();
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        GameSimManager.Instance.SetCounter(levelSO.name + "_Lose", 1);
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

    private void EndGameSimulation() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
