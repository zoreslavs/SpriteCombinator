public static class FolderPicker
{
    public static string Open(string title)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.OpenFolderPanel(title, "", "");
#else
        string[] paths = SFB.StandaloneFileBrowser.OpenFolderPanel(title, "", false);
        if (paths == null || paths.Length == 0)
        {
            return "";
        }
        return paths[0];
#endif
    }
}