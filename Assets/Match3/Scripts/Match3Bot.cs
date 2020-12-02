using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Bot : MonoBehaviour {

    [SerializeField] private Match3 match3;
    [SerializeField] private Match3Visual match3Visual;

    private LevelSO levelSO;

    private void Awake() {
        match3Visual.OnStateChanged += Match3Visual_OnStateChanged;

        match3.OnOutOfMoves += Match3_OnOutOfMoves;
        match3.OnWin += Match3_OnWin;
        match3.OnLevelSet += Match3_OnLevelSet;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Manual Bot Move
            //BotDoMove();
        }
    }

    private void Match3_OnLevelSet(object sender, Match3.OnLevelSetEventArgs e) {
        levelSO = e.levelSO;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        // Win!
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        // Out of moves!
    }

    private void Match3Visual_OnStateChanged(object sender, System.EventArgs e) {
        switch (match3Visual.GetState()) {
            case Match3Visual.State.WaitingForUser:
                BotDoMove();
                break;
        }
    }

    private void BotDoMove() {
        // Try to solve
        List<Match3.PossibleMove> possibleMoveList = match3.GetAllPossibleMoves();

        Match3.PossibleMove bestPossibleMove = GetBestPossibleMove(possibleMoveList);

        if (bestPossibleMove == null) {
            Debug.LogError("Bot cannot find a possible move!");
        } else {
            match3Visual.SwapGridPositions(bestPossibleMove.startX, bestPossibleMove.startY, bestPossibleMove.endX, bestPossibleMove.endY);
        }
    }

    private Match3.PossibleMove GetBestPossibleMove(List<Match3.PossibleMove> possibleMoveList) {
        Match3.PossibleMove bestPossibleMove = null;

        switch (levelSO.goalType) {
            case LevelSO.GoalType.Score:
                foreach (Match3.PossibleMove possibleMove in possibleMoveList) {
                    if (bestPossibleMove == null) {
                        bestPossibleMove = possibleMove;
                    } else {
                        if (possibleMove.GetTotalMatchAmount() > bestPossibleMove.GetTotalMatchAmount()) {
                            // Better move
                            bestPossibleMove = possibleMove;
                        }
                    }
                }
                break;
            case LevelSO.GoalType.Glass:
                foreach (Match3.PossibleMove possibleMove in possibleMoveList) {
                    if (bestPossibleMove == null) {
                        bestPossibleMove = possibleMove;
                    } else {
                        if (possibleMove.GetTotalGlassAmount() > bestPossibleMove.GetTotalGlassAmount()) {
                            // Better move
                            bestPossibleMove = possibleMove;
                        }
                    }
                }
                break;
            default:
                Debug.Log("Bot does not recognize this goal type");
                break;
        }
        return bestPossibleMove;
    }

}
