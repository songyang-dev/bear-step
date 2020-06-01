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
            yield return new WaitForSeconds(2f);
            Assert.AreEqual("Test 1", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
        }

        [UnityTest]
        public IEnumerator ChainedCollection()
        {
            yield return SetUp("Test/orbRow");

            var ui = root.GetComponentInChildren<OrbMessageUI>();

            ui.Display("Orb 1", .1f);
            ui.Display("Orb 2", .1f);
            ui.Display("Orb 3", .1f);
            ui.Display("Orb 4", .1f);
            
            Assert.False(ui.GetComponent<Text>().text == null, "No text found");

            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual("Orb 1", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
            yield return new WaitForSeconds(1.5f);
            Assert.AreEqual("Orb 2", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
            yield return new WaitForSeconds(1.7f);
            Assert.AreEqual("Orb 3", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
            yield return new WaitForSeconds(1.7f);
            Assert.AreEqual("Orb 4", ui.GetComponent<Text>().text, $"Text is not the same: {ui.GetComponent<Text>().text}");
        }
    }
}
