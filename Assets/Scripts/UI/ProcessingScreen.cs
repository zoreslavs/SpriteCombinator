using UnityEngine.UI;
using UnityEngine;
using System;

public class ProcessingScreen : MonoBehaviour
{
    [SerializeField] private Text labelText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text folderText;
    [SerializeField] private Button closeButton;
    [SerializeField] private ProgressBar progressBar;

    public event Action onClosed;

    private string initText;

    private void Awake()
    {
        initText = labelText.text;
        ResetScreen();
        gameObject.SetActive(false);
    }

    public void OnClose()
    {
        ResetScreen();
        gameObject.SetActive(false);
        onClosed?.Invoke();
    }

    public void UpdateProgress(float currentCount, float targetCount)
    {
        labelText.text = initText + ": " + currentCount + " / " + targetCount;
        progressBar.UpdateProgress((currentCount - 1) / targetCount);
    }

    public void FinishProcessing(string resultPath)
    {
        labelText.text = "Processing is finished!";
        resultText.text = "Find your images in folder:";
        folderText.text = resultPath;
        progressBar.UpdateProgress(1);
        closeButton.gameObject.SetActive(true);
    }

    private void ResetScreen()
    {
        labelText.text = initText;
        resultText.text = folderText.text = "";
        closeButton.gameObject.SetActive(false);
        progressBar.UpdateProgress(0);
    }
}