using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FileModificationWarning : AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        foreach (string path in paths)
            if (path.StartsWith("Assets/Scenes/") && path.EndsWith(".unity")) {
                //Debug.Log("Saved. Do something now.");
                break;
            }
        return paths;
    }
}
//"H:\vscode\pwsh test\poweshl.exe" cd H:\Projects\Unity\Randall-Game; git add .; git commit -m "automated: $(Get-Date -format 'yyyy-MM-dd HH:mm:ss')"; git branch hyperspecific; git pull; git push; while ($true){pause};
