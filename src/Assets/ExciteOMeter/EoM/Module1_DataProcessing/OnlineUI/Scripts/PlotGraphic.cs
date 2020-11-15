using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// using BLEngine.Console;

public class PlotGraphic : MonoBehaviour
{
    
	private List<float> _currentValue;
	private int sizeArray = 100, currentSizeArray;
	private Vector2 referenceSize;	// size of the container of the graph
	private Vector3[] pointsLine;
	private float zOffset = -0.1f;

	public LineRenderer _line;

	// Reference to Input points of the block
	// ConnectionPoint inputPoint;

	void Awake () {
		_line.positionCount = 100;
		_line.startWidth = 2f;
		_line.endWidth = 2f;
	}

	// Use this for initialization
	void Start () {
		//referenceSize = gameObject.transform.Find ("Plotter").GetComponent<RectTransform> ().rect.size;
		referenceSize = gameObject.transform.GetComponent<RectTransform> ().rect.size;
		referenceSize.y = referenceSize.y / 2; // Plot two sided positive and negative Y-values

		_currentValue = new List<float> () {0.0f, 0.0f};

//		CreatePoints ();
		CreatePointsLine ();
	}

	public void SetPoints(List<float> points)
	{
		_currentValue = points;
	}

	// Update is called once per frame
	void Update () {
		/////// Definition of the operation performed in each frame

		// Read array inside graph block
		// _currentValue  = inputPoint.data.numberArray;

		// Create new array of data if the size is different of the previous plotted graphic
		if (_currentValue.Count != sizeArray && _currentValue.Count > 1 ) {
			sizeArray = _currentValue.Count;
			CreatePointsLine ();
		}

		// Plot only if the input array is the same size than points generated
		if (_currentValue.Count == currentSizeArray) {
			// Normalize input data to 0 to 1 range, positive and negative sign
			float[] valuesArray = _currentValue.ToArray();
			float maxVal = Mathf.Abs(valuesArray.Max ());
			float minVal = Mathf.Abs(valuesArray.Min ());
			float maxAbsolute = maxVal > minVal ? maxVal : minVal;

			float scaleFactor = referenceSize.y / maxAbsolute;
            if (maxAbsolute == 0)
                scaleFactor = 1.0f;

			// Plot height with the input data
			for (int i = 0; i < currentSizeArray; i++) {
				pointsLine [i] = new Vector3 (pointsLine [i].x, valuesArray [i] * scaleFactor, zOffset);
			}
			_line.SetPositions (pointsLine);
		}
	}

	// Generate new array of particle points with Y=0
	private void CreatePointsLine()
	{
		_line.positionCount = sizeArray;

		currentSizeArray = sizeArray;
		pointsLine = new Vector3[sizeArray];

		float increment = referenceSize.x / (sizeArray - 1);
		for (int i = 0; i < sizeArray; i++)
		{
			float x = i * increment;
			pointsLine [i] = new Vector3 (x, 0f, zOffset);
		}
		_line.SetPositions (pointsLine);
	}
}
