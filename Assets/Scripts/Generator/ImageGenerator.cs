using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ImageGenerator : MonoBehaviour
{
	private List<string[]> images;
	private string[] backgrounds;
	private string[] bodies;
	private string[] shirts;
	private string[] items;
	private string[] acessories;

	Texture2D _mainTexture;
	Texture2D _tempTexture;
	private string resultDirPath;
	private int maxImagesCount;
	private int curImagesCount;
	private int targetImagesCount;

	public event Action<int> onImageGenerated;

	private void Awake()
    {
		images = new List<string[]> { backgrounds, bodies, shirts, items, acessories };
	}

    public void SetImageList(int index, string[] items)
    {
		images[index] = items;

		maxImagesCount = 1;
		foreach (var item in images)
        {
			if (item != null)
				maxImagesCount *= item.Length;
		}
	}

	public void Generate(int numImages, string resultPath)
	{
		curImagesCount = 0;
		targetImagesCount = numImages;
		resultDirPath = resultPath;

		//processingScreen.UpdateProgress(curImagesCount, targetImagesCount);

		RandomizeImagePaths();
		StartCoroutine(GenerateImages());
	}

	private void RandomizeImagePaths()
    {
		foreach (var item in images)
		{
			ShuffleHelper.Shuffle(item);
		}
	}

	private IEnumerator GenerateImages()
    {
		for (int i = 0; i < images.Count; i++)
		{
			for (int i1 = 0; i1 < images[0].Length; i1++)
			{
				for (int i2 = 0; i2 < images[1].Length; i2++)
				{
					for (int i3 = 0; i3 < images[2].Length; i3++)
					{
						for (int i4 = 0; i4 < images[3].Length; i4++)
						{
							for (int i5 = 0; i5 < images[4].Length; i5++)
							{
								curImagesCount++;
								if (curImagesCount > targetImagesCount)
									break;

								yield return GenerateImage(images[0][i1], images[1][i2], images[2][i3], images[3][i4], images[4][i5]);

								if (curImagesCount % 10 == 0)
								{
									Resources.UnloadUnusedAssets();
									GC.Collect();
									yield return new WaitForSeconds(1f);
								}
							}
						}
					}
				}
			}
		}
	}

	private bool GenerateImage(string backgroud, string body, string shirt, string item, string acessory)
	{
		_mainTexture = AlphaBlend(AlphaBlend(AlphaBlend(AlphaBlend(
			GetTexture(backgroud),
			GetTexture(shirt)),
			GetTexture(body)),
			GetTexture(item)),
			GetTexture(acessory));

		string resultPath = Path.Combine(resultDirPath + "/"+ curImagesCount.ToString() + ".png");
		File.WriteAllBytes(resultPath, _mainTexture.EncodeToPNG());

		onImageGenerated.Invoke(curImagesCount);

		return true;
	}
	
	private Texture2D AlphaBlend(Texture2D aBottom, Texture2D aTop)
	{
		if (aBottom.width != aTop.width || aBottom.height != aTop.height)
			throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");

		if (_mainTexture == null)
		{
			_mainTexture = new Texture2D(aTop.width, aTop.height);
			_mainTexture.hideFlags = HideFlags.HideAndDontSave;
		}

		var bData = aBottom.GetPixels();
		var tData = aTop.GetPixels();
		int count = bData.Length;
		var rData = new Color[count];

		for (int i = 0; i < count; i++)
		{
			Color B = bData[i];
			Color T = tData[i];
			float srcF = T.a;
			float destF = 1f - T.a;
			float alpha = srcF + destF * B.a;
			Color R = (T * srcF + B * B.a * destF) / alpha;
			R.a = alpha;
			rData[i] = R;
		}

		_mainTexture.SetPixels(rData);
		_mainTexture.Apply();

		return _mainTexture;
	}

	private Texture2D GetTexture(string imagePath)
    {
		if (_tempTexture != null)
        {
			Destroy(_tempTexture);
			_tempTexture = null;
		}

		_tempTexture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
		_tempTexture.LoadImage(File.ReadAllBytes(imagePath));

		return _tempTexture;
	}

	public int GetMaxImagesCount() { return maxImagesCount; }
}