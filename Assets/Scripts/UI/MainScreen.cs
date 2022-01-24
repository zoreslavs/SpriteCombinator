using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MainScreen : MonoBehaviour
{
	[SerializeField] private ImageGenerator imageGenerator;
	[SerializeField] private PathButton[] pathButtons;
	[SerializeField] private Button generateButton;
	[SerializeField] private Text maxCountText;
	[SerializeField] private InputField targetCountInput;
	[SerializeField] private ProcessingScreen processingScreen;

	private string[] folderPaths;
	private string resultFolderPath;
	private int targetImagesCount;

	private void Awake()
    {
		folderPaths = new string[pathButtons.Length];

		maxCountText.text = targetCountInput.text = "-";
		targetCountInput.interactable = false;
		generateButton.interactable = false;

		foreach (var button in pathButtons)
        {
			button.onPathSetEvent += OnFolderPathSet;
		}

		imageGenerator.onImageGenerated += OnImageGenerated;
	}

	//////
	/// DEBUG
	///
	//private void Start()
	//{
	//	imageGenerator.SetImageList(0, PathHelper.GetDirectoryItems("/Users/macbook/Documents/NFT/Background"));
	//	imageGenerator.SetImageList(1, PathHelper.GetDirectoryItems("/Users/macbook/Documents/NFT/Body"));
	//	imageGenerator.SetImageList(2, PathHelper.GetDirectoryItems("/Users/macbook/Documents/NFT/T-shirt"));
	//	imageGenerator.SetImageList(3, PathHelper.GetDirectoryItems("/Users/macbook/Documents/NFT/Item"));
	//	imageGenerator.SetImageList(4, PathHelper.GetDirectoryItems("/Users/macbook/Documents/NFT/Accessory"));
	//	resultFolderPath = folderPaths[folderPaths.Length - 1] = "/Users/macbook/Documents/NFT/Result";

	//	maxCountText.text = imageGenerator.GetMaxImagesCount().ToString();
	//	targetCountInput.text = "3";
	//	generateButton.interactable = true;
	//}
	//////
	
	private void Update()
	{
		if (!generateButton.interactable || !targetCountInput.interactable || targetCountInput.isFocused)
			return;

		int targetCount = int.Parse(targetCountInput.text);
		if (targetCount < 1 || targetCount > imageGenerator.GetMaxImagesCount())
		{
			targetCountInput.text = imageGenerator.GetMaxImagesCount().ToString();
		}
	}

    private void OnFolderPathSet(ImageType.Type type, string path, string[] items)
    {
		int index = (int)type;
		if (type == ImageType.Type.NONE)
		{
			resultFolderPath = path;
			folderPaths[index] = path;
		}
		else if (items != null && items.Length > 0)
		{
			imageGenerator.SetImageList(index, items);
			folderPaths[index] = path;
		}
		else
        {
			folderPaths[index] = null;
		}

		if (CheckAllFoldersSet())
		{
			maxCountText.text = targetCountInput.text = imageGenerator.GetMaxImagesCount().ToString();
			generateButton.interactable = targetCountInput.interactable = true;
		}
		else if (generateButton.interactable)
        {
			maxCountText.text = targetCountInput.text = "-";
			generateButton.interactable = targetCountInput.interactable = false;
		}
	}

	private void OnImageGenerated(int count)
    {
		if (count < targetImagesCount)
			processingScreen.UpdateProgress(count + 1, targetImagesCount);
		else 
			processingScreen.FinishProcessing(resultFolderPath);
	}

	private bool CheckAllFoldersSet()
    {
		bool result = true;
        for (int i = 0; i < folderPaths.Length; i++)
        {
			if (folderPaths[i] == null)
			{
				result = false;
				break;
			}
		}
		
		return result;
	}

	public void OnGenerate()
	{
		generateButton.interactable = false;
		targetImagesCount = int.Parse(targetCountInput.text);

		processingScreen.gameObject.SetActive(true);
		processingScreen.UpdateProgress(1, targetImagesCount);

		StartCoroutine(GenerateImages());
	}

	private IEnumerator GenerateImages()
	{
		yield return new WaitForSeconds(0.5f);

		imageGenerator.Generate(targetImagesCount, resultFolderPath);
	}
}