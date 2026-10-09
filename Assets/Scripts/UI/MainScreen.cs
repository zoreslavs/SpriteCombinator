using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private ImageGenerator imageGenerator;
    [SerializeField] private LayerEntry layerEntryPrefab;
    [SerializeField] private Transform layerListContainer;
    [SerializeField] private Button addLayerButton;
    [SerializeField] private Button resultFolderButton;
    [SerializeField] private Text resultFolderText;
    [SerializeField] private Button generateButton;
    [SerializeField] private Text maxCountText;
    [SerializeField] private InputField targetCountInput;
    [SerializeField] private ProcessingScreen processingScreen;
    [SerializeField] private int initialLayerCount = 2;
    [SerializeField] private int maxLayerCount = 10;

    private readonly List<LayerEntry> layers = new List<LayerEntry>();
    private string resultFolderPath;
    private int targetImagesCount;

    private void Awake()
    {
        addLayerButton.onClick.AddListener(AddLayer);
        resultFolderButton.onClick.AddListener(SelectResultFolder);
        generateButton.onClick.AddListener(OnGenerate);
        imageGenerator.onImageGenerated += OnImageGenerated;
        processingScreen.onClosed += RefreshState;

        for (int i = 0; i < initialLayerCount; i++)
        {
            AddLayer();
        }

        RefreshState();
    }

    private void Update()
    {
        if (!targetCountInput.interactable || targetCountInput.isFocused)
        {
            return;
        }

        int maxCount = GetMaxImagesCount();
        if (int.TryParse(targetCountInput.text, out int targetCount))
        {
            if (targetCount < 1 || targetCount > maxCount)
            {
                targetCountInput.text = maxCount.ToString();
            }
        }
    }

    private void AddLayer()
    {
        if (layers.Count >= maxLayerCount)
        {
            return;
        }

        LayerEntry entry = Instantiate(layerEntryPrefab, layerListContainer);
        entry.onChanged += RefreshState;
        entry.onRemoved += RemoveLayer;
        layers.Add(entry);
        UpdateLayerOrder();
        RefreshState();
    }

    private void RemoveLayer(LayerEntry entry)
    {
        layers.Remove(entry);
        Destroy(entry.gameObject);
        UpdateLayerOrder();
        RefreshState();
    }

    private void UpdateLayerOrder()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].SetDefaultName(i + 1);

            string label;
            if (layers.Count == 1)
            {
                label = "";
            }
            else if (i == 0)
            {
                label = "bottom";
            }
            else if (i == layers.Count - 1)
            {
                label = "top";
            }
            else
            {
                label = "";
            }
            layers[i].SetOrderLabel(label);
        }
    }

    private void SelectResultFolder()
    {
        string path = FolderPicker.Open("Select Output Folder");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        resultFolderPath = path;
        resultFolderText.text = path;
        RefreshState();
    }

    private void RefreshState()
    {
        int maxCount = GetMaxImagesCount();
        bool ready = maxCount > 0 && !string.IsNullOrEmpty(resultFolderPath);

        maxCountText.text = ready ? maxCount.ToString() : "-";
        targetCountInput.text = ready ? maxCount.ToString() : "-";
        targetCountInput.interactable = ready;
        generateButton.interactable = ready;
        addLayerButton.interactable = layers.Count < maxLayerCount;
    }

    private int GetMaxImagesCount()
    {
        int count = 1;
        int withImages = 0;
        foreach (var layer in layers)
        {
            if (layer.HasImages)
            {
                count *= layer.Images.Length;
                withImages++;
            }
        }
        return withImages > 0 ? count : 0;
    }

    private void OnGenerate()
    {
        List<string[]> activeLayers = new List<string[]>();
        foreach (var layer in layers)
        {
            if (layer.HasImages)
            {
                activeLayers.Add(layer.Images);
            }
        }

        if (activeLayers.Count == 0)
        {
            return;
        }

        generateButton.interactable = false;
        targetImagesCount = int.Parse(targetCountInput.text);

        processingScreen.gameObject.SetActive(true);
        processingScreen.UpdateProgress(1, targetImagesCount);

        StartCoroutine(StartGeneration(activeLayers));
    }

    private IEnumerator StartGeneration(List<string[]> activeLayers)
    {
        yield return new WaitForSeconds(0.5f);
        imageGenerator.Generate(activeLayers, targetImagesCount, resultFolderPath);
    }

    private void OnImageGenerated(int count)
    {
        if (count < targetImagesCount)
        {
            processingScreen.UpdateProgress(count + 1, targetImagesCount);
        }
        else
        {
            processingScreen.FinishProcessing(resultFolderPath);
        }
    }
}