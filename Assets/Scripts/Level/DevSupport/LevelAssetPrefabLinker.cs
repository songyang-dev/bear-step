using UnityEditor;
using UnityEngine;

/// <summary>
/// Links the assets in the level scene with correct references
/// </summary>
public class LevelAssetPrefabLinker : Editor
{
    private const string bearPrefabPath = "Assets/Prefabs/Bear.prefab";
    private static GameObject bearRoot;

    private const string orbPrefabPath = "Assets/Prefabs/Orb.prefab";
    private static GameObject orbRoot;

    private const string tilePrefabPath = "Assets/Prefabs/Tile.prefab";
    private static GameObject tileRoot;

    [MenuItem("Prefabs/Link Level Assets")]
    public static void SetAllReferences()
    {
        SetUp();

        TraverseBear(bearRoot);
        TraverseOrb(orbRoot);
        TraverseTile(tileRoot);

        CleanUp();
    }

    private static void TraverseTile(GameObject tileRoot)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(tileRoot);

        // add camera controller script back if needed
        if (count == 0) DestroyImmediate(tileRoot.GetComponent<Tile>());
        
        var tile = tileRoot.AddComponent<Tile>();

        tile.StateChangeDistance = 1;
        tile.StateChangeDuration = 1;

    }

    private static void TraverseOrb(GameObject orbRoot)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(orbRoot);

        // add camera controller script back if needed
        if (count == 0) DestroyImmediate(orbRoot.GetComponent<Orb>());

        var orb = orbRoot.AddComponent<Orb>();

    }

    private static void TraverseBear(GameObject bearRoot)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(bearRoot);

        // add camera controller script back if needed
        if (count == 0) DestroyImmediate(bearRoot.GetComponent<Bear>());

        var bear = bearRoot.AddComponent<Bear>();

        bear.MoveDuration = 0.5f;
        bear.MoveDistance = 1;
        bear.LogicalPosition = new Vector2Int();
    }

    private static void SetUp()
    {
        bearRoot = PrefabUtility.LoadPrefabContents(bearPrefabPath);
        orbRoot = PrefabUtility.LoadPrefabContents(orbPrefabPath);
        tileRoot = PrefabUtility.LoadPrefabContents(tilePrefabPath);
    }

    private static void CleanUp()
    {
        var pairs = new (GameObject, string)[]{
            (bearRoot, bearPrefabPath),
            (orbRoot, orbPrefabPath),
            (tileRoot, tilePrefabPath)
        };
        foreach (var item in pairs)
        {
            var (root, prefabPath) = item;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
