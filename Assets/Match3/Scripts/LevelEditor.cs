using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;

/*
 * Controls: 
 *   Number Keys 1-5 = Set Gem Type
 *   Right Click = Toggle Glass
 * */
public class LevelEditor : MonoBehaviour {

    [SerializeField] private LevelSO levelSO;
    [SerializeField] private Transform pfGemGridVisual;
    [SerializeField] private Transform pfGlassGridVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI levelText;

    private Grid<GridPosition> grid;

    private void Awake() {
        grid = new Grid<GridPosition>(levelSO.width, levelSO.height, 1f, Vector3.zero, (Grid<GridPosition> g, int x, int y) => new GridPosition(levelSO, g, x, y));

        levelText.text = levelSO.name;

        if (levelSO.levelGridPositionList == null || levelSO.levelGridPositionList.Count != levelSO.width * levelSO.height) {
            // Create new Level
            Debug.Log("Creating new level...");
            levelSO.levelGridPositionList = new List<LevelSO.LevelGridPosition>();

            for (int x = 0; x < grid.GetWidth(); x++) {
                for (int y = 0; y < grid.GetHeight(); y++) {
                    GemSO gem = levelSO.gemList[Random.Range(0, levelSO.gemList.Count)];

                    LevelSO.LevelGridPosition levelGridPosition = new LevelSO.LevelGridPosition { x = x, y = y, gemSO = gem };
                    levelSO.levelGridPositionList.Add(levelGridPosition);

                    CreateVisual(grid.GetGridObject(x, y), levelGridPosition);
                }
            }
        } else {
            // Load Level
            Debug.Log("Loading level...");
            for (int x = 0; x < grid.GetWidth(); x++) {
                for (int y = 0; y < grid.GetHeight(); y++) {

                    LevelSO.LevelGridPosition levelGridPosition = null;

                    foreach (LevelSO.LevelGridPosition tmpLevelGridPosition in levelSO.levelGridPositionList) {
                        if (tmpLevelGridPosition.x == x && tmpLevelGridPosition.y == y) {
                            levelGridPosition = tmpLevelGridPosition;
                            break;
                        }
                    }

                    if (levelGridPosition == null) {
                        Debug.LogError("Error! Null!");
                    }

                    CreateVisual(grid.GetGridObject(x, y), levelGridPosition);
                }
            }
        }

        cameraTransform.position = new Vector3(grid.GetWidth() * .5f, grid.GetHeight() * .5f, cameraTransform.position.z);
    }

    private void Update() {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

        grid.GetXY(mouseWorldPosition, out int x, out int y);

        if (IsValidPosition(x, y)) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[0]);
            if (Input.GetKeyDown(KeyCode.Alpha2)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[1]);
            if (Input.GetKeyDown(KeyCode.Alpha3)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[2]);
            if (Input.GetKeyDown(KeyCode.Alpha4)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[3]);
            if (Input.GetKeyDown(KeyCode.Alpha5)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[4]);

            if (Input.GetMouseButtonDown(1)) {
                grid.GetGridObject(x, y).SetHasGlass(!grid.GetGridObject(x, y).GetHasGlass());
            }
        }
    }

    private void CreateVisual(GridPosition gridPosition, LevelSO.LevelGridPosition levelGridPosition) {
        Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, gridPosition.GetWorldPosition(), Quaternion.identity);
        Transform glassGridVisualTransform = Instantiate(pfGlassGridVisual, gridPosition.GetWorldPosition(), Quaternion.identity);

        gridPosition.spriteRenderer = gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>();
        gridPosition.glassVisualGameObject = glassGridVisualTransform.gameObject;
        gridPosition.levelGridPosition = levelGridPosition;

        gridPosition.SetGemSO(levelGridPosition.gemSO);
        gridPosition.SetHasGlass(levelGridPosition.hasGlass);
    }

    private bool IsValidPosition(int x, int y) {
        if (x < 0 || y < 0 ||
            x >= grid.GetWidth() || y >= grid.GetHeight()) {
            // Invalid position
            return false;
        } else {
            return true;
        }
    }

    private class GridPosition {

        public SpriteRenderer spriteRenderer;
        public LevelSO.LevelGridPosition levelGridPosition;
        public GameObject glassVisualGameObject;

        private LevelSO levelSO;
        private Grid<GridPosition> grid;
        private int x;
        private int y;

        public GridPosition(LevelSO levelSO, Grid<GridPosition> grid, int x, int y) {
            this.levelSO = levelSO;
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public Vector3 GetWorldPosition() {
            return grid.GetWorldPosition(x, y);
        }

        public void SetGemSO(GemSO gemSO) {
            spriteRenderer.sprite = gemSO.sprite;
            levelGridPosition.gemSO = gemSO;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(levelSO);
#endif
        }

        public void SetHasGlass(bool hasGlass) {
            levelGridPosition.hasGlass = hasGlass;
            glassVisualGameObject.SetActive(hasGlass);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(levelSO);
#endif
        }

        public bool GetHasGlass() {
            return levelGridPosition.hasGlass;
        }

    }

}
