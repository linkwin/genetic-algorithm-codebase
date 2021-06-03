using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{

    /* Number of parameters in each chromosome/individual i.e. dimensionality */
    private int n_params;
    /* Number of chromosome in each generation */
    private int m_population;

    /* Goal/ Solution */
    private Individual target_individual;

    private Individual[] population;

    private float[] fitness_values;//fitness values of each individual in population
    private float sum_fitness_values;
    private float crossover_rate;
    private float mutation_rate;

    public GeneticAlgorithm(int n_params, int m_population, float crossover_rate, float mutation_rate, float[] target)
    {
        this.n_params = n_params;
        this.m_population = m_population;
        this.crossover_rate = crossover_rate;
        this.mutation_rate = mutation_rate;

        population = new Individual[m_population];
        fitness_values = new float[m_population];
        target_individual = new Individual(target);

        for (int i = 0; i < m_population; i++)
        {
            population[i] = new Individual(n_params);//randomly initialize population
        }
    }

    //Timestep
    public void RunGA()
    {
        CalculateFitnessValues();
        SumFitnessValues();

        Individual[] selections = Select();

        Individual[] new_population = new Individual[m_population];
        for (int i = 0; i < m_population; i++)
        {
            new_population[i] = CrossOver(selections[i], selections[UnityEngine.Random.Range(0, m_population)]);// Crossover each chromosome with a random chromosome from selections
        }

        Array.Copy(new_population, population, m_population);// Carryon next generation
    }


    void CalculateFitnessValues()
    {
        for (int i = 0; i < m_population; i++)
        {
            fitness_values[i] = population[i].Fitness(target_individual.Parameters);
        }
    }

    Individual CrossOver(Individual p, Individual m)
    {
        int s = sizeof(float);
        byte[] offspring = new byte[n_params * s];
        byte[] p_parent = new byte[n_params * s];
        byte[] m_parent = new byte[n_params * s];

        for (int i = 0; i < n_params; i++)
        {
            byte[] p_bytes = BitConverter.GetBytes(p.Parameters[i]);//initialize byte array for parameter i
            for (int j = 0; j < s; j++)
                p_parent[i * s + j] = p_bytes[j];//stack bytes in parent byte array in sets of 4 (for float)

            byte[] m_bytes = BitConverter.GetBytes(m.Parameters[i]);//initialize byte array for parameter i
            for (int j = 0; j < s; j++)
                m_parent[i * s + j] = p_bytes[j];//stack bytes in parent byte array in sets of 4 (for float)
        }

        BitArray offspring_chromosome = new BitArray(offspring);
        BitArray p_parent_chromosome = new BitArray(p_parent);
        BitArray m_parent_chromosome = new BitArray(m_parent);

        int crossover_index = (int) (crossover_rate * (p_parent_chromosome.Length));

        for (int i = 0; i < p_parent_chromosome.Length; i++)
        {
            if (i != crossover_index)
                offspring_chromosome[i] = p_parent_chromosome[i];
            else
                offspring_chromosome[i] = m_parent_chromosome[i];
        }

        //----MUTATE----
        int bitsToMutate = (int) (mutation_rate * offspring_chromosome.Length);
        for (int i = 0; i < bitsToMutate; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, offspring_chromosome.Length);
            offspring_chromosome[randIndex] = !offspring_chromosome[randIndex];
        }

        offspring_chromosome.CopyTo(offspring, 0);//TODO does this work?

        return new Individual(offspring, n_params);
    }

    /**
     * Selects chromosomes (Individuals) from current population with best relative fit, 
     * determined by roulette wheel method. 
     * 
     * <returns> Population of chromosones selected </returns>
     */
    Individual[] Select()
    {
        //-----roulette wheel method--------
        float[] prob = new float[m_population]; //initialize array to hold randomly generated probability values, one for each chromosome(Individual)
        for (int i = 0; i < m_population; i++)
            prob[i] = UnityEngine.Random.Range(0f, 1f);
        Array.Sort(prob);

        int[] selection = new int[m_population]; //initialize array to hold selected chromosomes indicies

        for (int i = 0; i < m_population; i++)
        {

            float fitProb = FitnessProbability(i);
            /*
            *                   { 0                                         if i == 0
            * lower [bound] =   { FitnessProbability(1)                     if i == 1
            *                   { sum of preceding fitness probabilities    otherwise
            */
            float lower = 0;
            if (i == 1)
                lower = FitnessProbability(1);
            else if (i > 1)
                for (int j = i - 1; j >= 0; j--)// Sum preceding fitness probabilities
                    lower += FitnessProbability(j);

            // upper [bound] is lower [bound] + the fitness probability of the current chromosome (i)
            float upper = lower + fitProb;

            int selections = 0; // number of times this chromosome gets selected
            for (int j = 0; j < m_population; j++)
            {
                //if random probability is in range of this cumulative fitness probability
                if (prob[j] >= lower && prob[j] <= upper)
                {
                    selections++;
                }
            }
            selection[i] = selections;
        }

        int[] indexDistribution = new int[m_population];//indexDistribution[i] is the index of which chromosome (in current population) to overwrite chromosome i with.
        int currentIndex = 0;
        for (int i = 0; i < m_population; i++)
        {
            for (int j = 0; j < selection[i]; j++)// For each selection of this chromosone index (i) can be 0
            {
                try
                {
                    indexDistribution[currentIndex] = i;
                }
                catch(IndexOutOfRangeException e)
                {
                    Debug.Log("INDEXERRORBUG");
                }
                currentIndex++;
            }
       
        }

        Individual[] new_population = new Individual[m_population];

        for (int i = 0; i < m_population; i++)// Select chromosome from population, store in new_population
            new_population[i] = population[indexDistribution[i]];

        return new_population;
    }

    float SumFitnessValues()
    {
        float sum = 0;
        for (int i = 0; i < m_population; i++)
            sum += fitness_values[i];
        sum_fitness_values = sum;
        return sum;
    }

    float FitnessProbability(int i)
    {
        return fitness_values[i] / sum_fitness_values;
    }


    public void PrintOut(int timeStep)
    {
        Debug.Log("--------Population: " + timeStep + " ---------------");

        for (int i = 0; i < m_population; i++)
            for (int j = 0; j < n_params; j++)
                Debug.Log(population[i].Parameters[j]);
    }

    public string DisplayIndividual(int index)
    {
        string returnVal = "";
        for (int i = 0; i < n_params; i++)
        {
            returnVal += population[index].Parameters[i];
            if (i != n_params - 1)
                returnVal += ", ";
        }
        return returnVal;
    }
}
