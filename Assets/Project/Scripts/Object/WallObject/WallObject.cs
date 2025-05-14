
using UnityEngine;

public class WallObject : MonoBehaviour
{
    [SerializeField] MeshRenderer wallRenderer;
    [SerializeField] private GameObject arrow;
    
    public Vector2Int BoardPosition { get; set; }
    public DestroyWallDirection Direction { get; set; }

    public ColorType Color { get; set; }
    public int Length { get; set; }
    
    public void SetWall(Material material, bool isCuttingBox)
    {
        wallRenderer.material = material;
        arrow.SetActive(isCuttingBox);
    }
}