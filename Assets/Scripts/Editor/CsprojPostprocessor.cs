using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


public class CsprojPostprocessor : AssetPostprocessor
{
    public static string OnGeneratedCSProject(string path, string content) {

        if (!path.EndsWith("Assembly-CSharp.csproj"))
            return content;

        return Regex.Replace(content, @"\s*<Compile Include="".*\.cs""\s*/>".ToString(), "")
            .TrimEnd('\n').TrimEnd('\r')
            .TrimEnd(" </Project>".ToCharArray())
            + @"
  <ItemGroup>
    <Content Include=""Assets\**\*.png"" />
    <Content Include=""Assets\**\*.ase"" />
    <Compile Include=""Assets\**\*.cs"" />
    <Content Include=""*.cs"" />
    <Content Include="".gitattributes"" />
    <Content Include="".gitignore"" />
    <Content Include="".editorconfig"" />
  </ItemGroup>
</Project>
";
    }

    public static string OnGeneratedSlnSolution(string path, string content) {
        return content;
    }
}

