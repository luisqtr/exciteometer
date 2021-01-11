using UnityEngine;
using System.Collections;
using System.IO;

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
//
// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi
using ExciteOMeter;

public class ScreenRecorder : MonoBehaviour
{
	// 4k = 3840 x 2160   1080p = 1920 x 1080
	public int captureWidth = 1920;
	public int captureHeight = 1080;

	// optional game object to hide during screenshots (usually your scene canvas hud)
	public GameObject hideGameObject;

	// optimize for many screenshots will not destroy any objects so future screenshots will be fast
	public bool optimizeForManyScreenshots = true;

	// configure with raw, jpg, png, or ppm (simple raw format)
	public enum Format { RAW, JPG, PNG, PPM };
	public Format format = Format.PPM;

	// folder to write output (defaults to data path)
	private string folder;

	// private vars for screenshot
	private Rect rect;
	private RenderTexture renderTexture;
	private Texture2D screenShot;
	private int counter = 0; // image #

	private Camera recordingCamera; // From which camera do the recording

	private string last_screenshot_filename;
	private bool isScreenRecorderSetup = false;

	// Singleton
	public static ScreenRecorder instance;

	// Use this for initialization
	void Awake ()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void SetupScreenRecorder(string _folder, Camera _camera)
	{
		folder = _folder;
		recordingCamera=_camera;
		counter = 0;

		isScreenRecorderSetup = true;
	}

	public void StopScreenRecorder()
	{
		isScreenRecorderSetup = false;
	}

	// create a unique filename using a one-up variable
	private string uniqueFilename(int width, int height)
	{
		// use width, height, and counter for unique file name
		last_screenshot_filename = string.Format("screen_{0}x{1}_{2}.{3}", width, height, counter, format.ToString().ToLower());
		var filename = string.Format("{0}/{1}", folder, last_screenshot_filename);

		// up counter for next call
		++counter;

		// return unique filename
		return filename;
	}

	public string CaptureScreenshot()
	{
		// Screenshot recorder has not been setup
		if(!isScreenRecorderSetup)
		{
			Debug.LogError("ScreenRecorder needs to be setup");
			return null;
		}

		// hide optional game object if set
		if (hideGameObject != null) hideGameObject.SetActive(false);

		// create screenshot objects if needed
		if (renderTexture == null)
		{
			// creates off-screen render texture that can rendered into
			rect = new Rect(0, 0, captureWidth, captureHeight);
			renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
			screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
		}

		// get main camera and manually render scene into rt
		recordingCamera.targetTexture = renderTexture;
		recordingCamera.Render();

		// read pixels will read from the currently active render texture so make our offscreen 
		// render texture active and then read the pixels
		RenderTexture.active = renderTexture;
		screenShot.ReadPixels(rect, 0, 0);

		// reset active camera texture and render texture
		recordingCamera.targetTexture = null;
		RenderTexture.active = null;

		// get our unique filename
		string filename = uniqueFilename((int)rect.width, (int)rect.height);

		// pull in our file header/data bytes for the specified image format (has to be done from main thread)
		byte[] fileHeader = null;
		byte[] fileData = null;
		if (format == Format.RAW)
		{
			fileData = screenShot.GetRawTextureData();
		}
		else if (format == Format.PNG)
		{
			fileData = screenShot.EncodeToPNG();
		}
		else if (format == Format.JPG)
		{
			fileData = screenShot.EncodeToJPG();
		}
		else // ppm
		{
			// create a file header for ppm formatted file
			string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
			fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
			fileData = screenShot.GetRawTextureData();
		}

		// create new thread to save the image to file (only operation that can be done in background)
		new System.Threading.Thread(() =>
		{
			// create file and write optional header with image bytes
			var f = System.IO.File.Create(filename);
			if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
			f.Write(fileData, 0, fileData.Length);
			f.Close();
			ExciteOMeterManager.DebugLog(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));
		}).Start();

		// unhide optional game object if set
		if (hideGameObject != null) hideGameObject.SetActive(true);

		// cleanup if needed
		if (optimizeForManyScreenshots == false)
		{
			Destroy(renderTexture);
			renderTexture = null;
			screenShot = null;
		}

		return last_screenshot_filename;
	}
}