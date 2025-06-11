using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageUploader : MonoBehaviour
{
    public string uploadUrl = "https://localhost:7233/articulo/upload"; // Cambia a tu URL real
    ImageUploader instanceUploader;
    private void Start()
    {
        instanceUploader= this;
    }
    public void UploadImage(string path)
    {
        StartCoroutine(UploadCoroutine(path));
    }

    IEnumerator UploadCoroutine(string filePath)
    {
        byte[] imageData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, Path.GetFileName(filePath), "image/jpeg");

        using UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError("Upload failed: " + www.error);
        else
            Debug.Log("Upload successful");
    }
}