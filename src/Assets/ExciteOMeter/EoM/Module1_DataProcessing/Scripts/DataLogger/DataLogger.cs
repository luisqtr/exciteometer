using System;
using System.Collections.Generic;
using System.IO;

namespace ExciteOMeter
{
    /// <summary>
    /// Manage a file to log data in CSV format
    /// </summary>
    [Serializable]
    public class DataLogger
    {
        public string filename = "datalogger.csv";
        public LogName logID = LogName.UNDEFINED;

        public List<string> headers = new List<string> {"sessionTime", "variableName"};

        // Stores full path including folders
        private string filepath = "";
        private TextWriter file;

        private bool isLogging = false;

        public bool IsLogging { get => isLogging; set => isLogging = value; }


        private string unityTimeStamp;

        public DataLogger()
        {
            // Creates an entity, but InitializeDataLogger needs to be called
            // explicitly to setup the file.
        }

        /// <summary>
        /// Create a log file, header is put exactly as specified in the string.
        /// <summary>
        public DataLogger(string filepath, string header)
        {
            InitializeDataLogger(filepath, new List<string>(){header});
        }

        /// <summary>
        /// Create a log file, headers in the list are separated by commas.
        /// <summary>
        public DataLogger(string filepath, List<string> headers)
        {
            InitializeDataLogger(filepath, headers);
        }

        public void InitializeDataLogger(string filepath, List<string> headers)
        {
            this.filepath = filepath;
            this.headers = headers;
            file = new StreamWriter(filepath, true);

            //builds the string that will be the _header of the csv _file
            string fillHeader = "unityTimeStamp";   // Start of the header file

            for (var i = 0; i < headers.Count; i++)
            {
                fillHeader = fillHeader + "," + headers[i];
            }

            //writes the first line of the _file (_header)
            file.WriteLine(fillHeader);
        }

        ~DataLogger()
        {
            file.Close();
        }

        public void ToggleLoggingState()
        {
            IsLogging = !IsLogging;
        }

        public bool WriteLine(string line)
        {
            if(IsLogging && filepath != "")
            {
                unityTimeStamp = GetTimestamp(DateTime.Now);

                file.WriteLine(unityTimeStamp + "," + line);
                return true;
            }
            // Should reach here only if the file cannot be written.
            return false;
        }

        public void Close()
        {
            if(file !=null)
                file.Close();
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd|HH:mm:ss.fff");
        }
    }
}