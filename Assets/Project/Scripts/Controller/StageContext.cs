using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageContext
{
    private Dictionary<Vector2Int, List<WallObject>> walls = new();
    
    private Dictionary<Vector2Int, BoardBlockObject> boardBlocks = new();
    private Dictionary<int, (BoardBlockObject, bool)> standardBlocks = new();
    private Dictionary<int, List<BoardBlockObject>> checkBlocks = new();
    
    private List<GameObject> quads = new List<GameObject>();
    
    public Transform WallsRoot { get; set; }
    public Transform BoardBlocksRoot { get; set; }
    public Transform BlocksRoot { get; set; }
    public Transform QuadsRoot { get; set; }
    
    public IEnumerable<KeyValuePair<Vector2Int, BoardBlockObject>> BoardBlocks => boardBlocks;
    public IEnumerable<KeyValuePair<int, (BoardBlockObject, bool)>> StandardBlocks => standardBlocks;
    
    public float BlockDistance { get; set; }
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    
    public void AddWall(WallObject wall, int x, int y)
    {
        AddWall(wall, new Vector2Int(x, y));
    }
    
    public void AddWall(WallObject wall, Vector2Int boardPosition)
    {
        if (!walls.ContainsKey(boardPosition))
        {
            walls.Add(boardPosition, new List<WallObject>());
        }
        walls[boardPosition].Add(wall);
    }

    public IEnumerable<WallObject> GetWalls(int x, int y)
    {
        return GetWalls(new Vector2Int(x, y));
    }

    public IEnumerable<WallObject> GetWalls(Vector2Int boardPosition)
    {
        return walls.GetValueOrDefault(boardPosition, new List<WallObject>());
    }

    public void SetBoardBlock(BoardBlockObject boardBlock, int x, int y)
    {
        SetBoardBlock(boardBlock, new Vector2Int(x, y));
    }

    public void SetBoardBlock(BoardBlockObject boardBlock, Vector2Int boardPosition)
    {
        boardBlocks.Add(boardPosition, boardBlock);
    }

    public BoardBlockObject GetBoardBlock(int x, int y)
    {
        return GetBoardBlock(new Vector2Int(x, y));
    }
    
    public BoardBlockObject GetBoardBlock(Vector2Int boardPosition)
    {
        return boardBlocks.GetValueOrDefault(boardPosition);
    }

    public void SetStandardBoardBlock(int uniqueIndex, BoardBlockObject boardBlock, bool isHorizontal)
    {
        standardBlocks[uniqueIndex] = (boardBlock, isHorizontal);
    }

    public BoardBlockObject GetStandardBoardBlock(int uniqueIndex)
    {
        return standardBlocks.GetValueOrDefault(uniqueIndex).Item1;
    }

    public void AddCheckBoardBlock(int uniqueIndex, BoardBlockObject boardBlock)
    {
        if (!checkBlocks.ContainsKey(uniqueIndex))
        {
            checkBlocks.Add(uniqueIndex, new List<BoardBlockObject>());
        }
        checkBlocks[uniqueIndex].Add(boardBlock);
    }

    public IEnumerable<BoardBlockObject> GetCheckBoardBlocks(int uniqueIndex)
    {
        return checkBlocks.GetValueOrDefault(uniqueIndex, new List<BoardBlockObject>());
    }

    public bool IsCheckBoardBlock(BoardBlockObject boardBlock)
    {
        return boardBlock.isCheckBlock && boardBlock.checkGroupIdx.All(x => checkBlocks.ContainsKey(x));
    }
}
