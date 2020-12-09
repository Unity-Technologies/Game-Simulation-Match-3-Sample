using System.Collections.Generic;
using UnityEngine;
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

        // TODO: Add event handler when winning/losing the game
        // Match3 provides event handlers OnWin and OnOutOfMoves.
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        // TODO: Enable tracking metric Win_Moves_Used when the game is won
        // GameSimulation manages result data in 'counters'. They have an integral value type, and you may have any number
        // of them.
        //
        // GameSimManager.Instance.SetCounter(string name, long value)

        // TODO: Gracefully exit
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        // TODO: Enable tracking metric Lose when the game is lost
        // As above, you'll want to use SetCounter to track losing.

        // TODO: Gracefully exit
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
