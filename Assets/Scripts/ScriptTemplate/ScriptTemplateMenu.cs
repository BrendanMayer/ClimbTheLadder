using UnityEditor;
using UnityEngine;

public class ScriptTemplateMenu
{
    [MenuItem("Assets/Create/State C# Script", false, 80)]
    public static void CreateCustomScript()
    {
        // Define the path to the template file
        string templatePath = "Assets/Scripts/ScriptTemplate/StateScript.cs.txt";

        // Define the default name for the new script
        string defaultName = "NewStateScript.cs";

        // Create the script at the selected location in the Project window
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultName);
    }
}
