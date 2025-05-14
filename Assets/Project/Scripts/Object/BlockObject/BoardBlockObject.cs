
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class BoardBlockObject : MonoBehaviour
{
    public BoardController controller;
    public BlockPiece playingBlock;
    public bool isCheckBlock;
    public List<int> checkGroupIdx;
    public List<ColorType> colorType;
    public List<bool> isHorizon;
    public List<int> len;
    public int x;
    public int y;

    public ColorType horizonColorType => 
        isHorizon.IndexOf(true) != -1 ? colorType[isHorizon.IndexOf(true)] : ColorType.None;
    
    public ColorType verticalColorType => 
        isHorizon.IndexOf(false) != -1 ? colorType[isHorizon.IndexOf(true)] : ColorType.None;
    
    public bool CheckAdjacentBlock(BlockPiece blockPiece, Vector3 destroyStartPos)
    {
        if (!isCheckBlock) return false;
        if (!blockPiece.Group.enabled) return false;
        for (int i = 0; i < colorType.Count; i++)
        {
            if (blockPiece.Color == colorType[i])
            {
                int length = 0;
                if (isHorizon[i])
                {
                    if (blockPiece.Group.Bounds.Width > len[i]) return false;
                    if (!controller.CheckCanDestroy(this, blockPiece)) return false;
                    length = blockPiece.Group.Bounds.Height;
                }
                else
                {
                    if (blockPiece.Group.Bounds.Height > len[i]) return false;
                    if (!controller.CheckCanDestroy(this, blockPiece)) return false;
                    length = blockPiece.Group.Bounds.Width;
                }

                /*block.dragHandler.transform.position =
                    new Vector3(x - block.offsetToCenter.x,
                                block.dragHandler.transform.position.y,
                                y - block.offsetToCenter.y) * 0.79f;*/

                blockPiece.Group.transform.position = destroyStartPos;
                blockPiece.Group.GetComponent<BlockDragHandler>().ReleaseInput();

                blockPiece.Group.SetCollisionEnabled(false);
                blockPiece.Group.enabled = false;

                bool isRight = isHorizon[i] ? y < controller.Height / 2 : x < controller.Width / 2;
                if (!isRight) length *= -1;
                Vector3 pos = isHorizon[i]
                    ? new Vector3(blockPiece.Group.transform.position.x, blockPiece.Group.transform.position.y,
                        blockPiece.Group.transform.position.z - length * 0.79f)
                    : new Vector3(blockPiece.Group.transform.position.x - length * 0.79f,
                        blockPiece.Group.transform.position.y, blockPiece.Group.transform.position.z);


                Vector3 centerPos =
                    isHorizon[i]
                        ? blockPiece.Group.GetCenterX()
                        : blockPiece.Group.GetCenterZ(); //_ctrl.CenterOfBoardBlockGroup(len, isHorizon, this);
                LaunchDirection direction = GetLaunchDirection(x, y, isHorizon[i]);
                Quaternion rotation = Quaternion.identity;

                centerPos.y = 0.55f;
                switch (direction)
                {
                    case LaunchDirection.Up:
                        centerPos += Vector3.forward * 0.65f;
                        centerPos.z = transform.position.z;
                        centerPos.z += 0.55f;
                        rotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case LaunchDirection.Down:
                        centerPos += Vector3.back * 0.65f;
                        break;
                    case LaunchDirection.Left:
                        centerPos += Vector3.left * 0.55f;
                        //offset.z = centerPos.transform.position.z;
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case LaunchDirection.Right:
                        centerPos += Vector3.right * 0.55f;
                        centerPos.x = transform.position.x;
                        centerPos.x += 0.65f;
                        rotation = Quaternion.Euler(0, -90, 0);
                        //offset.z = centerPos.transform.position.z;
                        break;
                }

                int blockLength = isHorizon[i] ? blockPiece.Group.Bounds.Width : blockPiece.Group.Bounds.Height;
                ParticleEffectService particleEffectService = ServiceLocator.Get<ParticleEffectService>();
                GameObject particleInstance = particleEffectService.CreateBlockDestroyEffect(blockPiece.Color);
                particleInstance.transform.position = centerPos;
                particleInstance.transform.rotation = rotation;
                particleInstance.transform.localScale = new Vector3(blockLength * 0.4f, 0.5f, blockLength * 0.4f);
                
                blockPiece.Group.DestroyMove(pos, particleInstance);
            }
        }

        return true;
    }
    
    LaunchDirection GetLaunchDirection(int x, int y, bool isHorizon)
    {
        // 모서리 케이스들
        if (x == 0 && y == 0)
            return isHorizon ? LaunchDirection.Down : LaunchDirection.Left;
    
        if (x == 0 && y == controller.Height)
            return isHorizon ? LaunchDirection.Up : LaunchDirection.Left;
    
        if (x == controller.Width && y == 0)
            return isHorizon ? LaunchDirection.Down : LaunchDirection.Right;
    
        if (x == controller.Width && y == controller.Height)
            return isHorizon ? LaunchDirection.Up : LaunchDirection.Right;
    
        // 기본 경계 케이스들
        if (x == 0)
            return isHorizon ? LaunchDirection.Down : LaunchDirection.Left;
    
        if (y == 0)
            return isHorizon ? LaunchDirection.Down : LaunchDirection.Left;
    
        if (x == controller.Width)
            return isHorizon ? LaunchDirection.Down : LaunchDirection.Right;
    
        if (y == controller.Height)
            return isHorizon ? LaunchDirection.Up : LaunchDirection.Right;
    
        // 기본값 (필요하다면)
        return LaunchDirection.Up;
    }
}

public enum ColorType
{
    None,
    Red,
    Orange,
    Yellow,
    Gray,
    Purple,
    Beige,
    Blue,
    Green
}
