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

        [Test]
        public void uint32_conversion_and_xover()
        {
            int size = 32;
            UInt32 n = 15;

            GeneticAlgorithm ga = new GeneticAlgorithm();

            BitArray parent1 = new BitArray(BitConverter.GetBytes(n));
            BitArray parent2 = new BitArray(BitConverter.GetBytes(n));

            BitArray offspring = ga.DoCrossOver(parent1, parent2, size / 2);

            byte[] o = new byte[size / 8];
            offspring.CopyTo(o, 0);

            Assert.AreEqual(n, BitConverter.ToUInt32(o, 0));

        }

        [Test]
        public void float_mutate()
        {
            float n = 46f;
            int size = sizeof(float) * 8;

            GeneticAlgorithm ga = new GeneticAlgorithm();

            BitArray chromosome = new BitArray(BitConverter.GetBytes(n));

            bool[] chrom = new bool[size];
            chromosome.CopyTo(chrom, 0);

            int i_start = 10;
            bool[] m_chrom = ga.DoMutate(chrom, i_start, 0.1f);

            for (int i = 0; i < i_start; i++)
                if (chrom[i] != m_chrom[i])
                    Assert.Fail();

            Assert.AreNotEqual(chrom, m_chrom);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
//        [UnityTest]
//        public IEnumerator GATesterWithEnumeratorPasses()
//        {
//            // Use the Assert class to test conditions.
//            // Use yield to skip a frame.
//            yield return null;
//        }
    }
}
