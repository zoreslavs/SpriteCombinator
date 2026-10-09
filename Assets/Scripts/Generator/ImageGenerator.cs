using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ImageGenerator : MonoBehaviour
{
    private Texture2D _resultTexture;
    private Texture2D _tempTexture;
    private string resultDirPath;
    private int curImagesCount;
    private int targetImagesCount;

    public event Action<int> onImageGenerated;

    public void Generate(List<string[]> layers, int numImages, string resultPath)
    {
        curImagesCount = 0;
        targetImagesCount = numImages;
        resultDirPath = resultPath;

        foreach (var layer in layers)
        {
            ShuffleHelper.Shuffle(layer);
        }

        StartCoroutine(GenerateImages(layers));
    }

    private IEnumerator GenerateImages(List<string[]> layers)
    {
        int[] indices = new int[layers.Count];

        while (true)
        {
            curImagesCount++;
            if (curImagesCount > targetImagesCount)
            {
                break;
            }

            string[] paths = new string[layers.Count];
            for (int i = 0; i < layers.Count; i++)
            {
                paths[i] = layers[i][indices[i]];
            }

            GenerateImage(paths);

            if (curImagesCount % 10 == 0)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                yield return new WaitForSeconds(1f);
            }

            int carry = layers.Count - 1;
            while (carry >= 0)
            {
                indices[carry]++;
                if (indices[carry] < layers[carry].Length)
                {
                    break;
                }
                indices[carry] = 0;
                carry--;
            }

            if (carry < 0)
            {
                break;
            }
        }
    }

    private void GenerateImage(string[] layerPaths)
    {
        LoadTexture(layerPaths[0], ref _resultTexture);

        for (int i = 1; i < layerPaths.Length; i++)
        {
            LoadTexture(layerPaths[i], ref _tempTexture);
            AlphaBlend(_resultTexture, _tempTexture);
        }

        string resultPath = Path.Combine(resultDirPath, curImagesCount + ".png");
        File.WriteAllBytes(resultPath, _resultTexture.EncodeToPNG());

        onImageGenerated?.Invoke(curImagesCount);
    }

    private void AlphaBlend(Texture2D bottom, Texture2D top)
    {
        Color[] bData = bottom.GetPixels();
        Color[] tData;
        if (top.width == bottom.width && top.height == bottom.height)
        {
            tData = top.GetPixels();
        }
        else
        {
            tData = ScalePixels(top, bottom.width, bottom.height);
        }

        int count = bData.Length;

        for (int i = 0; i < count; i++)
        {
            Color b = bData[i];
            Color t = tData[i];
            float srcF = t.a;
            float destF = 1f - t.a;
            float alpha = srcF + destF * b.a;

            if (alpha <= 0f)
            {
                bData[i] = new Color(0f, 0f, 0f, 0f);
                continue;
            }

            Color r = (t * srcF + b * b.a * destF) / alpha;
            r.a = alpha;
            bData[i] = r;
        }

        bottom.SetPixels(bData);
        bottom.Apply();
    }

    private Color[] ScalePixels(Texture2D src, int dstWidth, int dstHeight)
    {
        Color[] srcData = src.GetPixels();
        int srcWidth = src.width;
        int srcHeight = src.height;
        Color[] dstData = new Color[dstWidth * dstHeight];

        for (int y = 0; y < dstHeight; y++)
        {
            float sy = (dstHeight <= 1) ? 0f : (float)y / (dstHeight - 1) * (srcHeight - 1);
            int y0 = (int)sy;
            int y1 = Mathf.Min(y0 + 1, srcHeight - 1);
            float fy = sy - y0;

            for (int x = 0; x < dstWidth; x++)
            {
                float sx = (dstWidth <= 1) ? 0f : (float)x / (dstWidth - 1) * (srcWidth - 1);
                int x0 = (int)sx;
                int x1 = Mathf.Min(x0 + 1, srcWidth - 1);
                float fx = sx - x0;

                Color c0 = Color.Lerp(srcData[y0 * srcWidth + x0], srcData[y0 * srcWidth + x1], fx);
                Color c1 = Color.Lerp(srcData[y1 * srcWidth + x0], srcData[y1 * srcWidth + x1], fx);
                dstData[y * dstWidth + x] = Color.Lerp(c0, c1, fy);
            }
        }

        return dstData;
    }

    private void LoadTexture(string imagePath, ref Texture2D texture)
    {
        if (texture == null)
        {
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
        }

        texture.LoadImage(File.ReadAllBytes(imagePath));
    }
}