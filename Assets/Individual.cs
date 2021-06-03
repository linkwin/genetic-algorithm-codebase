using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Individual
{
    float[] parameters;
    public int n_params;

    public float[] Parameters { get => parameters; set => parameters = value; }

    public Individual(int n_params)
    {
        this.n_params = n_params;
        parameters = new float[n_params];
        for (int i = 0; i < n_params; i++)
            parameters[i] = UnityEngine.Random.Range(-50f, 50f);//TODO magic number
    }

    public Individual(byte[] chromosome, int n_params)
    {
        this.n_params = n_params;
        parameters = new float[n_params];
        if (chromosome.Length != n_params * sizeof(float))
            Debug.LogError("Chromosome length does not match!");
        
        for (int i = 0; i < n_params; i++)
            parameters[i] = BitConverter.ToSingle(chromosome, i * sizeof(float));
    }

    public Individual(float[] p)
    {
        parameters = p;
        n_params = p.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    /**
     * Camputes distance between <em>parameters</em> and <em>target</em> if treated as a
     * vector in n-dimensional space;
     * 
     * <param name="goal"> The target set of parameters </param>
     * 
     * <returns> 1 / Euclidian distance between <em>parameters</em> and <em>target</em> </returns>
     */
    public float Fitness(float[] goal)
    {
        float sum = 0;
        for (int i = 0; i < n_params; i++)
        {
            sum += (parameters[i] - goal[i]) * (parameters[i] - goal[i]);
        }

        return 1 / Mathf.Sqrt(sum);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
