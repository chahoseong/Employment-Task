using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Data_Script;
using UnityEngine;

[CreateAssetMenu(menuName = "Actionfit/Builder/Wall", fileName = "New Wall Builder")]
public class WallBuilder : ScriptableObject
{
    [SerializeField] private WallObject[] wallPrefabs;
    [SerializeField] private MaterialService materialService;

    public async Task Build(IEnumerable<WallData> source, StageContext stageContext)
    {
        foreach (var wallData in source)
        {
            // 기본 위치 계산
            Vector3 position = new Vector3(
                wallData.x * stageContext.BlockDistance,
                0f,
                wallData.y * stageContext.BlockDistance);
            Quaternion rotation = Quaternion.identity;
            DestroyWallDirection destroyDirection = DestroyWallDirection.None;
            bool shouldAddWallAttributes = false;

            // 벽 방향과 유형에 따라 위치와 회전 조정
            switch (wallData.WallDirection)
            {
                case ObjectPropertiesEnum.WallDirection.Single_Up:
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    shouldAddWallAttributes = true;
                    destroyDirection = DestroyWallDirection.Up;
                    break;

                case ObjectPropertiesEnum.WallDirection.Single_Down:
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    shouldAddWallAttributes = true;
                    destroyDirection = DestroyWallDirection.Down;
                    break;

                case ObjectPropertiesEnum.WallDirection.Single_Left:
                    position.x -= 0.5f;
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    shouldAddWallAttributes = true;
                    destroyDirection = DestroyWallDirection.Left;
                    break;

                case ObjectPropertiesEnum.WallDirection.Single_Right:
                    position.x += 0.5f;
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    shouldAddWallAttributes = true;
                    destroyDirection = DestroyWallDirection.Right;
                    break;

                case ObjectPropertiesEnum.WallDirection.Left_Up:
                    // 왼쪽 위 모서리
                    position.x -= 0.5f;
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;

                case ObjectPropertiesEnum.WallDirection.Left_Down:
                    // 왼쪽 아래 모서리
                    position.x -= 0.5f;
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    break;

                case ObjectPropertiesEnum.WallDirection.Right_Up:
                    // 오른쪽 위 모서리
                    position.x += 0.5f;
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 270f, 0f);
                    break;

                case ObjectPropertiesEnum.WallDirection.Right_Down:
                    // 오른쪽 아래 모서리
                    position.x += 0.5f;
                    position.z -= 0.5f;
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;

                case ObjectPropertiesEnum.WallDirection.Open_Up:
                    // 위쪽이 열린 벽
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;

                case ObjectPropertiesEnum.WallDirection.Open_Down:
                    // 아래쪽이 열린 벽
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    break;

                case ObjectPropertiesEnum.WallDirection.Open_Left:
                    // 왼쪽이 열린 벽
                    position.x -= 0.5f;
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;

                case ObjectPropertiesEnum.WallDirection.Open_Right:
                    // 오른쪽이 열린 벽
                    position.x += 0.5f;
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;

                default:
                    Debug.LogError($"지원되지 않는 벽 방향: {wallData.WallDirection}");
                    continue;
            }

            // 길이에 따른 위치 조정 (수평/수직 벽만 조정)
            if (wallData.length > 1)
            {
                // 수평 벽의 중앙 위치 조정 (Up, Down 방향)
                if (wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Up ||
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Down ||
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Up ||
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Down)
                {
                    // x축으로 중앙으로 이동
                    position.x += (wallData.length - 1) * stageContext.BlockDistance * 0.5f;
                }
                // 수직 벽의 중앙 위치 조정 (Left, Right 방향)
                else if (wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Left ||
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Right ||
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Left ||
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Right)
                {
                    // z축으로 중앙으로 이동
                    position.z += (wallData.length - 1) * stageContext.BlockDistance * 0.5f;
                }
            }
            
            if (wallData.length - 1 >= 0 && wallData.length - 1 < wallPrefabs.Length)
            {
                WallObject wall = Instantiate(wallPrefabs[wallData.length - 1], stageContext.WallsRoot);
                wall.transform.position = position;
                wall.transform.rotation = rotation;
                
                wall.BoardPosition = new Vector2Int(wallData.x, wallData.y);
                wall.Direction = destroyDirection;
                wall.Color = wallData.wallColor;
                wall.Length = wallData.length;
                wall.SetWall(materialService.GetWallMaterial(wallData.wallColor), wallData.wallColor != ColorType.None);

                if (shouldAddWallAttributes && wallData.wallColor != ColorType.None)
                {
                    stageContext.AddWall(wall, wallData.x, wallData.y);
                }
            }
            else
            {
                Debug.LogError($"프리팹 인덱스 범위를 벗어남: {wallData.length - 1}, 사용 가능한 프리팹: 0-{wallPrefabs.Length - 1}");
            }
        }
        
        await Task.Yield();
    }
}
