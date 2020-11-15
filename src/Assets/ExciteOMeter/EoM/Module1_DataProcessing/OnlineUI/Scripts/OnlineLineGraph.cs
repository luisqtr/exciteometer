using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;
using TMPro;

namespace ExciteOMeter
{
    public class OnlineLineGraph : MonoBehaviour
    {
        private UILineRenderer              graphRenderer;
        public int                          bufferSamples=30;
        public float                        maxValue=1;
        public float                        minValue=0;
        
        public TextMeshProUGUI maxValueText;
        public TextMeshProUGUI minValueText;

        private bool isFirstSample = true;
        private List<float> samples;
        private List<Vector2> newXY;

        void Awake() 
        {
            graphRenderer = GetComponent<UILineRenderer>();    
        }

        void Start()
        {
            RestartPlot();
        }

        public void RestartPlot()
        {
            samples = new List<float>();
            newXY = new List<Vector2>();
            isFirstSample = true;
            CreateStartingPoints();
            UpdateTexts(true);
        }

        public void UpdateLineColor(Color color)
        {
            graphRenderer = GetComponent<UILineRenderer>();
            graphRenderer.color = color;
        }

        public void PlotNewSample(float value)
        {
            AddNewValue(value);
            UpdateTexts();
        }

        void UpdateTexts(bool defaultText=false)
        {
            if(defaultText)
            {
                minValueText.text = "-";
                maxValueText.text = "-";
            }
            else
            {
                minValueText.text = minValue.ToString("F0");
                maxValueText.text = maxValue.ToString("F0");
            }
        }

        async void AddNewValue(float value)
        {
            await Task.Delay(10);

            // Calculate limits
            if(isFirstSample)
            {
                isFirstSample=false;
                minValue = value - 1;
                maxValue = value + 1;

                // Calculate first barch of data
                for (int v=0; v<bufferSamples; v++)
                {
                    samples.Add (value);
                }
            }
            else
            {
                // Modify points of the graph
                samples.RemoveAt(0);
                samples.Add(value);

                // Update min max
                maxValue = samples.Max();
                minValue = samples.Min();
            }

            // Avoid errors
            if (minValue == maxValue)
            {
                maxValue = minValue + 1;
            }

            // Convert data to 0-1 axis
            newXY = new List<Vector2>();

            // Move timestamps to the left
            for (int v=0; v<bufferSamples; v++)
            {
                newXY.Add (
                    new Vector2(CalculateX(v), CalculateY(samples.ElementAt(v)))
                );
            }

            graphRenderer.Points = newXY.ToArray();
        }

        // Updates visual graph with new value
        async void CreateStartingPoints ()
        {
            await Task.Delay(10);

            for (int v=0; v<bufferSamples; v++)
            {
                newXY.Add (
                    new Vector2(CalculateX(v),0.5f)
                );
            }

            // redraw
            graphRenderer.Points = newXY.ToArray();
        }

        float CalculateX(int index)
        {
            return (float)((float)index/((float)bufferSamples-1f));
        }
        float CalculateY(float value)
        {
            return (value-minValue)/(maxValue-minValue);
        }
    }
}