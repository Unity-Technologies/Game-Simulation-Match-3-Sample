using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Match3UI : MonoBehaviour {

    [SerializeField] private Match3 match3;

    private TextMeshProUGUI movesText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI targetScoreText;
    private TextMeshProUGUI glassText;
    private Transform winLoseTransform;

    private void Awake() {
        movesText = transform.Find("movesText").GetComponent<TextMeshProUGUI>();
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        glassText = transform.Find("glassText").GetComponent<TextMeshProUGUI>();
        targetScoreText = transform.Find("targetScoreText").GetComponent<TextMeshProUGUI>();

        winLoseTransform = transform.Find("winLose");
        winLoseTransform.gameObject.SetActive(false);

        match3.OnLevelSet += Match3_OnLevelSet;
        match3.OnMoveUsed += Match3_OnMoveUsed;
        match3.OnGlassDestroyed += Match3_OnGlassDestroyed;
        match3.OnScoreChanged += Match3_OnScoreChanged;

        match3.OnOutOfMoves += Match3_OnOutOfMoves;
        match3.OnWin += Match3_OnWin;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#1ACC23>YOU WIN!</color>";
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#CC411A>YOU LOSE!</color>";
    }

    private void Match3_OnScoreChanged(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void Match3_OnGlassDestroyed(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void Match3_OnMoveUsed(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void Match3_OnLevelSet(object sender, System.EventArgs e) {
        LevelSO levelSO = match3.GetLevelSO();

        switch (levelSO.goalType) {
            default:
            case LevelSO.GoalType.Glass:
                transform.Find("glassImage").gameObject.SetActive(true);
                glassText.gameObject.SetActive(true);

                targetScoreText.gameObject.SetActive(false);
                break;
            case LevelSO.GoalType.Score:
                transform.Find("glassImage").gameObject.SetActive(false);
                glassText.gameObject.SetActive(false);

                targetScoreText.gameObject.SetActive(true);

                targetScoreText.text = levelSO.targetScore.ToString();
                break;
        }

        UpdateText();
    }

    private void UpdateText() {
        movesText.text = match3.GetMoveCount().ToString();
        scoreText.text = match3.GetScore().ToString();
        glassText.text = match3.GetGlassAmount().ToString();
    }


}
