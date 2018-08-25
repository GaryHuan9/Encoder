using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace CodeHelpers
{

    public static class FileHelper
    {
        public static void ForEachFile(string path, Action<string> thisAction)
        {
            Debug.Log(path);
            Directory.GetDirectories(path).ForEach(thisPath => ForEachFile(thisPath, thisAction));
            Directory.GetFiles(path).ForEach(thisPath => thisAction(path));
        }
    }
}