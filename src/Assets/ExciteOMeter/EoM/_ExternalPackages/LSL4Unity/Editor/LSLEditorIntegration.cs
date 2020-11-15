using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.Assertions;

namespace Assets.LSL4Unity.EditorExtensions
{
    public class LSLEditorIntegration
    {
        public static readonly string wikiURL = "https://github.com/xfleckx/LSL4Unity/wiki";

        public static readonly string folder64Name = "x86_64";
        public static readonly string folder32Name = "x86";
        public static readonly string libName = "liblsl";
        
        public const string DLL_ENDING = ".dll";
        public const string SO_ENDING = ".so";
        public const string BUNDLE_ENDING = ".bundle";

        static readonly string wrapperFileName = "LSL.cs";
        static readonly string assetSubFolder = "LSL4Unity";
        static readonly string libFolder = "Plugins";

        [MenuItem("LSL/Show Streams")]
        static void OpenLSLWindow()
        {
            var window = EditorWindow.GetWindow<LSLShowStreamsWindow>(true);

            window.Init();

            window.ShowUtility();
        }

        [MenuItem("LSL/Show Streams", true)]
        static bool ValidateOpenLSLWindow()
        {
            string assetDirectory = Application.dataPath;

            bool libAvailable = false;
            bool apiAvailable = false;


            var results = Directory.GetDirectories(assetDirectory, assetSubFolder, SearchOption.AllDirectories);

            Assert.IsTrue(results.Any(), "Expecting a directory named: '" + assetSubFolder + "' containing the content inlcuding this script! Did you renamed it?");

            var root = results.Single();

#if (UNITY_EDITOR_WIN && UNITY_EDITOR_64)
            libAvailable = File.Exists(Path.Combine(root, Path.Combine(libFolder, Path.Combine(folder64Name, libName + DLL_ENDING))));
#elif UNITY_EDITOR_WIN
            libAvailable = File.Exists(Path.Combine(root, Path.Combine(libFolder, Path.Combine(folder32Name, libName + DLL_ENDING))));
#elif UNITY_EDITOR_LINUX && UNITY_EDITOR_64
            libAvailable = File.Exists(Path.Combine(root, Path.Combine(libFolder, Path.Combine(folder64Name, libName + SO_ENDING))));
#elif UNITY_EDITOR_LINUX
            libAvailable = File.Exists(Path.Combine(root, Path.Combine(libFolder, Path.Combine(folder32Name, libName + SO_ENDING))));
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            libAvailable = File.Exists(Path.Combine(root, Path.Combine(libFolder, libName + BUNDLE_ENDING)));
#endif

            apiAvailable = File.Exists(Path.Combine(root, wrapperFileName));

            if (libAvailable && apiAvailable)
                return true;

            Debug.LogError("LabStreamingLayer libraries not available! See " + wikiURL + " for installation instructions");
            return false;
        }

    }
}