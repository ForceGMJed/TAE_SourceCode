using UnityEngine;

public class ScreenController : MonoBehaviour
{

    public static ScreenController instance;


    private int lastStableWidth = 0;
    private int lastStableHeight = 0;
    //private int updateWidth;
    //private int updateHeight;

    private int frameLapse;
    [SerializeField]
    private int updateAspectDelay = 20;

    private void Awake()
    {
        if (instance != null && instance != this)
        { 
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        lastStableWidth = Screen.width;
        lastStableHeight = Screen.height;
    }


    void Update()
    {
        frameLapse++;

        if (frameLapse % updateAspectDelay != 0)
            return;

        frameLapse = 0;

        var width = Screen.width; var height = Screen.height;

        width = Screen.width;
        height = Screen.height;

        if (lastStableWidth != width) // if the user is changing the width
        {
            // update the height
            var heightAccordingToWidth = width / 16.0 * 9.0;
            Screen.SetResolution(width, (int)Mathf.Round((float)heightAccordingToWidth), false, 0);
        }
        else if (lastStableHeight != height) // if the user is changing the height
        {
            // update the width
            var widthAccordingToHeight = height / 9.0 * 16.0;
            Screen.SetResolution((int)Mathf.Round((float)widthAccordingToHeight), height, false, 0);

        }

        lastStableWidth = width;
        lastStableHeight = height;


    }


    //private void LateUpdate()
    //{

    //    var lateFramewidth = Screen.width; var lateFrameheight = Screen.height;


    //    //user still dragging the frame
    //    if (updateWidth != lateFramewidth || updateHeight != lateFrameheight)
    //    {
    //        Debug.LogError("user still dragging the frame");
    //        return;
    //    }

    //    //if User never dragged 
    //    if (lateFramewidth == lastStableWidth || lateFrameheight == lastStableHeight)
    //    {
    //        return;
    //    }



    //    Debug.LogError("lateFramewidth :" + lateFramewidth);
    //    Debug.LogError("lateFrameheight :" + lateFrameheight);

    //    //user stopped draggin and the current late frame window dimensions are not the same as the last stable window dimensions

    //    if (lastStableWidth != lateFramewidth)
    //    {
    //        // update the height
    //        var heightAccordingToWidth = lateFramewidth / 16.0 * 9.0;
    //        Screen.SetResolution(lateFramewidth, (int)Mathf.Round((float)heightAccordingToWidth), false, 0);

    //        Debug.LogError("Screen width updated");
    //    }
    //    else if (lastStableHeight != lateFrameheight)
    //    {
    //        // update the width
    //        var widthAccordingToHeight = lateFrameheight / 9.0 * 16.0;
    //        Screen.SetResolution((int)Mathf.Round((float)widthAccordingToHeight), lateFrameheight, false, 0);

    //        Debug.LogError("Screen height updated");
    //    }

    //    lastStableWidth = lateFramewidth;
    //    lastStableHeight = lateFrameheight;

    //    Debug.LogError("Screen size updated");

    //}

}

