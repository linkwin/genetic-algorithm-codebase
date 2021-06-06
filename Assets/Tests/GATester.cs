using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GATester
    {

        [Test]
        public void float_conversion_and_xover()
        {
            GeneticAlgorithm ga = new GeneticAlgorithm();

            BitArray parent1 = new BitArray(BitConverter.GetBytes(15f));
            BitArray parent2 = new BitArray(BitConverter.GetBytes(15f));

            BitArray offspring = ga.DoCrossOver(parent1, parent2, 16);

            byte[] o = new byte[4];
            offspring.CopyTo(o, 0);

            Assert.AreEqual(15f, BitConverter.ToSingle(o, 0));

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GATesterWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
