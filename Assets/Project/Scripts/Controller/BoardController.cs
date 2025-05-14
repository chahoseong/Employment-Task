using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance;
    private StageContext stageContext;
    
    public int Width => stageContext.BoardWidth;
    public int Height => stageContext.BoardHeight;

    private void Awake()
    {
        Instance = this;
    }
    
    public void Init(StageContext stageContext)
    {
        this.stageContext = stageContext;
    }
    
    public bool CheckCanDestroy(BoardBlockObject boardBlock, BlockPiece blockPiece)
    {
        if (!stageContext.IsCheckBoardBlock(boardBlock))
        {
            return false;
        }
        
        List<BoardBlockObject> horizonBoardBlocks = new List<BoardBlockObject>();
        List<BoardBlockObject> verticalBoardBlocks = new List<BoardBlockObject>();

        foreach (var checkIndex in boardBlock.checkGroupIdx)
        {
            foreach (var boardBlockObj in stageContext.GetCheckBoardBlocks(checkIndex))
            {
                foreach (var horizon in boardBlockObj.isHorizon)
                {
                    if (horizon) horizonBoardBlocks.Add(boardBlockObj);
                    else verticalBoardBlocks.Add(boardBlockObj);
                }
            }
        }

        int matchingIndex = boardBlock.colorType.FindIndex(color => color == blockPiece.Color);
        bool hor = boardBlock.isHorizon[matchingIndex];   
        //Horizon
        if (hor)
        {
            int minX = stageContext.BoardWidth;
            int maxX = -1;
            foreach (var coordinate in horizonBoardBlocks)
            {
                if (coordinate.x < minX) minX = (int)coordinate.x;

                if (coordinate.x > maxX) maxX = (int)coordinate.x;
            }

            // 개별 좌표가 나갔는지 여부를 판단.
            if (blockPiece.Group.Bounds.min.x < minX - stageContext.BlockDistance / 2 || blockPiece.Group.Bounds.max.x > maxX + stageContext.BlockDistance / 2)
            {
                return false;
            }

            (int, int)[] blockCheckCoords = new (int, int)[horizonBoardBlocks.Count];

            for (int i = 0; i < horizonBoardBlocks.Count; i++)
            {
                if (horizonBoardBlocks[i].y <= stageContext.BoardHeight / 2)
                {
                    int maxY = -1;

                    foreach (var currentBlock in blockPiece.Group.BlockPieces)
                    {
                        if (Mathf.Approximately(currentBlock.BlockPosition.y, horizonBoardBlocks[i].y))
                        {
                            if (currentBlock.BlockPosition.y > maxY)
                            {
                                maxY = (int)currentBlock.BlockPosition.y;
                            }
                        }
                    }

                    blockCheckCoords[i] = ((int)horizonBoardBlocks[i].x, maxY);

                    for (int l = blockCheckCoords[i].Item2; l <= horizonBoardBlocks[i].y; l++)
                    {
                        if (blockCheckCoords[i].Item1 < blockPiece.Group.Bounds.min.x || blockCheckCoords[i].Item1 > blockPiece.Group.Bounds.max.x)
                            continue;

                        Vector2Int boardPosition = new Vector2Int(blockCheckCoords[i].Item1, l);
                        BoardBlockObject otherBoardBlock = stageContext.GetBoardBlock(boardPosition);
                        
                        if (otherBoardBlock &&
                            otherBoardBlock.playingBlock != null &&
                            otherBoardBlock.playingBlock.Color != boardBlock.horizonColorType)
                        {
                            return false;
                        }
                    }
                }
                // up to downside
                else
                {
                    int minY = 100;

                    foreach (var currentBlock in blockPiece.Group.BlockPieces)
                    {
                        if (Mathf.Approximately(currentBlock.BlockPosition.y, horizonBoardBlocks[i].y))
                        {
                            if (currentBlock.BlockPosition.y < minY)
                            {
                                minY = (int)currentBlock.BlockPosition.y;
                            }
                        }
                    }

                    blockCheckCoords[i] = ((int)horizonBoardBlocks[i].x, minY);

                    for (int l = blockCheckCoords[i].Item2; l >= horizonBoardBlocks[i].y; l--)
                    {
                        if (blockCheckCoords[i].Item1 < blockPiece.Group.Bounds.min.x || blockCheckCoords[i].Item1 > blockPiece.Group.Bounds.max.x)
                            continue;
                        
                        Vector2Int boardPosition = new Vector2Int(blockCheckCoords[i].Item1, l);
                        BoardBlockObject otherBoardBlock = stageContext.GetBoardBlock(boardPosition);
                        
                        if (otherBoardBlock &&
                            otherBoardBlock.playingBlock != null &&
                            otherBoardBlock.playingBlock.Color != boardBlock.horizonColorType)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        // Vertical
        else
        {
            int minY = stageContext.BoardHeight;
            int maxY = -1;

            foreach (var coordinate in verticalBoardBlocks)
            {
                if (coordinate.y < minY) minY = (int)coordinate.y;
                if (coordinate.y > maxY) maxY = (int)coordinate.y;
            }

            if (blockPiece.Group.Bounds.min.y < minY - stageContext.BlockDistance / 2 || blockPiece.Group.Bounds.max.y > maxY + stageContext.BlockDistance / 2)
            {
                return false;
            }

            (int, int)[] blockCheckCoors = new (int, int)[verticalBoardBlocks.Count];

            for (int i = 0; i < verticalBoardBlocks.Count; i++)
            {
                //x exist in left
                if (verticalBoardBlocks[i].x <= stageContext.BoardWidth / 2)
                {
                    int maxX = int.MinValue;

                    foreach (var currentBlock in blockPiece.Group.BlockPieces)
                    {
                        if (Mathf.Approximately(currentBlock.BlockPosition.y, verticalBoardBlocks[i].y))
                        {
                            if (currentBlock.BlockPosition.x > maxX)
                            {
                                maxX = (int)currentBlock.BlockPosition.x;
                            }
                        }
                    }

                    // 튜플에 y와 maxX를 저장
                    blockCheckCoors[i] = (maxX, (int)verticalBoardBlocks[i].y);

                    for (int l = blockCheckCoors[i].Item1; l >= verticalBoardBlocks[i].x; l--)
                    {
                        if (blockCheckCoors[i].Item2 < blockPiece.Group.Bounds.min.y || blockCheckCoors[i].Item2 > blockPiece.Group.Bounds.max.y)
                            continue;
                        
                        Vector2Int boardPosition = new Vector2Int(l, blockCheckCoors[i].Item2);
                        BoardBlockObject otherBoardBlock = stageContext.GetBoardBlock(boardPosition);
                        
                        if (otherBoardBlock &&
                            otherBoardBlock.playingBlock != null &&
                            otherBoardBlock.playingBlock.Color != boardBlock.verticalColorType)
                        {
                            return false;
                        }
                    }
                }
                //x exist in right
                else
                {
                    int minX = 100;

                    foreach (var currentBlock in blockPiece.Group.BlockPieces)
                    {
                        if (Mathf.Approximately(currentBlock.BlockPosition.y, verticalBoardBlocks[i].y))
                        {
                            if (currentBlock.BlockPosition.x < minX)
                            {
                                minX = (int)currentBlock.BlockPosition.x;
                            }
                        }
                    }

                    // 튜플에 y와 minX를 저장
                    blockCheckCoors[i] = (minX, (int)verticalBoardBlocks[i].y);

                    for (int l = blockCheckCoors[i].Item1; l <= verticalBoardBlocks[i].x; l++)
                    {
                        if (blockCheckCoors[i].Item2 < blockPiece.Group.Bounds.min.y || blockCheckCoors[i].Item2 > blockPiece.Group.Bounds.max.y)
                            continue;
                        
                        Vector2Int boardPosition = new Vector2Int(l, blockCheckCoors[i].Item2);
                        BoardBlockObject otherBoardBlock = stageContext.GetBoardBlock(boardPosition);
                        
                        if (otherBoardBlock &&
                            otherBoardBlock.playingBlock != null &&
                            otherBoardBlock.playingBlock.Color != boardBlock.verticalColorType)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}