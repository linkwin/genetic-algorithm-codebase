using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAController : MonoBehaviour
{
    GeneticAlgorithm ga;
    public int populationSize = 6;
    public int numParams = 1;

    public float crossoverRate = 0.5f;
    public float mutationRate = 0.1f;
    public float[] targetParams;

    public float simulationSpeed = 5f;
    public int timeSteps = 25;

    EventRaiser generationDisplayUpdate;

    Text generationDisplay;

    Text[] paramDisplays;

    // Start is called before the first frame update
    void Start()
    {
        generationDisplayUpdate = GetComponent<EventRaiser>();

        generationDisplay = GameObject.FindGameObjectWithTag("GenerationDisplay").GetComponent<Text>();
        paramDisplays = generationDisplay.GetComponentsInChildren<Text>();

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
