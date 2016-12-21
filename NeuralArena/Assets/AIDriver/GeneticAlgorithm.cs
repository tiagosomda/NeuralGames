using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class GeneticAlgorithm
{
    private List<Genome> m_thisGeneration;
    private List<Genome> m_nextGeneration;

    private double mutationProb;
    private double crossoverProb;
    private bool elitism;
    private List<double> fitnessTable = new List<double>();

    static System.Random random = new System.Random();

    private GeneticAlgorithm()
    {
        //default values

        mutationProb = 0.02;
        crossoverProb = 0.5;
        elitism = false;
    }

    private GeneticAlgorithm(double mutationProbability, double crossoverProbability)
    {
        mutationProb = mutationProbability;
        crossoverProb = crossoverProbability;
        elitism = true;
    }

    public static GeneticAlgorithm CreateAlgorithm(double mutationProbability, double crossoverProbability)
    {
        return new GeneticAlgorithm(mutationProbability, crossoverProbability);
    }

    public List<Genome> CreateNextGeneration(List<Brain> genomePopulation, double mutationProb, double crossoverProb)
    {
        double tempMutatio = mutationProb;
        double tempCrossover = crossoverProb;

        this.mutationProb = mutationProb;
        this.crossoverProb = crossoverProb;

        var next = CreateNextGeneration(genomePopulation);

        mutationProb = tempMutatio;
        crossoverProb = tempCrossover;

        return next;
    }

    public List<Genome> CreateNextGeneration(List<Brain> generation)
    {
        m_thisGeneration = new List<Genome>();

        generation.ForEach(gen => m_thisGeneration.Add(gen.genome));

        var m_crossoverRate = crossoverProb;
        var m_populationSize = m_thisGeneration.Count;
        m_nextGeneration = new List<Genome>();

        RankPopulation();

        Genome g = null;
        if (elitism)
            g = m_thisGeneration[m_populationSize - 1].DeepCopy();

        for (int i = 0; i < m_populationSize; i += 2)
        {
            int pidx1 = RouletteSelection();
            int pidx2 = RouletteSelection();
            Genome parent1, parent2, child1, child2;
            parent1 = m_thisGeneration[pidx1];
            parent2 = m_thisGeneration[pidx2];

            if (random.NextDouble() < m_crossoverRate)
            {
                Crossover(parent1, parent2, out child1, out child2);
                //parent1.Crossover(ref parent2, out child1, out child2);
            }
            else
            {
                child1 = parent1;
                child2 = parent2;
            }
            child1 = Mutate(child1); //child1.Mutate();
            child2 = Mutate(child2); //child2.Mutate();

            m_nextGeneration.Add(child1);
            m_nextGeneration.Add(child2);
        }
        if (elitism && g != null)
            m_nextGeneration[0] = g;

        m_thisGeneration.Clear();
        foreach (Genome ge in m_nextGeneration)
            m_thisGeneration.Add(ge);

        return m_nextGeneration;
    }

    private void RankPopulation()
    {
        m_thisGeneration.Sort(delegate (Genome x, Genome y)
        { return Comparer<double>.Default.Compare(x.Fitness, y.Fitness); });

        //m_thisGeneration.Reverse();

        //  create fitness table
        fitnessTable.Clear();
        m_thisGeneration.ForEach(genome => fitnessTable.Add(genome.Fitness));
    }

    private int RouletteSelection()
    {
        double totalFitness = 0;
        m_thisGeneration.ForEach(g => totalFitness += g.Fitness);

        double randomFitness = random.NextDouble() * totalFitness;
        int idx = -1;
        int mid;
        int first = 0;
        int last = m_thisGeneration.Count - 1;
        mid = (last - first) / 2;

        //  ArrayList's BinarySearch is for exact values only
        //  so do this by hand.
        while (idx == -1 && first <= last)
        {
            if (randomFitness < fitnessTable[mid])
            {
                last = mid;
            }
            else if (randomFitness > fitnessTable[mid])
            {
                first = mid;
            }
            mid = (first + last) / 2;
            //  lies between i and i+1
            if ((last - first) == 1)
                idx = last;
        }
        return idx;
    }

    private Genome Mutate(Genome genome)
    {
        var g = genome;
        for (int pos = 0; pos < genome.m_genes.Length; pos++)
        {
            if (random.NextDouble() < mutationProb)
                g.m_genes[pos] = (g.m_genes[pos] + (random.NextDouble() + random.Next(-20, 20))) / 2.0;
        }
        return g;
    }

    private void Crossover(Genome parent1, Genome parent2, out Genome child1, out Genome child2)
    {
        if (random.NextDouble() > crossoverProb)
        {
            // no crossover
            child1 = parent1;
            child2 = parent2;
            return;
        }

        var m_length = parent1.m_genes.Length;

        int pos = (int)(random.NextDouble() * (double)m_length);
        child1 = new Genome(m_length, false);
        child2 = new Genome(m_length, false);

        for (int i = 0; i < m_length; i++)
        {
            if (i < pos)
            {
                child1.m_genes[i] = parent1.m_genes[i];
                child2.m_genes[i] = parent2.m_genes[i];
            }
            else
            {
                child1.m_genes[i] = parent2.m_genes[i];
                child2.m_genes[i] = parent1.m_genes[i];
            }
        }
    }
}

