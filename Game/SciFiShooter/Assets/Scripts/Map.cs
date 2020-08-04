using QuickType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.U2D;

public class Map : MonoBehaviour
{
    public static Map Instance=null;
    private const int mapCellMaxWidth = 11;
    private const int mapCellMaxHeight = 22;

    private const float offsetWorldMinX = -5;
    private const float offsetWorldMaxX = 5f;
    private const float offsetWorldMaxY = 41f;
    private const float offsetWorldMinY = -1f;
    private const float offsetWorldXStep = 1f;
    private const float offsetWorldYStep = 2f;

    private int currentMapCellWidth = mapCellMaxWidth, currentMapCellHeight = mapCellMaxHeight;


    public int CurrentMapCellWidth { get => currentMapCellWidth; }
    public int CurrentMapCellHeight { get => currentMapCellHeight; }


    MapDefine currentMapDefine;
    int mapTileFirstGID;
    int enemyTileFirstGID;
    int currentMapIndex = -1;
    int currentMapWave = 0;

    private MapTile maptile = null;
    private MapTile enemyTile = null;

    //Using Addressable Asset Process replace Resource Folder Process
    [SerializeField] AssetReference mapTileText;
    [SerializeField] AssetReference enemyTileText;

    List<Vector2Int> emptyCell = new List<Vector2Int>();

    Dictionary<string, Sprite> spriteFrameCache = new Dictionary<string, Sprite>();
    Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();
    Dictionary<int, List<EnemyController>> currentEnemies = new Dictionary<int, List<EnemyController>>();

    [SerializeField] PlayerController playerController;
    [SerializeField] NavMeshSurface navMeshSurface;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartAsync();
    }
    private async void StartAsync()
    {
        await InitTile();
        LoadMap(1, 1, null);
    }

    private async Task InitTile()
    {
        var mapTileAsset = await mapTileText.LoadAssetAsync<TextAsset>().Task;
        maptile = MapTile.FromJson(mapTileAsset.text);
        FixImageNameOnMapTile(maptile);

        var enemyTileAsseet = await enemyTileText.LoadAssetAsync<TextAsset>().Task;
        enemyTile = MapTile.FromJson(enemyTileAsseet.text);
        FixImageNameOnMapTile(enemyTile);

    }


    //Remove relative path on tile image property -> just get name only
    void FixImageNameOnMapTile(MapTile maptile)
    {
        int tileLeng = maptile.Tiles.Length;

        for (int i = 0; i < tileLeng; i++)
        {
            string imgName = Path.GetFileNameWithoutExtension(maptile.Tiles[i].Image);
            maptile.Tiles[i].Image = imgName;
        }
    }

    public async void LoadMap(int mapIndex, int level, Action<Vector3> loadDoneCB)
    {
       
        string mapCode = "map_" + mapIndex + "_" + level;
        var mapAsset = await Addressables.LoadAssetAsync<TextAsset>(mapCode).Task; //Resources.Load<TextAsset>("Map/" + mapCode);

        //Load Map Json
        currentMapDefine = MapDefine.FromJson(mapAsset.text);
        Addressables.Release(mapAsset);
        currentMapCellHeight = currentMapDefine.Height;
        currentMapCellWidth = currentMapDefine.Width;

        Tileset mapTileSet = Array.Find(currentMapDefine.Tilesets, tileSet => tileSet.Source.Contains("MapTile"));
        Debug.Assert(mapTileSet != null, "There isn't a map tile set!!!!");
        mapTileFirstGID = mapTileSet.Firstgid;

        Tileset enemyTileSet = Array.Find(currentMapDefine.Tilesets, tileSet => tileSet.Source.Contains("Enemy"));
        Debug.Assert(enemyTileSet != null, "There isn't a enemy tile set!!!!");
        enemyTileFirstGID = enemyTileSet.Firstgid;

        //Load tile sprite
        if (currentMapIndex != mapIndex)
        {
            string mapTileCode = "Map" + mapIndex + "Tile";
            spriteFrameCache.Clear();

            SpriteAtlas spriteAtlas = await Addressables.LoadAssetAsync<SpriteAtlas>(mapTileCode).Task;

            int spriteLeng = spriteAtlas.spriteCount;
            Sprite[] sprites = new Sprite[spriteLeng];
            spriteAtlas.GetSprites(sprites);

            for (int i = 0; i < spriteLeng; i++)
            {
                int index = sprites[i].name.IndexOf("(Clone)");
                string name = sprites[i].name.Remove(index);
                spriteFrameCache[name] = sprites[i];
            }
            Addressables.Release(spriteAtlas);
        
        }

        
        currentMapIndex = mapIndex;

        //---------------------------------------

        currentMapWave = 1;
        Vector3 startPoint = Vector3.zero;
        LoadTile(currentMapDefine.Layers[0].Data, out startPoint);
        await LoadEnemy();
        SpawnEnemy(currentMapWave);
       
       
    }

    /// <summary>
    /// Load Map Tile from Json
    /// </summary>
    /// <param name="tileId"></param>
    /// <param name="startPoint"></param>
    private void LoadTile(int[] tileId, out Vector3 startPoint)
    {
        startPoint = Vector3.zero;
    

        int height = currentMapDefine.Height;
        int width = currentMapDefine.Width;
        int dataLeng = tileId.Length;
        int row = 0, col = 0;
        GameObject tileObject = Resources.Load<GameObject>("Objects/TileObj");
        emptyCell.Clear();
        for (int i = 0; i < dataLeng; i++)
        {
            Vector3 worldPos = ConvertCellPosToWorld(row, col);
            bool hasCreateObj = false;

            if (tileId[i] > 0)
            {
                //if (tileId[i] == mapTileFirstGID)
                //{
                //    startPoint = worldPos;
                //    continue;
                //}
                Tile tile = maptile.GetTileWithId(tileId[i] - mapTileFirstGID);
                GameObject tiled = null;
                tiled = Instantiate(tileObject);
                tiled.transform.position = worldPos;
                if (!hasCreateObj)
                    tiled.GetComponentInChildren<SpriteRenderer>().sprite = spriteFrameCache[tile.Image];
                //cellObject.Add(tiled);
            }
            else
            {
                emptyCell.Add(new Vector2Int(row, col));
            }


            col++;
            if (col >= width)
            {
                col = 0;
                //lastBeginLTRTrap = Vector3.zero;
                //lastEndRTLTrap = Vector3.zero;
                row++;
            }
        }
        navMeshSurface.BuildNavMesh();

    }

    /// <summary>
    /// Preload Enemy into Dictionary. Call SpawnEnemy(wave) to set gameOBject Visible on Map
    /// </summary>
    private async Task LoadEnemy()
    {
        int waveLeng = currentMapDefine.Layers.Length;
        currentEnemies.Clear();
        bool hasBoss = false;
        for (int v = 1; v < waveLeng; v++)
        {
            currentEnemies.Add(v, new List<EnemyController>());
            int height = currentMapDefine.Height;
            int width = currentMapDefine.Width;
            int dataLeng = currentMapDefine.Layers[v].Data.Length;
            int row = 0, col = 0;

            bool isBossLayer = currentMapDefine.Layers[v].Name == "Boss";
            if (isBossLayer) hasBoss = true;


            for (int i = 0; i < dataLeng; i++)
            {
                Vector3 worldPos = ConvertCellPosToWorld(row, col);

                int tileId = currentMapDefine.Layers[v].Data[i] - enemyTileFirstGID;
                if (tileId >= 0)
                {
                    string monsterId = enemyTile.Tiles[tileId].Image;
                    monsterId = monsterId.Replace(".png", string.Empty);

                    EnemyController enemyController = null;
                    GameObject enemyPrefab = null;
                    if (prefabCache.ContainsKey(monsterId))
                    {
                        enemyPrefab = prefabCache[monsterId];
                    }
                    else
                    {
                        enemyPrefab = await Addressables.LoadAssetAsync<GameObject>(monsterId).Task;
                        prefabCache.Add(monsterId, enemyPrefab);
                    }

                    Debug.Assert(enemyPrefab != null, "Enemy Prefab null " + monsterId);

                    enemyController = Instantiate(enemyPrefab).GetComponent<EnemyController>();
                    enemyController.transform.position = worldPos;
                    enemyController.gameObject.SetActive(false);
                    enemyController.onDead += OnEnemyDead;
                    currentEnemies[v].Add(enemyController);

                }
                col++;
                if (col >= width)
                {
                    col = 0;
                    row++;
                }


                if (isBossLayer)
                {
                    //Implement Boss Data
                }
                else
                {
                   
                }
            }
        }
    }
       
    private void SpawnEnemy(int wave)
    {
        List<EnemyController> enemyControllers = this.currentEnemies[wave];
        enemyControllers.ForEach((enemy) =>
        {
            enemy.InitData();
            enemy.SetTarget(playerController);
            enemy.gameObject.SetActive(true);
        });
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

    public Vector3 ConvertCellPosToWorld(int row,int col)
    {
       
        float zPosOffset = (mapCellMaxHeight - currentMapCellHeight) * offsetWorldYStep;
        float xWorldPos = offsetWorldMinX + col * offsetWorldXStep;
        float zWorldPos = offsetWorldMaxY - row * offsetWorldYStep - zPosOffset;
        return new Vector3(xWorldPos, 0f, zWorldPos);
    }

    public void ClaimCellPos(ref Vector2 cellPos)
    {
        if (cellPos.x < 0) cellPos.x = 0;
        else if (cellPos.x >= CurrentMapCellHeight) cellPos.x = CurrentMapCellHeight - 1;

        if (cellPos.y < 0) cellPos.y = 0;
        else if (cellPos.y >= currentMapCellWidth) cellPos.y = currentMapCellWidth - 1;
    }


    public Vector3 GetARandomEmptyCellWorldPos()
    {
        Vector2Int cellPos = emptyCell[UnityEngine.Random.Range(0, emptyCell.Count)];
        Vector3 worldPos = ConvertCellPosToWorld(cellPos.x, cellPos.y);
        return worldPos;
    }

    public EnemyController GetNearestEnemy(Vector3 pos)
    {
        if (!currentEnemies.ContainsKey(currentMapWave) || currentEnemies[currentMapWave].Count == 0)
            return null;

        EnemyController nearest = null;
        List<EnemyController> currentEnemy = currentEnemies[currentMapWave];

        float minDistance = float.MaxValue;

        currentEnemy.ForEach((enemy) =>
        {
            if (enemy.Alive)
            {
                float distance = UtilityHandler.Distance(pos, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        });
        return nearest;
    }

    void OnEnemyDead(EnemyController enemyController)
    {
        currentEnemies[currentMapWave].Remove(enemyController);
        if (currentEnemies[currentMapWave].Count == 0)
        {
            if (currentEnemies.ContainsKey(currentMapWave + 1))
                SpawnEnemy(++currentMapWave);
        }
    }
}
