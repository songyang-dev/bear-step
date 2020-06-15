using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests {
    public class MenuSceneSetUp
    {
        protected GameObject root;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            root = GameObject.Instantiate(
                (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/MainMenuObjects.prefab", typeof(GameObject))
                );
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