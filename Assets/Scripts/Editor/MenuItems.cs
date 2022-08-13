using System.IO;
using UnityEngine;
using UnityEditor;

public class MenuItems
{
    private static string defaultOpenPath = "";
    private static string defaultSavePath = "";

    [MenuItem("Tools/Export IDATA")]
    private static void ExportIDATA()
    {
        string file = EditorUtility.OpenFilePanel("Open IDATA file to extract asset", defaultOpenPath, "BIN");
        string save = EditorUtility.SaveFolderPanel("Save location", defaultSavePath, "");
        defaultOpenPath = Path.GetDirectoryName(file);
        defaultSavePath = Path.GetDirectoryName(save);

        EXP_IDATA.ExtractIDATA(file, save);
    }
}
