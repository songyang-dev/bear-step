using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests {
    public class LevelSceneSetUp
    {
        protected GameObject root;

        protected IEnumerator SetUp(string level)
        {
            root = GameObject.Instantiate(
                (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelGameObjects.prefab", typeof(GameObject))
                );
            root.GetComponentInChildren<GameManager>().test = true;
            root.GetComponentInChildren<GameManager>().testLevel = level;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.Destroy(root);
            yield return null;
        }
    }
}