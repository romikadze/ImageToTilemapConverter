using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDrawer : MonoBehaviour
{
    [SerializeField] private Converter _converter;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Sprite _tileSprite;
    [SerializeField] private string _palettePathFromResourcesFolder = "";
    [SerializeField] private string _tilemapPathFromResourcesFolder = "";
    
    private const string ASSET_EXTENSION = ".asset";
    private const string PREFAB_EXTENSION = ".prefab";
    private const string BASIC_PATH = "Assets/Resources/";

    private string _tilemapDirectoryPath;
    private string _paletteDirectoryPath;
    private string _paletteDirectoryPathFromResources;
    private void GeneratePaths()
    {
        _tilemapDirectoryPath = BASIC_PATH + _tilemapPathFromResourcesFolder + _converter.ImageName;
        _paletteDirectoryPathFromResources = _palettePathFromResourcesFolder + _converter.ImageName + " Palette";
        _paletteDirectoryPath = BASIC_PATH + _paletteDirectoryPathFromResources;
    }
    
    public void GenerateTilemap()
    {
        _tilemap.ClearAllTiles();
        Tile[] tileList = Resources.LoadAll<Tile>(_paletteDirectoryPathFromResources + "/");
        Dictionary<Color, Tile> tiles = new Dictionary<Color, Tile>();

        if (tileList.Length == 0)
        {
            throw new Exception("No Tiles Found in");
        }
        
        foreach (var tile in tileList)
        {
            tiles.Add(tile.color, tile);
        }
            
        foreach (var data in _converter.TilemapData)
        {
            if(tiles.TryGetValue(data.Value, out var tile))
               _tilemap.SetTile(data.Key, tile);
        }
        
        GeneratePaths();
        
        if(!Directory.Exists(_tilemapDirectoryPath))
            UnityEngine.Windows.Directory.CreateDirectory(_tilemapDirectoryPath);
        
        PrefabUtility.SaveAsPrefabAsset(_tilemap.gameObject,  
            _tilemapDirectoryPath + "/" + _tilemap.gameObject.name + PREFAB_EXTENSION);
    }

    public void GeneratePalette()
    {
        
        for (int i = 0; i < _converter.PaletteData.Count; i++)
        {
            Tile tile = new Tile();
            tile.sprite = _tileSprite;
            tile.color = _converter.PaletteData[i];

            GeneratePaths();
            
            if(!Directory.Exists(_paletteDirectoryPath))
                UnityEngine.Windows.Directory.CreateDirectory(_paletteDirectoryPath);
            AssetDatabase.CreateAsset(tile,  _paletteDirectoryPath + "/" + (i+1) + ASSET_EXTENSION);
        }
    }
}