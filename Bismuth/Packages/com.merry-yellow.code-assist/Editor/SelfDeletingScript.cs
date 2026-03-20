using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
//using Serilog = Meryel.UnityCodeAssist.Serilog;
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor.Setup
{
    [InitializeOnLoad]
    public static class SelfDeletingScript
    {
        static int counter;

        static SelfDeletingScript()
        {
            counter = -4;// start initializing five frames later
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            counter++;

            if (counter != 0)
                return;

            Assister.RegenerateProjectFilesAux(showError: false);

            // delete itself (file), so csproject files are regenerated with analyzer references (AnalyzerPostProcessor.cs effect)
            var scriptMeta = CommonTools.GetScriptPath("SelfDeletingScript.cs.meta");
            System.IO.File.Delete(scriptMeta);
            var script = CommonTools.GetScriptPath("SelfDeletingScript.cs");
            System.IO.File.Delete(script);
        }
    }
}