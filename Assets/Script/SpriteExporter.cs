using UnityEngine;
using System.IO;

public class SpriteExporter : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;

    public void Export()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        renderCamera.Render();

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        string path = Application.dataPath + "/merged_sprite.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        Debug.Log("Xuất thành công: " + path);

        RenderTexture.active = currentRT;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Export();
        }
    }
}
