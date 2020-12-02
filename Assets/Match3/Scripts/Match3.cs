using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Represents the underlying Grid logic
 * */
public class Match3 : MonoBehaviour {

    public event EventHandler OnGemGridPositionDestroyed;
    public event EventHandler<OnNewGemGridSpawnedEventArgs> OnNewGemGridSpawned;
    public event EventHandler<OnLevelSetEventArgs> OnLevelSet;
    public event EventHandler OnGlassDestroyed;
    public event EventHandler OnMoveUsed;
    public event EventHandler OnOutOfMoves;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnWin;

    public class OnNewGemGridSpawnedEventArgs : EventArgs {
        public GemGrid gemGrid;
        public GemGridPosition gemGridPosition;
    }

    public class OnLevelSetEventArgs : EventArgs {
        public LevelSO levelSO;
        public Grid<GemGridPosition> grid;
    }

    [SerializeField] private LevelSO levelSO;
    [SerializeField] private bool autoLoadLevel;
    [SerializeField] private bool match4Explosions; // Explode neighbour nodes on 4 match

    private int gridWidth;
    private int gridHeight;
    private Grid<GemGridPosition> grid;
    private int score;
    private int moveCount;

    private void Start() {
        if (autoLoadLevel) {
            SetLevelSO(levelSO);
        }
    }

    public LevelSO GetLevelSO() {
        return levelSO;
    }

    public void SetLevelSO(LevelSO levelSO) {
        this.levelSO = levelSO;

        gridWidth = levelSO.width;
        gridHeight = levelSO.height;
        grid = new Grid<GemGridPosition>(gridWidth, gridHeight, 1f, Vector3.zero, (Grid<GemGridPosition> g, int x, int y) => new GemGridPosition(g, x, y));

        // Initialize Grid
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {

                // Get Saved LevelGridPosition
                LevelSO.LevelGridPosition levelGridPosition = null;

                foreach (LevelSO.LevelGridPosition tmpLevelGridPosition in levelSO.levelGridPositionList) {
                    if (tmpLevelGridPosition.x == x && tmpLevelGridPosition.y == y) {
                        levelGridPosition = tmpLevelGridPosition;
                        break;
                    }
                }

                if (levelGridPosition == null) {
                    // Couldn't find LevelGridPosition with this x, y!
                    Debug.LogError("Error! Null!");
                }

                GemSO gem = levelGridPosition.gemSO;
                GemGrid gemGrid = new GemGrid(gem, x, y);
                grid.GetGridObject(x, y).SetGemGrid(gemGrid);
                grid.GetGridObject(x, y).SetHasGlass(levelGridPosition.hasGlass);
            }
        }

        score = 0;
        moveCount = levelSO.moveAmount;

        OnLevelSet?.Invoke(this, new OnLevelSetEventArgs { levelSO = levelSO, grid = grid });
    }

    public int GetScore() {
        return score;
    }

    public bool HasMoveAvailable() {
        return moveCount > 0;
    }

    public int GetMoveCount() {
        return moveCount;
    }

    public int GetUsedMoveCount() {
        return levelSO.moveAmount - moveCount;
    }

    public void UseMove() {
        moveCount--;
        OnMoveUsed?.Invoke(this, EventArgs.Empty);
    }

    public int GetGlassAmount() {
        int glassAmount = 0;
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
                if (gemGridPosition.HasGlass()) {
                    glassAmount++;
                }
            }
        }
        return glassAmount;
    }

    public bool CanSwapGridPositions(int startX, int startY, int endX, int endY) {
        if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY)) return false; // Invalid Position

        if (startX == endX && startY == endY) return false; // Same Position

        SwapGridPositions(startX, startY, endX, endY); // Swap

        bool hasLinkAfterSwap = HasMatch3Link(startX, startY) || HasMatch3Link(endX, endY);

        SwapGridPositions(startX, startY, endX, endY); // Swap Back

        return hasLinkAfterSwap;
    }

    public void SwapGridPositions(int startX, int startY, int endX, int endY) {
        if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY)) return; // Invalid Position

        if (startX == endX && startY == endY) return; // Same Position

        GemGridPosition startGemGridPosition = grid.GetGridObject(startX, startY);
        GemGridPosition endGemGridPosition = grid.GetGridObject(endX, endY);

        GemGrid startGemGrid = startGemGridPosition.GetGemGrid();
        GemGrid endGemGrid = endGemGridPosition.GetGemGrid();

        startGemGrid.SetGemXY(endX, endY);
        endGemGrid.SetGemXY(startX, startY);

        startGemGridPosition.SetGemGrid(endGemGrid);
        endGemGridPosition.SetGemGrid(startGemGrid);
    }

    public void TestingGetAllMatch3Links(int startX, int startY, int endX, int endY) {
        SwapGridPositions(startX, startY, endX, endY); // Swap

        List<List<GemGridPosition>> allLinkedGemGridPositionList = GetAllMatch3Links();
        Debug.Log("allLinkedGemGridPositionList.Count: " + allLinkedGemGridPositionList.Count);

        foreach (List<GemGridPosition> linkedGemGridPositionList in allLinkedGemGridPositionList) {
            Debug.Log("linkedGemGridPositionList");
            foreach (GemGridPosition gemGridPosition in linkedGemGridPositionList) {
                Debug.Log(gemGridPosition.GetWorldPosition());
            }
        }

        SwapGridPositions(startX, startY, endX, endY); // Swap Back
    }

    public bool TryFindMatchesAndDestroyThem() {
        List<List<GemGridPosition>> allLinkedGemGridPositionList = GetAllMatch3Links();

        bool foundMatch = false;

        List<Vector2Int> explosionGridPositionList = new List<Vector2Int>();

        foreach (List<GemGridPosition> linkedGemGridPositionList in allLinkedGemGridPositionList) {
            foreach (GemGridPosition gemGridPosition in linkedGemGridPositionList) {
                TryDestroyGemGridPosition(gemGridPosition);
            }

            if (linkedGemGridPositionList.Count >= 4) {
                // More than 4 linked
                score += 200;

                // Special Explosion Gem
                GemGridPosition explosionOriginGemGridPosition = linkedGemGridPositionList[0];

                int explosionX = explosionOriginGemGridPosition.GetX();
                int explosionY = explosionOriginGemGridPosition.GetY();

                // Explode all 8 neighbours
                explosionGridPositionList.Add(new Vector2Int(explosionX - 1, explosionY - 1));
                explosionGridPositionList.Add(new Vector2Int(explosionX + 0, explosionY - 1));
                explosionGridPositionList.Add(new Vector2Int(explosionX + 1, explosionY - 1));
                explosionGridPositionList.Add(new Vector2Int(explosionX - 1, explosionY + 0));
                explosionGridPositionList.Add(new Vector2Int(explosionX + 1, explosionY + 0));
                explosionGridPositionList.Add(new Vector2Int(explosionX - 1, explosionY + 1));
                explosionGridPositionList.Add(new Vector2Int(explosionX + 0, explosionY + 1));
                explosionGridPositionList.Add(new Vector2Int(explosionX + 1, explosionY + 1));
            }

            foundMatch = true;
        }

        bool spawnExplosion = match4Explosions;

        if (spawnExplosion) {
            foreach (Vector2Int explosionGridPosition in explosionGridPositionList) {
                if (IsValidPosition(explosionGridPosition.x, explosionGridPosition.y)) {
                    GemGridPosition gemGridPosition = grid.GetGridObject(explosionGridPosition.x, explosionGridPosition.y);
                    TryDestroyGemGridPosition(gemGridPosition);
                }
            }
        }

        OnScoreChanged?.Invoke(this, EventArgs.Empty);

        return foundMatch;
    }

    private void TryDestroyGemGridPosition(GemGridPosition gemGridPosition) {
        if (gemGridPosition.HasGemGrid()) {
            score += 100;

            gemGridPosition.DestroyGem();
            OnGemGridPositionDestroyed?.Invoke(gemGridPosition, EventArgs.Empty);
            gemGridPosition.ClearGemGrid();
        }

        if (gemGridPosition.HasGlass()) {
            score += 100;

            gemGridPosition.DestroyGlass();
            OnGlassDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SpawnNewMissingGridPositions() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

                if (gemGridPosition.IsEmpty()) {
                    GemSO gem = levelSO.gemList[UnityEngine.Random.Range(0, levelSO.gemList.Count)];
                    GemGrid gemGrid = new GemGrid(gem, x, y);

                    gemGridPosition.SetGemGrid(gemGrid);

                    OnNewGemGridSpawned?.Invoke(gemGrid, new OnNewGemGridSpawnedEventArgs {
                        gemGrid = gemGrid,
                        gemGridPosition = gemGridPosition,
                    });
                }
            }
        }
    }

    public void FallGemsIntoEmptyPositions() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

                if (!gemGridPosition.IsEmpty()) {
                    // Grid Position has Gem
                    for (int i = y - 1; i >= 0; i--) {
                        GemGridPosition nextGemGridPosition = grid.GetGridObject(x, i);
                        if (nextGemGridPosition.IsEmpty()) {
                            gemGridPosition.GetGemGrid().SetGemXY(x, i);
                            nextGemGridPosition.SetGemGrid(gemGridPosition.GetGemGrid());
                            gemGridPosition.ClearGemGrid();

                            gemGridPosition = nextGemGridPosition;
                        } else {
                            // Next Grid Position is not empty, stop looking
                            break;
                        }
                    }
                }
            }
        }
    }

    public bool HasMatch3Link(int x, int y) {
        List<GemGridPosition> linkedGemGridPositionList = GetMatch3Links(x, y);
        return linkedGemGridPositionList != null && linkedGemGridPositionList.Count >= 3;
    }

    public List<GemGridPosition> GetMatch3Links(int x, int y) {
        GemSO gemSO = GetGemSO(x, y);

        if (gemSO == null) return null;

        int rightLinkAmount = 0;
        for (int i = 1; i < gridWidth; i++) {
            if (IsValidPosition(x + i, y)) {
                GemSO nextGemSO = GetGemSO(x + i, y);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    rightLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int leftLinkAmount = 0;
        for (int i = 1; i < gridWidth; i++) {
            if (IsValidPosition(x - i, y)) {
                GemSO nextGemSO = GetGemSO(x - i, y);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    leftLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int horizontalLinkAmount = 1 + leftLinkAmount + rightLinkAmount; // This Gem + left + right

        if (horizontalLinkAmount >= 3) {
            // Has 3 horizontal linked gems
            List<GemGridPosition> linkedGemGridPositionList = new List<GemGridPosition>();
            int leftMostX = x - leftLinkAmount;
            for (int i = 0; i < horizontalLinkAmount; i++) {
                linkedGemGridPositionList.Add(grid.GetGridObject(leftMostX + i, y));
            }
            return linkedGemGridPositionList;
        }


        int upLinkAmount = 0;
        for (int i = 1; i < gridHeight; i++) {
            if (IsValidPosition(x, y + i)) {
                GemSO nextGemSO = GetGemSO(x, y + i);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    upLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int downLinkAmount = 0;
        for (int i = 1; i < gridHeight; i++) {
            if (IsValidPosition(x, y - i)) {
                GemSO nextGemSO = GetGemSO(x, y - i);
                if (nextGemSO == gemSO) {
                    // Same Gem
                    downLinkAmount++;
                } else {
                    // Not same Gem
                    break;
                }
            } else {
                // Invalid position
                break;
            }
        }

        int verticalLinkAmount = 1 + downLinkAmount + upLinkAmount; // This Gem + down + up

        if (verticalLinkAmount >= 3) {
            // Has 3 vertical linked gems
            List<GemGridPosition> linkedGemGridPositionList = new List<GemGridPosition>();
            int downMostY = y - downLinkAmount;
            for (int i = 0; i < verticalLinkAmount; i++) {
                linkedGemGridPositionList.Add(grid.GetGridObject(x, downMostY + i));
            }
            return linkedGemGridPositionList;
        }

        // No links
        return null;
    }

    public List<List<GemGridPosition>> GetAllMatch3Links() {
        // Finds all the links with the current grid
        List<List<GemGridPosition>> allLinkedGemGridPositionList = new List<List<GemGridPosition>>();

        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                if (HasMatch3Link(x, y)) {
                    List<GemGridPosition> linkedGemGridPositionList = GetMatch3Links(x, y);

                    if (allLinkedGemGridPositionList.Count == 0) {
                        // First one
                        allLinkedGemGridPositionList.Add(linkedGemGridPositionList);
                    } else {
                        bool uniqueNewLink = true;

                        foreach (List<GemGridPosition> tmpLinkedGemGridPositionList in allLinkedGemGridPositionList) {
                            if (linkedGemGridPositionList.Count == tmpLinkedGemGridPositionList.Count) {
                                // Same number of links
                                // Are they all the same?
                                bool allTheSame = true;
                                for (int i = 0; i < linkedGemGridPositionList.Count; i++) {
                                    if (linkedGemGridPositionList[i] == tmpLinkedGemGridPositionList[i]) {
                                        // This one is the same, link is not unique
                                    } else {
                                        // These don't match
                                        allTheSame = false;
                                        break;
                                    }
                                }

                                if (allTheSame) {
                                    // Nodes are all the same, not a new unique link
                                    uniqueNewLink = false;
                                }
                            }
                        }

                        // Add to the total list if it's a unique link
                        if (uniqueNewLink) {
                            allLinkedGemGridPositionList.Add(linkedGemGridPositionList);
                        }
                    }
                }
            }
        }

        return allLinkedGemGridPositionList;
    }

    public List<PossibleMove> GetAllPossibleMoves() {
        List<PossibleMove> allPossibleMovesList = new List<PossibleMove>();

        // Test the Horizontal Axis first, prioritize nodes lower on the grid
        for (int y = 0; y < gridHeight; y++) {
            for (int x = 0; x < gridWidth; x++) {
                // Test Swap: Left, Right, Up, Down
                List<PossibleMove> testPossibleMoveList = new List<PossibleMove>();
                testPossibleMoveList.Add(new PossibleMove(x, y, x - 1, y + 0));
                testPossibleMoveList.Add(new PossibleMove(x, y, x + 1, y + 0));
                testPossibleMoveList.Add(new PossibleMove(x, y, x + 0, y + 1));
                testPossibleMoveList.Add(new PossibleMove(x, y, x + 0, y - 1));

                for (int i=0; i<testPossibleMoveList.Count; i++) {
                    PossibleMove possibleMove = testPossibleMoveList[i];

                    bool skipPossibleMove = false;

                    for (int j = 0; j < allPossibleMovesList.Count; j++) {
                        PossibleMove tmpPossibleMove = allPossibleMovesList[j];
                        if (tmpPossibleMove.startX == possibleMove.startX &&
                            tmpPossibleMove.startY == possibleMove.startY &&
                            tmpPossibleMove.endX == possibleMove.endX &&
                            tmpPossibleMove.endY == possibleMove.endY) {
                            // Already tested this combo
                            skipPossibleMove = true;
                            break;
                        }
                        if (tmpPossibleMove.startX == possibleMove.endX &&
                            tmpPossibleMove.startY == possibleMove.endY &&
                            tmpPossibleMove.endX == possibleMove.startX &&
                            tmpPossibleMove.endY == possibleMove.startY) {
                            // Already tested this combo
                            skipPossibleMove = true;
                            break;
                        }
                    }

                    if (skipPossibleMove) {
                        continue;
                    }

                    SwapGridPositions(possibleMove.startX, possibleMove.startY, possibleMove.endX, possibleMove.endY); // Swap

                    List<List<GemGridPosition>> allLinkedGemGridPositionList = GetAllMatch3Links();

                    if (allLinkedGemGridPositionList.Count > 0) {
                        // Making this Move results in a Match
                        possibleMove.allLinkedGemGridPositionList = allLinkedGemGridPositionList;
                        allPossibleMovesList.Add(possibleMove);
                    }

                    SwapGridPositions(possibleMove.startX, possibleMove.startY, possibleMove.endX, possibleMove.endY); // Swap Back
                }

            }
        }

        return allPossibleMovesList;
    }

    private GemSO GetGemSO(int x, int y) {
        if (!IsValidPosition(x, y)) return null;

        GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

        if (gemGridPosition.GetGemGrid() == null) return null;

        return gemGridPosition.GetGemGrid().GetGem();
    }

    private bool IsValidPosition(int x, int y) {
        if (x < 0 || y < 0 ||
            x >= gridWidth || y >= gridHeight) {
            // Invalid position
            return false;
        } else {
            return true;
        }
    }

    public bool TryIsGameOver() {
        if (!HasMoveAvailable()) {
            // No more moves, game over!
            OnOutOfMoves?.Invoke(this, EventArgs.Empty);
            return true;
        }

        switch (levelSO.goalType) {
            default:
            case LevelSO.GoalType.Score:
                if (score >= levelSO.targetScore) {
                    // Reached Target Score!
                    OnWin?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                break;
            case LevelSO.GoalType.Glass:
                if (GetGlassAmount() <= 0) {
                    // All glass destroyed!
                    OnWin?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                break;
        }

        // Not game over
        return false;
    }










    /*
     * Possible Move and what matches would happen if this move was made
     * */
    public class PossibleMove {

        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public List<List<GemGridPosition>> allLinkedGemGridPositionList;

        public PossibleMove() { }

        public PossibleMove(int startX, int startY, int endX, int endY) {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }

        public int GetTotalMatchAmount() {
            int total = 0;
            foreach (List<GemGridPosition> linkedGemGridPositionList in allLinkedGemGridPositionList) {
                total += linkedGemGridPositionList.Count;
            }
            return total;
        }

        public int GetTotalGlassAmount() {
            int total = 0;
            foreach (List<GemGridPosition> linkedGemGridPositionList in allLinkedGemGridPositionList) {
                foreach (GemGridPosition gemGridPosition in linkedGemGridPositionList) {
                    if (gemGridPosition.HasGlass()) {
                        total++;
                    }
                }
            }
            return total;
        }

        public override string ToString() {
            return startX + ", " + startY + " => " + endX + ", " + endY + " == " + allLinkedGemGridPositionList?.Count;
        }

    }


    /*
     * Represents a single Grid Position
     * Only the Grid Position which may or may not have an actual Gem on it
     * */
    public class GemGridPosition {

        public event EventHandler OnGlassDestroyed;

        private GemGrid gemGrid;

        private Grid<GemGridPosition> grid;
        private int x;
        private int y;
        private bool hasGlass;

        public GemGridPosition(Grid<GemGridPosition> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetGemGrid(GemGrid gemGrid) {
            this.gemGrid = gemGrid;
            grid.TriggerGridObjectChanged(x, y);
        }

        public int GetX() {
            return x;
        }

        public int GetY() {
            return y;
        }

        public Vector3 GetWorldPosition() {
            return grid.GetWorldPosition(x, y);
        }

        public GemGrid GetGemGrid() {
            return gemGrid;
        }

        public void ClearGemGrid() {
            gemGrid = null;
        }

        public void DestroyGem() {
            gemGrid?.Destroy();
            grid.TriggerGridObjectChanged(x, y);
        }

        public bool HasGemGrid() {
            return gemGrid != null;
        }

        public bool IsEmpty() {
            return gemGrid == null;
        }

        public bool HasGlass() {
            return hasGlass;
        }

        public void SetHasGlass(bool hasGlass) {
            this.hasGlass = hasGlass;
        }

        public void DestroyGlass() {
            SetHasGlass(false);
            OnGlassDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString() {
            return gemGrid?.ToString();
        }
    }

    /*
     * Represents a Gem Object in the Grid
     * */
    public class GemGrid {

        public event EventHandler OnDestroyed;

        private GemSO gem;
        private int x;
        private int y;
        private bool isDestroyed;

        public GemGrid(GemSO gem, int x, int y) {
            this.gem = gem;
            this.x = x;
            this.y = y;

            isDestroyed = false;
        }

        public GemSO GetGem() {
            return gem;
        }

        public Vector3 GetWorldPosition() {
            return new Vector3(x, y);
        }

        public void SetGemXY(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void Destroy() {
            isDestroyed = true;
            OnDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString() {
            return isDestroyed.ToString();
        }

    }

}
