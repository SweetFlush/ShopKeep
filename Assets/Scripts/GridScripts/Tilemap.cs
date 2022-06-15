using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap
{
    private GridSystem<TilemapObject> grid;

    public Tilemap(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new GridSystem<TilemapObject>(width, height, cellSize, originPosition, (GridSystem<TilemapObject> g, int x, int y)
            => new TilemapObject(grid, x, y));
    }

    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite)
    {
        TilemapObject tilemapObject = grid.GetGridObject(worldPosition);

        if(tilemapObject != null)
        {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public class TilemapObject
    {
        public enum TilemapSprite
        {
            None,
            Ground
        }

        private GridSystem<TilemapObject> grid;
        private int x;
        private int y;
        private TilemapSprite tilemapSprite;

        public TilemapObject(GridSystem<TilemapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTilemapSprite(TilemapSprite tilemapSprite)
        {
            this.tilemapSprite = tilemapSprite;
            grid.TriggerGridObjectChanged(x, y);
        }

        public string ToString()
        {
            return tilemapSprite.ToString();
        }
    }


}
