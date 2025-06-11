using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SFB; // Solo si estás usando StandaloneFileBrowser

public class ImageManager : MonoBehaviour
{
    public RawImage imageDisplay;
    public Button uploadButton;
    public Button downloadButton;
    public string uploadUrl = "https://localhost:7233/articulo/upload";
    public string downloadUrl = "https://localhost:7233/articulo/image/";

    private void Start()
    {
        downloadButton.onClick.AddListener(DownloadImage);
    }

    public void OpenFileAndUpload(int i)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        if (paths.Length > 0 && File.Exists(paths[0]))
        {
            StartCoroutine(UploadImageForArticulo(paths[0],i));
        }
        else
        {
            Debug.Log("No file selected.");
        }
#else
        Debug.LogError("File picker only works in Standalone or Editor");
#endif
    }

    /*private IEnumerator UploadCoroutine(string filePath, int articleId)
    {
        byte[] imageData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, Path.GetFileName(filePath), "image/jpeg");
        form.AddField("articleId", articleId.ToString()); // ← Aquí añadimos el ID

        using UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError("Upload failed: " + www.error);
        else
            Debug.Log("Upload successful");
    }*/
    private IEnumerator UploadImageForArticulo(string filePath, int articuloId)
    {
        byte[] imageData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, Path.GetFileName(filePath), "image/jpeg");

        string url = $"https://localhost:7233/articulo/{articuloId}/actualizar-imagen";
        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error al subir la imagen: " + www.error);
        else
            Debug.Log("Imagen subida correctamente al artículo");
    }

    public void DownloadImage()
    {
        StartCoroutine(DownloadCoroutine());
    }

    private IEnumerator DownloadCoroutine()
    {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(downloadUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            imageDisplay.texture = texture;
        }
        else
        {
            Debug.LogError("Failed to download image: " + www.error);
        }
    }

}