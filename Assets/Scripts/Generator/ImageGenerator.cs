using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ImageGenerator : MonoBehaviour
{
    private List<string[]> layers = new List<string[]>();

    private Texture2D _resultTexture;
    private Texture2D _tempTexture;
    private string resultDirPath;
    private int maxImagesCount;
    private int curImagesCount;
    private int targetImagesCount;

    public event Action<int> onImageGenerated;

    public void SetLayerImages(int index, string[] images)
    {
        while (layers.Count <= index)
        {
            layers.Add(null);
        }

        layers[index] = images;
        RecalculateMaxCount();
    }

    public void Generate(int numImages, string resultPath)
    {
        curImagesCount = 0;
        targetImagesCount = numImages;
        resultDirPath = resultPath;

        RandomizeLayerOrder();
        StartCoroutine(GenerateImages());
    }

    private void RandomizeLayerOrder()
    {
        foreach (var layer in layers)
        {
            if (layer != null)
            {
                ShuffleHelper.Shuffle(layer);
            }
        }
    }

    private IEnumerator GenerateImages()
    {
        var activeLayers = new List<string[]>();
        foreach (var layer in layers)
        {
            if (layer != null && layer.Length > 0)
            {
                activeLayers.Add(layer);
            }
        }

        if (activeLayers.Count == 0)
        {
            yield break;
        }

        int[] indices = new int[activeLayers.Count];

        while (true)
        {
            curImagesCount++;
            if (curImagesCount > targetImagesCount)
            {
                break;
            }

            string[] paths = new string[activeLayers.Count];
            for (int i = 0; i < activeLayers.Count; i++)
            {
                paths[i] = activeLayers[i][indices[i]];
            }

            GenerateImage(paths);

            if (curImagesCount % 10 == 0)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                yield return new WaitForSeconds(1f);
            }

            int carry = activeLayers.Count - 1;
            while (carry >= 0)
            {
                indices[carry]++;
                if (indices[carry] < activeLayers[carry].Length)
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
        var bData = bottom.GetPixels();
        var tData = top.GetPixels();
        int count = bData.Length;

        for (int i = 0; i < count; i++)
        {
            Color b = bData[i];
            Color t = tData[i];
            float srcF = t.a;
            float destF = 1f - t.a;
            float alpha = srcF + destF * b.a;
            Color r = (t * srcF + b * b.a * destF) / alpha;
            r.a = alpha;
            bData[i] = r;
        }

        bottom.SetPixels(bData);
        bottom.Apply();
    }

    private void LoadTexture(string imagePath, ref Texture2D texture)
    {
        if (texture == null)
        {
            texture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
        }

        texture.LoadImage(File.ReadAllBytes(imagePath));
    }

    public int GetMaxImagesCount() => maxImagesCount;

    private void RecalculateMaxCount()
    {
        maxImagesCount = 1;
        foreach (var layer in layers)
        {
            if (layer != null)
            {
                maxImagesCount *= layer.Length;
            }
        }
    }
}