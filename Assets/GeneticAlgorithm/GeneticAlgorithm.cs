﻿using System;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    /* Number of parameters in each individual */
    private int n_params;

    /* Number of individuals in each generation */
    private int m_population;

    /* Goal/ Solution parameter set. */
    private Individual target_individual;

    /* Current generation*/
    private Individual[] population;

    /* Fitness values of each individual in current population. */
    private float[] fitness_values;
    /* Current sum of fittness values, calculated each generation. */
    private float sum_fitness_values;

    private float crossover_rate;
    private float mutation_rate;

    private Random random;

    public GeneticAlgorithm(int n_params, int m_population, float crossover_rate, float mutation_rate, float[] target)
    {
        this.n_params = n_params;
        this.m_population = m_population;
        this.crossover_rate = crossover_rate;
        this.mutation_rate = mutation_rate;

        random = new Random();

        population = new Individual[m_population];
        fitness_values = new float[m_population];
        target_individual = new Individual(target);

        for (int i = 0; i < m_population; i++)
        {
            population[i] = new Individual(n_params);//randomly initialize population
        }
    }

    public GeneticAlgorithm()
    {
        random = new Random();
        // stub for testing
    }

    /**
     * Called once per timestep/generation.
     */
    public void RunGA()
    {
        CalculateFitnessValues();
        SumFitnessValues();

        Individual[] selections = Select();

        Individual[] new_population = new Individual[m_population];
        for (int i = 0; i < m_population; i++)
        {
            new_population[i] = CrossOver(selections[i], selections[random.Next(0, m_population)]);// Crossover each chromosome with a random chromosome from selections
        }

        Array.Copy(new_population, population, m_population);// Carryon next generation
    }


    /**
     * Calculates the fitness values of each individual using fitness function defined in 
     * the individual.
     */
    void CalculateFitnessValues()
    {
        for (int i = 0; i < m_population; i++)
        {
            fitness_values[i] = population[i].Fitness(target_individual.Parameters);
        }
    }


    /**
     * Builds chromosomes from the parameters of <em>m</em> and <em>p</em> called parents.  
     * Produces offspring by crossing over bits of parameters defined by crossover rate. 
     * Then mutates (flips a bit on) each Individual at random influenced by mutation rate.
     * 
     * <returns> Individual after the result of crossover operation and mutation. </returns>
     * <param name="m"> Maternal parent. </param>
     * <param name="p"> Paternal parent. </param>
     */
    Individual CrossOver(Individual p, Individual m)//TODO generalize to n parents
    {
        int s = sizeof(float);
        byte[] offspring = new byte[n_params * s];

        BitArray offspring_chromosome = new BitArray(offspring);

        bool[] o_chromosome = new bool[offspring_chromosome.Length];

        int crossover_index = (int) (crossover_rate * (s * 8));

        for (int i = 0; i < n_params; i++)
        {
            byte[] p_bytes = BitConverter.GetBytes(p.Parameters[i]);//initialize byte array for parameter i
            BitArray p_param = new BitArray(p_bytes);

            byte[] m_bytes = BitConverter.GetBytes(m.Parameters[i]);//initialize byte array for parameter i
            BitArray m_param = new BitArray(m_bytes);

            DoCrossOver(p_param, m_param, crossover_index, p_param.Length - 1).CopyTo(o_chromosome, i * p_param.Length);
        }

        o_chromosome = DoMutate(o_chromosome, 0, mutation_rate);

        BitArray o_bitarray = new BitArray(o_chromosome);
        o_bitarray.CopyTo(offspring, 0);

        return new Individual(offspring, n_params);
    }

    /**
     * Calculates how many bits to mutate based on mutation rate, and flips the
     * bits of that many random bits.
     * 
     * <param name="o_chromosome"> Chromosome to mutate. </param>
     * <param name="start_index"> Index in o_chromsome to start mutating (only mutate from start_index to end of array). </param>
     * <param name="mutate_rate"> Determines the number of bits to mutate. calculated as mutate_rate * length of o_chromosome. </param>
     * <returns> The mutated chromosome. </returns>
     */
    public bool[] DoMutate(bool[] o_chromosome, int start_index, float mutate_rate)
    {
        bool[] ret = new bool[o_chromosome.Length];
        Array.Copy(o_chromosome, ret, o_chromosome.Length);
        
        int bitsToMutate = (int) (mutate_rate * o_chromosome.Length);
        for (int i = 0; i < bitsToMutate; i++)
        {
            int randIndex = random.Next(start_index, o_chromosome.Length);
            ret[randIndex] = !o_chromosome[randIndex];
        }

        return ret;
    }



    /**
     * Returns result of crossover: p_param - [0, crossover index] 
     *                              m_param - [crossover index, end of array].
     * 
     * <param name="m_param"> Maternal parent chromosome 32 bit length for floating point (single) type. </param>
     * <param name="p_param"> Paternal parent chromosome 32 bit length for floating point (single) type. </param>
     * <param name="crossover_index"> The index in p_param array to begin cross over. </param>
     * <returns> Offspring produced from crossing over specified region from m_param to p_param. </returns>
     */
    public BitArray DoCrossOver(BitArray p_param, BitArray m_param, int crossover_index, int crossover_index_end)//TODO generalize to n crossover points
    {
        if (crossover_index_end >= p_param.Length || crossover_index >= p_param.Length)
            throw new IndexOutOfRangeException();

        BitArray offspring = new BitArray(p_param.Length);

        for (int j = 0; j < p_param.Length; j++)
        {
            if (j < crossover_index || j > crossover_index_end)
                offspring[j] = p_param[j];
            else
                offspring[j] = m_param[j];
        }

        return offspring;
    }

    /**
     * Selects chromosomes (Individuals) from current population with best relative fit, 
     * determined by roulette wheel method. 
     * 
     * TODO: restructure into atomic testable methods.
     * 
     * <returns> Population of chromosones selected </returns>
     */
    Individual[] Select()
    {
        //-----roulette wheel method--------
        float[] prob = new float[m_population]; //initialize array to hold randomly generated probability values, one for each chromosome(Individual)
        for (int i = 0; i < m_population; i++)
            prob[i] = (float) random.NextDouble();
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
                    Console.WriteLine("INDEXERRORBUG");
                }
                currentIndex++;
            }
       
        }

        Individual[] new_population = new Individual[m_population];

        for (int i = 0; i < m_population; i++)// Select chromosome from population, store in new_population
            new_population[i] = population[indexDistribution[i]];

        return new_population;
    }

    /**
     * Simply sums up all the fitness values of the current generation.
     * 
     * <returns> The sum of the fitness values of each individual in the current population.  </returns>
     */ 
    float SumFitnessValues()
    {
        float sum = 0;
        for (int i = 0; i < m_population; i++)
            sum += fitness_values[i];
        sum_fitness_values = sum;
        return sum;
    }

    /**
     * Calculates the fitness probability of individual <em>i</em>. This calculation is used
     * in roulette wheel selection method.
     * 
     * <param name="i"> The index of the individual to calculate the fitness probability of. </param>
     * <returns> The fitness prbability of individual i </returns>
     */
    float FitnessProbability(int i)
    {
        return fitness_values[i] / sum_fitness_values;
    }


    /**
     * Prints a debug log of current population parameters.
     * 
     * <param name="timeStep"> The timestep that this method was called on, aka the generation number.</param>
     */
    public void PrintOut(int timeStep)
    {
        Console.WriteLine("--------Population: " + timeStep + " ---------------");

        for (int i = 0; i < m_population; i++)
            for (int j = 0; j < n_params; j++)
                Console.WriteLine(population[i].Parameters[j]);
    }

    /**
     * Pakages parameters of individual at <em>index</em> into a string array.
     * 
     * <param name="index"> The index of Individual to get string of</param>
     * 
     * <returns> String array with parameters of individual specified by <em>index</em></returns>
     */
    public string[] DisplayIndividual(int index)
    {
        string[] returnVal = new string[n_params];
        for (int i = 0; i < n_params; i++)
        {
            returnVal[i] = population[index].Parameters[i].ToString();
        }
        return returnVal;
    }

    /**
     * Returns current generation individual at location <em>index</em>.
     * 
     * <returns> Current generation individual at location <em>index</em>. </returns>
     */
    public Individual getIndividual(int index)
    {
        return population[index];
    }
}
