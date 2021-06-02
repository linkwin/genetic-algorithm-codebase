﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAController : MonoBehaviour
{
    GeneticAlgorithm ga;
    public int populationSize = 6;
    public float simulationSpeed = 5f;
    public int timeSteps = 25;

    // Start is called before the first frame update
    void Start()
    {
        ga = new GeneticAlgorithm(1, 6);
        StartCoroutine(TimeStep());
    }

    IEnumerator TimeStep()
    {
        for (int i = 0; i < timeSteps; i++)
        {
            ga.RunGA();

            ga.PrintOut(i);

            yield return new WaitForSeconds(simulationSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}