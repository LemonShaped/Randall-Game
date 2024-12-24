using System;
using System.Text.RegularExpressions;
using UnityEditor;

// https://regex101.com

public class CsprojPostprocessor : AssetPostprocessor
{
    public static string OnGeneratedCSProject(string path, string content)
    {
        try
        {

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
        catch (Exception)
        {
            return content;
        }
    }

    public static string OnGeneratedSlnSolution(string path, string content)
    {
        Match projectsMatch = Regex.Match(content, @"^# Visual Studio \d\d(?:\r\n?|\n)+(^Project\("".*^EndProject$(?:\r\n?|\n))^Global$", RegexOptions.Multiline | RegexOptions.Singleline);

        if (!projectsMatch.Success)
            return content;
        string projects = projectsMatch.Result("$1");


        Match assemblyCsharpMatch = Regex.Match(projects, @"(^Project\(""[^""]*""\) = ""[^""]*"", ""Assembly-CSharp\.csproj"", ""[^""]+""(?:\r\n?|\n)^EndProject(?:\r\n?|\n))", RegexOptions.Multiline | RegexOptions.Singleline);

        if (!assemblyCsharpMatch.Success)
            return content;
        string assemblyCsharp = assemblyCsharpMatch.Result("$1");


        return content.Replace(projects, assemblyCsharp + content.Replace(assemblyCsharp, ""));

    }
}
