using UnityEditor;


public class PlayerScriptTemplateMenu
{
    [MenuItem("Assets/Create/Player State C# Script", false, 80)]
    public static void CreateCustomScript()
    {
        // Define the path to the template file
        string templatePath = "Assets/Scripts/ScriptTemplate/PlayerStateScript.cs.txt";

        // Define the default name for the new script
        string defaultName = "NewStateScript.cs";

        // Create the script at the selected location in the Project window
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultName);
    }
}
