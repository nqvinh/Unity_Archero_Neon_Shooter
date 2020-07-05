using QuickType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class MapLoader : MonoBehaviour
{
    
    private const int mapCellMaxWidth = 11;
    private const int mapCellMaxHeight = 22;

    //The Min X Position of a tile on map
    private const float offsetWorldMinX = -5;

    //The Max X Position of a tile on map
    private const float offsetWorldMaxX = 5f;

    //The Max Y Position of a tile on map
    private const float offsetWorldMaxY = 41f; // = MinOffsetY + (mapCellMaxHeight-1)*offsetWorldYStep

    //The Min Y Position of a tile on map
    private const float offsetWorldMinY = -1f;

    //STEP DISTANCE BETWEEN TWO TILE On X-Axis
    private const float offsetWorldXStep = 1f;

    //STEP DISTANCE BETWEEN TWO TILE On Y-Axis
    private const float offsetWorldYStep = 2f;

    private int currentMapCellWidth = mapCellMaxWidth, currentMapCellHeight = mapCellMaxHeight;


    public int CurrentMapCellWidth { get => currentMapCellWidth; }
    public int CurrentMapCellHeight { get => currentMapCellHeight; }


    MapDefine currentMapDefine;
    int mapTileFirstGID;
    int currentMapIndex = -1;
    MapTile maptile = null;


    Dictionary<string, Sprite> spriteFrameCache = new Dictionary<string, Sprite>();
    Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();


    private void Start()
    {
        if (maptile == null)
        {
            var mapTileAsset = Resources.Load<TextAsset>("Map/MapTile");
            maptile = MapTile.FromJson(mapTileAsset.text);
            FixImageNameOnMapTile(maptile);
        }

        StartCoroutine(LoadMap(1, 1, null));
    }


    //Remove relative path on tile image property -> just get name only
    //Using name to get sprite from sprite-sheet
    void FixImageNameOnMapTile(MapTile maptile)
    {
        int tileLeng = maptile.Tiles.Length;

        for (int i = 0; i < tileLeng; i++)
        {
            string imgName = Path.GetFileNameWithoutExtension(maptile.Tiles[i].Image);
            maptile.Tiles[i].Image = imgName;
        }
    }

    public IEnumerator LoadMap(int mapIndex, int level, Action<Vector3> loadDoneCB)
    {
        //Load map from resource, Map Stage X Level Y
        string mapCode = "map_" + mapIndex + "_" + level;
        var mapAsset = Resources.Load<TextAsset>("Map/" + mapCode);

        //Load JSON to Mem
        currentMapDefine = MapDefine.FromJson(mapAsset.text);
        currentMapCellHeight = currentMapDefine.Height;
        currentMapCellWidth = currentMapDefine.Width;

        //Find tileset along map stage, Each stage has different map tile set. Ex: LavaMap, SnowMap...
        Tileset mapTileSet = Array.Find(currentMapDefine.Tilesets, tileSet => tileSet.Source.Contains("MapTile"));
        Debug.Assert(mapTileSet != null, "There isn't a map tile set!!!!");
        mapTileFirstGID = mapTileSet.Firstgid;

        //Load tile sprite
        //Current Map Index: Each Layer in tilemap, Each Layer consider as Wave. One Map Has multiple wave, ex: Wave1, Wave2, WaveBoss
        if (currentMapIndex != mapIndex)
        {
            string mapTileCode = "Map" + mapIndex + "Tile";
            spriteFrameCache.Clear();

            //Cache Sprite From Resource, Pack sprite to a sprite-sheet call MapXTile, where X is Stage

            //Sprite[] sprites = Resources.LoadAll<Sprite>("Map/" + mapTileCode); => Load from TexturePacker Sprite Sheet

            SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("Map/" + mapTileCode); //=> Load from Sprite Atlas
            int spriteLeng = spriteAtlas.spriteCount;
            
            
            
            Sprite[] sprites = new Sprite[spriteLeng];
            spriteAtlas.GetSprites(sprites);

            for (int i = 0; i < spriteLeng; i++)
            {
                int index = sprites[i].name.IndexOf("(Clone)");
                string name = sprites[i].name.Remove(index);
                spriteFrameCache[name] = sprites[i];
            }

            //Unload old prefab cache
            /*foreach (KeyValuePair<string, GameObject> prefab in prefabCache)
            {
                Resources.UnloadAsset(prefab.Value);
                // do something with entry.Value or entry.Key
            }*/
            prefabCache.Clear();
        }
        currentMapIndex = mapIndex;

        //---------------------------------------


        Vector3 startPoint = Vector3.zero;
        LoadTile(currentMapDefine.Layers[0].Data, out startPoint);

        Resources.UnloadAsset(mapAsset);
        yield return new WaitForEndOfFrame();
    }

    private void LoadTile(int[] tileId, out Vector3 startPoint)
    {
        startPoint = Vector3.zero;


        int height = currentMapDefine.Height;
        int width = currentMapDefine.Width;
        int dataLeng = tileId.Length;
        int row = 0, col = 0;
        GameObject tileObject = Resources.Load<GameObject>("Objects/TileObj");

        for (int i = 0; i < dataLeng; i++)
        {
            //Convert CELL POS TO WORLD POS, EX: CEll(0,0) => Vector3(12,1,12)
            Vector3 worldPos = ConvertCellPosToWorld(row,col);
            bool hasCreateObj = false;

            if (tileId[i] > 0)
            {
                //if (tileId[i] == mapTileFirstGID)
                //{
                //    startPoint = worldPos;
                //    continue;
                //}
                //FIND TILE BY ID
                Tile tile = maptile.GetTileWithId(tileId[i] - mapTileFirstGID);
                GameObject tiled = null;
                tiled = Instantiate(tileObject);
                tiled.transform.position = worldPos;
                if (!hasCreateObj)
                    tiled.GetComponentInChildren<SpriteRenderer>().sprite = spriteFrameCache[tile.Image];
                //cellObject.Add(tiled);
            }


            col++;
            if (col >= width)
            {
                col = 0;
                row++;
            }
        }
    }



    public Vector2 ConvertWorldToCellPos(Vector3 pos, bool isClaimToMap = true)
    {
        float zPosOffset = (mapCellMaxHeight - currentMapCellHeight) * offsetWorldYStep;
        float worldX = pos.x;
        float worldZ = pos.z;
        int cellCol = Mathf.CeilToInt((worldX - offsetWorldMinX) / offsetWorldXStep);
        int cellRow = Mathf.CeilToInt((offsetWorldMaxY - worldZ - zPosOffset) / offsetWorldYStep);
        Vector2 res = new Vector2(cellRow, cellCol);
        if (isClaimToMap)
            ClaimCellPos(ref res);
        return res;
    }



    public Vector3 ConvertCellPosToWorld(Vector2 cellPos)
    {
        int cellRow = (int)cellPos.x;
        int cellCol = (int)cellPos.y;
        return ConvertCellPosToWorld(cellRow, cellCol);
    }

    public Vector3 ConvertCellPosToWorld(int row, int col)
    {
        float zPosOffset = (mapCellMaxHeight - currentMapCellHeight) * offsetWorldYStep;
        float xWorldPos = offsetWorldMinX + col * offsetWorldXStep;
        float zWorldPos = offsetWorldMaxY - row * offsetWorldYStep - zPosOffset;
        return new Vector3(xWorldPos, 0.5f, zWorldPos);
    }

    public void ClaimCellPos(ref Vector2 cellPos)
    {
        if (cellPos.x < 0) cellPos.x = 0;
        else if (cellPos.x >= CurrentMapCellHeight) cellPos.x = CurrentMapCellHeight - 1;

        if (cellPos.y < 0) cellPos.y = 0;
        else if (cellPos.y >= currentMapCellWidth) cellPos.y = currentMapCellWidth - 1;
    }

}
