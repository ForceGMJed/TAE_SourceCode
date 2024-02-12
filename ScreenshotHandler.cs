using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScreenshotHandler : MonoBehaviour
{

#if UNITY_EDITOR
    private static ScreenshotHandler instance;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

    }
    private void TakeScreenshot()
    {
        StartCoroutine(TakeScreenshotAtEndOfFrame());

    
    }

    public static void TakeScreenShot_static()
    {
        instance.TakeScreenshot();
    }



    IEnumerator TakeScreenshotAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        Debug.Log("width: " + width + "height: " + height);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();

        byte[] byteArray = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", byteArray);



    }


#endif
}



