using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapScreenshotManager : MonoBehaviour
{
    [SerializeField] private List<Tilemap> _tilemaps;
    [SerializeField] private Camera _tilemapCamera;
    [SerializeField] private Image _image;
    [SerializeField] private TileBase _newTile;
    private string _saveFolder = "Assets/Resources/New/Icons";

    private List<string> _names = new();

    private void Start()
    {
        _names = GetAllDirectoryNamesInResources().ToList();
        
        // Ensure the directory exists
        if (!Directory.Exists(_saveFolder))
        {
            Directory.CreateDirectory(_saveFolder);
        }

        foreach (var tilemap in _tilemaps)
        {
            tilemap.CompressBounds();
            tilemap.gameObject.SetActive(false);
        }

        foreach (var tilemap in _tilemaps)
        {
            tilemap.color = Color.white;
            tilemap.gameObject.SetActive(true);
            AdjustTilemapToFitCamera(tilemap);
            CenterTilemapInCamera(tilemap);

            if (_newTile != null)
            {
                foreach (var position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(position))
                    {
                        var color = tilemap.GetColor(position);
                        tilemap.SetTile(position, _newTile);
                        tilemap.SetTileFlags(position, TileFlags.None);
                        tilemap.SetColor(position, color);
                    }
                }
            }
            
            CaptureAndSaveScreenshot(_names[0], 512, 512);
            _names.RemoveAt(0);
            tilemap.gameObject.SetActive(false);
        }
            /*Texture2D screenshotTexture = LoadScreenshotAsTexture("screenshot");
        if (screenshotTexture != null)
        {
            _image.sprite = Sprite.Create(screenshotTexture, new Rect(0, 0, screenshotTexture.width, screenshotTexture.height), new Vector2(0.5f, 0.5f));
        }*/
    }
    
    public string[] GetAllDirectoryNamesInResources()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", "Tilemap");
        if (Directory.Exists(resourcesPath))
        {
            return Directory.GetDirectories(resourcesPath);
        }
        else
        {
            Debug.LogError("Resources folder does not exist.");
            return new string[0];
        }
    }
    
    private void CenterTilemapInCamera(Tilemap tilemap)
    {
        // Get the Renderer component of the tilemap
        Renderer renderer = tilemap.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Get the center of the tilemap's bounds in world space
            Vector3 tilemapCenter = renderer.bounds.center;

            // Set the camera position to center the tilemap
            _tilemapCamera.transform.position = new Vector3(tilemapCenter.x, tilemapCenter.y, _tilemapCamera.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Tilemap does not have a Renderer component.");
        }
    }
    
    private void AdjustTilemapToFitCamera(Tilemap tilemap)
    {
        // Get the bounds of the tilemap in world space
        Bounds tilemapBounds = tilemap.localBounds;

        // Get the size of the tilemap in world space
        Vector3 tilemapSize = tilemapBounds.size;

        // Get the size of the camera view in world space
        float cameraHeight = 2f * _tilemapCamera.orthographicSize * 0.98f;
        float cameraWidth = cameraHeight * _tilemapCamera.aspect;

        // Calculate the scale factor to fit the tilemap within the camera view
        float scaleFactor = Mathf.Min(cameraWidth / tilemapSize.x, cameraHeight / tilemapSize.y);

        // Apply the scale factor to the tilemap
        tilemap.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    private void CaptureAndSaveScreenshot(string fileName, int width, int height)
    {
        // Ensure the directory exists
        if (!Directory.Exists(_saveFolder))
        {
            Directory.CreateDirectory(_saveFolder);
        }

        // Capture screenshot
        RenderTexture rt = new RenderTexture(width, height, 24);
        _tilemapCamera.targetTexture = rt;
        _tilemapCamera.Render();

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        _tilemapCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Save the Texture2D as a PNG file
        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Path.Combine(_saveFolder, fileName + ".png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Saved screenshot to: {filePath}");
    }
}
