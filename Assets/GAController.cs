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

    /* Derived from the length of targetParams*/
    private int numParams;

    /* Object handling GA logic*/
    GeneticAlgorithm ga;

    /*FUuture Use*/
    EventRaiser generationDisplayUpdate;//TODO

    Text generationDisplay;//TODO

    Text[] paramDisplays;//TODO

    public GameObject point;

    private GameObject[] objectPool;

    /*
     * Setup GA object with parameters supplied from unity editor.
     * 
     * Starts Time Step coroutine which calls the GA update method and
     * displays results of each generation to the screen.
     * 
     * Temporarily sets up UI elements for displaying GA test data.
     * 
     */
    void Start()
    {
        objectPool = new GameObject[populationSize];
        for (int i = 0; i < objectPool.Length; i++)
        {
            objectPool[i] = GameObject.Instantiate(point, Vector3.zero, point.transform.rotation);
        }

        generationDisplayUpdate = GetComponent<EventRaiser>();

        generationDisplay = GameObject.FindGameObjectWithTag("GenerationDisplay").GetComponent<Text>();
        paramDisplays = generationDisplay.GetComponentsInChildren<Text>();

        numParams = targetParams.Length;
        ga = new GeneticAlgorithm(numParams, populationSize, crossoverRate, mutationRate, targetParams);
        StartCoroutine(TimeStep());
    }

    /*
     * Update loop for running GA.
     * 
     * Defined as a Unity coroutine to allow easy timestep control.
     */
    IEnumerator TimeStep()
    {
        for (int i = 0; i < timeSteps; i++)
        {
            ga.RunGA();

            ga.PrintOut(i);

            string generation = "Generation " + i + "\n";
            string[] generationTable = new string[numParams];
            for (int j = 0; j < populationSize; j++)//populate generation table with parameters from each individual.
            {
                for (int k = 0; k < numParams; k++)
                {
                    generationTable[k] += ga.DisplayIndividual(j)[k] + "\n";
                }
            }

            for (int f = 0 + 1; f < numParams + 1; f++)//display generation table in text boxes on screen
            {
                paramDisplays[f].text = generationTable[f - 1];//TODO
            }

            for (int k = 0; k < populationSize; k++)
            {
                Individual I = ga.getIndividual(k);
//                objectPool[k].transform.position = new Vector3(I.Parameters[0], I.Parameters[1], 0);
            }

            //generationDisplayUpdate.RaiseEvent();
            generationDisplay.text = generation;//TODO uncouple from hard ref to ui element

            yield return new WaitForSeconds(simulationSpeed);
        }
    }

}
