using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeHelpers.CodeCounter
{

#if UNITY_EDITOR

    using UnityEditor;
    using UnityEditor.Callbacks;

    public class CodeCounter : EditorWindow
    {
        [MenuItem("Window/Code Counter")]
        static void Init()
        {
            CodeCounter thisCounter = (CodeCounter)GetWindow(typeof(CodeCounter));

            thisCounter.Show();
            thisCounter.Refresh();
        }

        string scriptsPath = "/Scripts";
        string infoText = "";

        int totalCount;
        List<ScriptInfo> theseInfo;

        Vector2 scrollPosition;

        bool countCodeHelpers;

        void OnGUI()
        {
            if (GUILayout.Button("Refresh")) Refresh();
            scriptsPath = GUILayout.TextField(scriptsPath);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Sort By Name") && theseInfo != null)
            {
                theseInfo = theseInfo.OrderBy(thisInfo => thisInfo.name).ToList();
                RefreshGUI();
            }

            if (GUILayout.Button("Sort By Line Count") && theseInfo != null)
            {
                theseInfo = theseInfo.OrderByDescending(thisInfo => thisInfo.lineCount).ToList();
                RefreshGUI();
            }

            EditorGUILayout.EndHorizontal();

            countCodeHelpers = GUILayout.Toggle(countCodeHelpers, "Count CodeHelpers Lines");

            EditorGUILayout.Space();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label(infoText, GUI.skin.label);
            GUILayout.EndScrollView();
        }

        void Refresh()
        {
            theseInfo = new List<ScriptInfo>();

            totalCount = CheckFolder(Application.dataPath + scriptsPath);

            theseInfo = theseInfo.OrderBy(thisInfo => thisInfo.name).ToList();

            RefreshGUI();
        }

        void RefreshGUI()
        {
            infoText = "Total Count: " + totalCount + "\n";

            theseInfo.ForEach(thisInfo =>
            {
                infoText += "\n" + thisInfo;
            });

            Repaint();
        }

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {

        }

        int CheckFolder(string path)
        {
            if (!countCodeHelpers && path.Contains("CodeHelpers")) return 0;

            int totalLineCount = 0;

            Directory.GetDirectories(path).ForEach(thisPath => totalLineCount += CheckFolder(thisPath));
            Directory.GetFiles(path).ForEach(thisPath => totalLineCount += CheckFile(thisPath));

            return totalLineCount;
        }

        int CheckFile(string path)
        {
            if (path.EndsWith(".cs", System.StringComparison.Ordinal))
            {
                int lineCount = File.ReadAllLines(path).Length;
                string[] pathPieces = path.Split('/');

                theseInfo.Add(new ScriptInfo(pathPieces[pathPieces.Length - 1], lineCount));

                return lineCount;
            }

            return 0;
        }
    }

#endif

}