using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureArrayExporter : MonoBehaviour
{
    [MenuItem("Tools/Texture/Export Texture2DArray to PNG")]
    public static void ExportToPNG()
    {
        // 1. 获取 Texture2DArray
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Texture2DArray texArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(path);
        if (texArray == null)
        {
            Debug.LogError("请选择有效的 Texture2DArray 文件！");
            return;
        }

        // 2. 检查 Shader 是否存在
        Shader blitShader = Shader.Find("Universe/BlitCopy");
        if (blitShader == null)
        {
            Debug.LogError("Shader 'Universe/BlitCopy' 未找到！请确保 Shader 存在并已正确加载。");
            return;
        }

        // 3. 创建输出文件夹（改为相对路径）
        string outputPath = Path.Combine("Assets", "ExportedTextures");
        if (!AssetDatabase.IsValidFolder(outputPath))
        {
            AssetDatabase.CreateFolder("Assets", "ExportedTextures");
        }

        // 4. 遍历每一层
        for (int i = 0; i < texArray.depth; i++)
        {
            // 使用 RenderTexture 中转解决压缩格式问题
            RenderTexture rt = RenderTexture.GetTemporary(texArray.width, texArray.height);
            Material blitMaterial = new Material(blitShader);
            blitMaterial.SetInt("_ArrayIndex", i);
            Graphics.Blit(texArray, rt, blitMaterial);
            
            Texture2D tempTex = new Texture2D(texArray.width, texArray.height, TextureFormat.RGBA32, false);
            RenderTexture.active = rt;
            tempTex.ReadPixels(new Rect(0, 0, texArray.width, texArray.height), 0, 0);
            tempTex.Apply();
            RenderTexture.ReleaseTemporary(rt);

            // 保存 PNG
            byte[] pngData = tempTex.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(outputPath, $"TextureSlice_{i}.png"), pngData);
        }

        AssetDatabase.Refresh();
        Debug.Log($"导出完成！共 {texArray.depth} 张 PNG，路径: {outputPath}");
    }
}
