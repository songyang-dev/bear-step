using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class OrbUIResponse : LevelSceneSetUp
    {
        [UnityTest]
        public IEnumerator OrbUIDisplayDuration()
        {
            yield return SetUp("Test/cross");

            var ui = root.GetComponentInChildren<OrbMessageUI>();

            ui.Display("Test 1", 1f);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual("Test 1", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
        }
    }
}
