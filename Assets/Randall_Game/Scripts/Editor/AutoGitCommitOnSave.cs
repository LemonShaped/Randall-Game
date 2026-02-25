using UnityEditor;

public class AutoGitCommitOnSave : AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        foreach (string path in paths)
            if (path.StartsWith("Assets/Scenes/") && path.EndsWith(".unity")) {
                //Debug.Log("Ctrl+S was pressed. Auto-committing.");

                //git commit
                // Process.Start("git", "branch auto-commit-qw123aq");
                // Process.Start("git", "add .");
                // Process.Start("git", "commit -m \"Auto-commit\"");
                break;
            }
        return paths;
    }
}
//"H:\vscode\pwsh test\poweshl.exe" cd H:\Projects\Unity\Randall-Game; git add .; git commit -m "automated: $(Get-Date -format 'yyyy-MM-dd HH:mm:ss')"; git branch hyperspecific; git pull; git push; while ($true){pause};
