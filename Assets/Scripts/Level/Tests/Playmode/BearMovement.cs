﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BearMovement : SceneSetUp
    {

        /// <summary>
        /// Tests whether the coroutine behaves as expected
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator BearMovementCoroutine()
        {
            // tests whether the coroutine works properly            
            Vector3 initial;

            foreach (var direction in new List<Direction> {
                Direction.East, Direction.South, Direction.West, Direction.South 
                })
            {
                initial = bear.transform.position;

                bear.Move(direction);
                yield return null;
                Assert.AreNotEqual(bear.transform.position, initial, $"Bear did not move when direction is {direction}");

                yield return new WaitForSeconds(bear.moveDuration);
                Assert.AreEqual(initial + bear.directions[direction], bear.transform.position, $"Bear did not arrive toward {direction}");
            }
        }

        /// <summary>
        /// Tests the logical coordinates of the bear
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator BearMovementLogicalCoordinate()
        {
            // tests whether the coroutine works properly            
            Vector2Int initial;

            var correctCoordinates = new Dictionary<Direction, Vector2Int> {
                {Direction.South, new Vector2Int(0,1)},
                {Direction.East, new Vector2Int(1,0)},
                {Direction.North, new Vector2Int(0,-1)}
            };

            foreach (var direction in correctCoordinates.Keys)
            {
                initial = bear.logicalPosition;

                bear.Move(direction);
                yield return new WaitForSeconds(bear.moveDuration);
                Assert.AreEqual(initial + correctCoordinates[direction], bear.logicalPosition, 
                    $"Bear did not arrive toward {direction}");
            }
        }
    }
}