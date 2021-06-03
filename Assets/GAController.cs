using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAController : MonoBehaviour
{
    [Tooltip("Numer of Individual per generation.")]
    public int populationSize = 6;

    [Tooltip("The percentage of the chromosome to crossover with others")]
    public float crossoverRate = 0.5f;

    [Tooltip("The percentage of time a bit in a chromosome gets flipped")]
    public float mutationRate = 0.1f;

    [Tooltip("The set of goal parameters the GA is trying to fit to")]
    public float[] targetParams;

    [Tooltip("Time in seconds to wait in between update steps")]
    public float simulationSpeed = 5f;

    [Tooltip("The total number of timesteps to run the GA")]
    public int timeSteps = 25;

    private int numParams = 1;

    GeneticAlgorithm ga;

    EventRaiser generationDisplayUpdate;

    Text generationDisplay;

    Text[] paramDisplays;

    // Start is called before the first frame update
    void Start()
    {
        generationDisplayUpdate = GetComponent<EventRaiser>();

        generationDisplay = GameObject.FindGameObjectWithTag("GenerationDisplay").GetComponent<Text>();
        paramDisplays = generationDisplay.GetComponentsInChildren<Text>();

        numParams = targetParams.Length;
        ga = new GeneticAlgorithm(numParams, populationSize, crossoverRate, mutationRate, targetParams);
        StartCoroutine(TimeStep());
    }

    IEnumerator TimeStep()
    {
        for (int i = 0; i < timeSteps; i++)
        {
            ga.RunGA();

            ga.PrintOut(i);

            string generation = "Generation " + i + "\n";
            string[] generationTable = new string[numParams];
            for (int j = 0; j < populationSize; j++)
            {
                for (int k = 0; k < numParams; k++)
                {
                    generationTable[k] += ga.DisplayIndividual(j)[k] + "\n";
                }
            }

            for (int f = 0 + 1; f < numParams + 1; f++)
            {
                paramDisplays[f].text = generationTable[f - 1];
            }

            //generationDisplayUpdate.RaiseEvent();
            generationDisplay.text = generation;//TODO uncouple from hard ref to ui element

            yield return new WaitForSeconds(simulationSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
