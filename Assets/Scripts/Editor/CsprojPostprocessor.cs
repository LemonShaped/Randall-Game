using System;
using System.Text.RegularExpressions;
using UnityEditor;

// https://regex101.com

public class CsprojPostprocessor : AssetPostprocessor
{
    public static string OnGeneratedCSProject(string path, string content)
    {
        try {

            if (!path.EndsWith("Assembly-CSharp.csproj"))
                return content;

            return Regex.Replace(content, @"\s*<Compile Include="".*\.cs""\s*/>".ToString(), "")
                .TrimEnd('\n').TrimEnd('\r')
                .TrimEnd(" </Project>".ToCharArray())
                + @"
  <ItemGroup>
    <Compile Include=""Assets\**\*.cs"" />

    <Content Include=""Assets\**\*.png"" />
    <Content Include=""Assets\**\*.ase"" />
    <Content Include=""*.cs"" />
    <Content Include="".gitattributes"" />
    <Content Include="".gitignore"" />
    <Content Include="".editorconfig"" />
  </ItemGroup>
</Project>
";
        }
        catch (Exception) {
            return content;
        }
    }

    public static string OnGeneratedSlnSolution(string path, string content)
    {
        /*
        Debug.Log("generating sln");
        Debug.Log(content);

        Match projectsMatch = Regex.Match(content, @"^# Visual Studio \d+(?:\r\n?|\n)+(^Project\("".*)(^Project\(""[^""]*""\) = ""[^""]*"", ""Assembly-CSharp\.csproj"", ""[^""]+""(?:\r\n?|\n)^EndProject(?:\r\n?|\n))(.*^EndProject$(?:\r\n?|\n))^Global$",
            RegexOptions.Multiline | RegexOptions.Singleline
        );

        if (!projectsMatch.Success) {
            Debug.Log("sadface");
            return content;
        }

        string projects = projectsMatch.Groups[1].Value + projectsMatch.Groups[2].Value + projectsMatch.Groups[3].Value;
        string assemblyCsharp = projectsMatch.Groups[2].Value;

        Debug.Log("game assembly: " + assemblyCsharp);

        return Regex.Replace(content, Regex.Escape(projects), assemblyCsharp + projectsMatch.Groups[1].Value + projectsMatch.Groups[3].Value);
        */
        return content;
    }
}
