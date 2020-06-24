using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LinkReferences : Editor
{
    [MenuItem("Prefabs/Link All", false, 1)]
    static void Link()
    {
        LevelPrefabLinker.SetAllReferences();
        LevelAssetPrefabLinker.SetAllReferences();
        AnimationSetter.SetAllAnimations();
    }
}
