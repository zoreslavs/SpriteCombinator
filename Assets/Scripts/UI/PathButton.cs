using UnityEngine.UI;
using UnityEngine;
using System;
using SFB;

public class PathButton : MonoBehaviour
{
    [SerializeField] private ImageType.Type type;
    [SerializeField] private Text labelText;
    [SerializeField] private Text pathText;

    public event Action<ImageType.Type, string, string[]> onPathSetEvent;

    private string initLabelText;

    private void Awake()
    {
        initLabelText = labelText.text;
    }

    public void OnPress()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true);
        if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            return;
        }

        pathText.text = paths[0];
        labelText.text = initLabelText;

        string[] items = null;
        if (type != ImageType.Type.NONE)
        {
            items = PathHelper.GetDirectoryItems(paths[0]);
            labelText.text += " (" + items.Length + ")";
        }

        onPathSetEvent.Invoke(type, paths[0], items);
    }
}