using Leaf;
using Leaf.Editor;
using Shipwreck;
using UnityEditor;

static public class LocalizationExporter {
    [MenuItem("Shipwrecks/Export Leaf Strings")]
    static public void ExportAllStrings()
    {
        LeafExport.StringsAsCSV<ScriptNode, LeafNodePackage<ScriptNode>>("Assets", "LocExport.csv", "English", ScriptMgr.GetParser(),
            new LeafExport.CustomRule(typeof(StickyAsset), (s) => StickyAsset.GetLocalizableContent((StickyAsset) s)));
    }
}