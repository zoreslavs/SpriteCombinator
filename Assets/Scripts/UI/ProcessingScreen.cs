using UnityEngine.UI;
using UnityEngine;

public class ProcessingScreen : MonoBehaviour
{
    [SerializeField] private Text labelText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text folderText;
    [SerializeField] private Button closeButton;
    [SerializeField] private ProgressBar progressBar;

    private string initText;

    private void Awake()
    {
        initText = labelText.text;
        resultText.text = folderText.text = "";
        gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        Application.Quit();
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
}