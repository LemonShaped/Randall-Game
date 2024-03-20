using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


public class CsprojPostprocessor : AssetPostprocessor
{
    public static string OnGeneratedCSProject(string path, string content)
    {
        if (!path.EndsWith("Assembly-CSharp.csproj"))
            return content;

        return PatchCsprojContent(content);
    }

    private static string PatchCsprojContent(string content)
    {
        content = Regex.Replace(content, "\\s*<Compile Include=\".*\\.cs\"\\s*/>".ToString(), "").TrimEnd('\n').TrimEnd('\r').TrimEnd(" </Project>".ToCharArray()) + @"
  <ItemGroup>
    <Compile Include=""Assets\**\*.cs"" />
    <Content Include="".gitattributes"" />
    <Content Include="".gitignore"" />
    <Content Include="".editorconfig"" />
    <Content Include=""Assets\**\*.png"" />
    <Content Include=""Assets\**\*.ase"" />
  </ItemGroup>
</Project>
";
        return content;
    }
}

