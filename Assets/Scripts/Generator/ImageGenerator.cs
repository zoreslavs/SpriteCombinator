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

								GenerateImage(images[0][i1], images[1][i2], images[2][i3], images[3][i4], images[4][i5]);

								yield return new WaitForSeconds(2f);
							}
						}
					}
				}
			}
		}
	}

	private void GenerateImage(string backgroud, string body, string shirt, string item, string acessory)
	{
		Texture2D texture1 = ImageHelper.AlphaBlend(GetTexture(backgroud), GetTexture(shirt));
		Texture2D texture2 = ImageHelper.AlphaBlend(texture1, GetTexture(body));
		Texture2D texture3 = ImageHelper.AlphaBlend(texture2, GetTexture(item));
		Texture2D texture4 = ImageHelper.AlphaBlend(texture3, GetTexture(acessory));

		string resultPath = Path.Combine(resultDirPath + "/"+ curImagesCount.ToString() + ".png");
		File.WriteAllBytes(resultPath, texture4.EncodeToPNG());

		onImageGenerated.Invoke(curImagesCount);
	}

	private Texture2D GetTexture(string imagePath)
    {
		Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
		texture.LoadImage(File.ReadAllBytes(imagePath));

		return texture;
	}

	public int GetMaxImagesCount() { return maxImagesCount; }
}