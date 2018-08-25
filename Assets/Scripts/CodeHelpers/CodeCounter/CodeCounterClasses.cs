using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR


namespace CodeHelpers.CodeCounter
{
    class ProjectScriptsInfo
    {
        List<ScriptInfo> allScript;

        public ProjectScriptsInfo Compare(ProjectScriptsInfo other)
        {
            var theseInfo1 = allScript.ToDictionary(thisInfo => thisInfo.name, thisInfo => thisInfo);
            var theseInfo2 = other.allScript.ToDictionary(thisInfo => thisInfo.name, thisInfo => thisInfo);

            ProjectScriptsInfo result = new ProjectScriptsInfo();

            theseInfo1.ForEach((string thisName) =>
            {
                int otherAmount = 0;
                if (theseInfo2.ContainsKey(thisName)) otherAmount = theseInfo2[thisName].lineCount;

                int difference = otherAmount - theseInfo1[thisName].lineCount;
                if (difference != 0) result.allScript.Add(new ScriptInfo(thisName, difference));
            });

            theseInfo2.ForEach((string thisName) =>
            {
                int otherAmount = 0;
                if (theseInfo1.ContainsKey(thisName)) otherAmount = theseInfo1[thisName].lineCount;

                int difference = otherAmount - theseInfo2[thisName].lineCount;
                if (difference != 0) result.allScript.Add(new ScriptInfo(thisName, difference));
            });

            return result;
        }
    }

    struct ScriptInfo
    {
        public ScriptInfo(string name, int lineCount)
        {
            this.name = name;
            this.lineCount = lineCount;
        }

        public readonly string name;
        public int lineCount;

        public override string ToString()
        {
            return name + " : " + lineCount;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}

#endif