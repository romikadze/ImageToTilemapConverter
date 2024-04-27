using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class Converter : MonoBehaviour
{
    
    public Dictionary<Vector3Int, Color> TilemapData { get; private set; } = new Dictionary<Vector3Int, Color>();
    public Dictionary<Vector3, Color> GizmosData { get; private set; } = new Dictionary<Vector3, Color>();

    [field: Range(1f, 30f)] [field: SerializeField] public int ColorPickRadius { get; private set; }

    [SerializeField] private Texture2D _sourceImage;
    [SerializeField] private int _startX;
    [SerializeField] private int _startY;
    [SerializeField] private int _step;
    [Range(1f, 99f)] [SerializeField] private float _colorDelta;
    
    [Range(1, 3)] [SerializeField] private int _colorPickStep;
    [field: SerializeField] public List<Color> PaletteData { get; private set; }
    
    private List<Color> _colors = new List<Color>();


    public void OnValidate()
    {
        if (_sourceImage == null) return;
        if (_sourceImage.width / _step > 150 || _sourceImage.height / _step > 150 || _step == 0) return;
        
        PaletteData.Clear();
        TilemapData.Clear();
        GizmosData.Clear();
        DrawImage();

        

        for (int i = _startX; i < _sourceImage.width; i += _step)
        {
            for (int j = _startY; j < _sourceImage.height; j += _step)
            {
                Color pixelColor;
                _colors.Clear();

                for (int x = i - ColorPickRadius; x < i + ColorPickRadius; x += _colorPickStep)
                {
                    for (int y = j - ColorPickRadius; y < j + ColorPickRadius; y += _colorPickStep)
                    {
                        var color = _sourceImage.GetPixel(x, y);
                        if (color.a != 0)
                            _colors.Add(color);
                    }
                }

                pixelColor = GetAverageColor(_colors);

                if (pixelColor.a == 0) continue;

                bool isColorSimilar = false;
                int colorIndex = 0;
                foreach (var color in PaletteData)
                {
                    if (DeltaE(RGBToLab(pixelColor), RGBToLab(color)) <= _colorDelta)
                    {
                        pixelColor = color;
                        isColorSimilar = true;
                        colorIndex = PaletteData.IndexOf(color);
                        break;
                    }
                }

                if (!isColorSimilar)
                    PaletteData.Add(pixelColor);
                else
                    PaletteData[colorIndex] =
                        GetAverageColor(new List<Color>() { PaletteData[colorIndex], pixelColor });

                TilemapData.TryAdd(new Vector3Int((i - _startX) / _step, (j - _startY) / _step), pixelColor);

                Vector3 gizmosPosition =
                    transform.TransformPoint(new Vector3(i - _sourceImage.width / 2, j - _sourceImage.height / 2));

                GizmosData.TryAdd(gizmosPosition, pixelColor);
            }
        }
    }

    #region ImageAnalys

    private void DrawImage()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            transform.SetParent(new GameObject("Canvas", typeof(Canvas)).transform);
        }
        else
        {
            transform.SetParent(canvas.transform);
        }

        Sprite sprite = Sprite.Create(_sourceImage, new Rect(0, 0, _sourceImage.width, _sourceImage.height),
            new Vector2(0, 0));
        Image image = GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }

    private Color GetAverageColor(List<Color> colors)
    {
        Color outColor = Color.clear;
        foreach (var color in colors)
        {
            outColor += color;
        }

        if (colors.Count == 0) return Color.clear;
        return outColor / colors.Count;
    }

    private float DeltaE(Vector4 color1, Vector4 color2)
    {
        return (float)Math.Sqrt(Math.Pow((color1.x - color2.x), 2f) + Math.Pow((color1.y - color2.y), 2f) +
                                Math.Pow((color1.z - color2.z), 2f));
    }

    private Vector4 RGBToLab(Vector4 color)
    {
        float[] xyz = new float[3];
        float[] lab = new float[3];
        float[] rgb = { color[0], color[1], color[2], color[3] };

        if (rgb[0] > .04045f)
        {
            rgb[0] = Mathf.Pow((rgb[0] + 0.055f) / 1.055f, 2.4f);
        }
        else
        {
            rgb[0] /= 12.92f;
        }

        if (rgb[1] > .04045f)
        {
            rgb[1] = Mathf.Pow((rgb[1] + 0.055f) / 1.055f, 2.4f);
        }
        else
        {
            rgb[1] /= 12.92f;
        }

        if (rgb[2] > .04045f)
        {
            rgb[2] = Mathf.Pow((rgb[2] + 0.055f) / 1.055f, 2.4f);
        }
        else
        {
            rgb[2] /= 12.92f;
        }

        rgb[0] *= 100.0f;
        rgb[1] *= 100.0f;
        rgb[2] *= 100.0f;


        xyz[0] = ((rgb[0] * .412453f) + (rgb[1] * .357580f) + (rgb[2] * .180423f));
        xyz[1] = ((rgb[0] * .212671f) + (rgb[1] * .715160f) + (rgb[2] * .072169f));
        xyz[2] = ((rgb[0] * .019334f) + (rgb[1] * .119193f) + (rgb[2] * .950227f));


        xyz[0] /= 95.047f;
        xyz[1] /= 100.0f;
        xyz[2] /= 108.883f;

        if (xyz[0] > .008856f)
        {
            xyz[0] = Mathf.Pow(xyz[0], (1.0f / 3.0f));
        }
        else
        {
            xyz[0] = (xyz[0] * 7.787f) + (16.0f / 116.0f);
        }

        if (xyz[1] > .008856f)
        {
            xyz[1] = Mathf.Pow(xyz[1], 1.0f / 3.0f);
        }
        else
        {
            xyz[1] = (xyz[1] * 7.787f) + (16.0f / 116.0f);
        }

        if (xyz[2] > .008856f)
        {
            xyz[2] = Mathf.Pow(xyz[2], 1.0f / 3.0f);
        }
        else
        {
            xyz[2] = (xyz[2] * 7.787f) + (16.0f / 116.0f);
        }

        lab[0] = (116.0f * xyz[1]) - 16.0f;
        lab[1] = 500.0f * (xyz[0] - xyz[1]);
        lab[2] = 200.0f * (xyz[1] - xyz[2]);
        //Debug.Log("L:" + lab[0]);
        //Debug.Log("A:" + lab[1]);
        //Debug.Log("B:" + lab[2]);
        //Debug.Log("W:" + color[3]);

        return new Vector4(lab[0], lab[1], lab[2], color[3]);
    }

    #endregion
}