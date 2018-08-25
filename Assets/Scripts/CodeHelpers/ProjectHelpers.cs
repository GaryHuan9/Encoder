using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

namespace CodeHelpers.ProjectHelpers
{

#if UNITY_EDITOR

    using UnityEditor;

    public class ProjectHelper : EditorWindow
    {
        [MenuItem("Window/Project Helper")]
        static void Init()
        {
            GetWindow(typeof(ProjectHelper)).Show();
        }

        void OnGUI()
        {

        }
    }

    public class Try : EditorWindow
    {
        [MenuItem("Window/Try")]
        static void Init()
        {
            Try thisWindow = (Try)GetWindow(typeof(Try));

            thisWindow.Show();
            thisWindow.thisPath = Application.dataPath;
        }

        string thisPath;

        void OnGUI()
        {
            thisPath = EditorGUILayout.TextField(thisPath);

            EditorGUILayout.LabelField(Path.GetExtension(thisPath));
        }
    }

#endif
}