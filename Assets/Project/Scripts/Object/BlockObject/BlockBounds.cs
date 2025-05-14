using UnityEngine;

public struct BlockBounds
{
    public Vector2 min;
    public Vector2 max;
    
    public int Width => (int)max.x - (int)min.x + 1;
    public int Height => (int)max.y - (int)min.y + 1;
}
