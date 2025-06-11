using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;
public class ImageDownloader : MonoBehaviour
{
    public string getImageUrl = "https://localhost:7233/articulo/image/1"; // Cambia el ID según tu imagen
    public RawImage targetImage;
    public static ImageDownloader instanceDownloader;
    private void Start()
    {
        Debug.Log("Inicia");
        instanceDownloader=this;
    }
    private void Awake()
    {
        Debug.Log("Inicia");
        instanceDownloader = this;
    }
    public void DownloadImage()
    {
        Debug.Log("Inicia DOWNLOAD");
        StartCoroutine(DownloadCoroutine());
    }

    IEnumerator DownloadCoroutine()
    {
        Debug.Log("Inicia CORUTINA");
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(getImageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            targetImage.texture = texture;
        }
        else
        {
            Debug.LogError("Failed to download image: " + www.error);
        }
    }
}