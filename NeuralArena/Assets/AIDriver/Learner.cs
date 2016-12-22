using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using UnityEngine.UI;
using System.IO;

public class Learner : MonoBehaviour
{
    public float crossOverProbability;
    public float mutationProbability;

    public int maxIterations = 20000;
    public int iteration;

    private GeneticAlgorithm geneticAlgorithm;
    private List<Brain> brainArr;
    private double prevMaxFitness = -1;

    public void Awake()
    {
        geneticAlgorithm = GeneticAlgorithm.CreateAlgorithm(mutationProbability, crossOverProbability);
        brainArr = new List<Brain>();
    }

    public AICharacter[] NextGen(AICharacter[] population)
    {
        #region END GEN
        int name = 0;
        var maxFitness = 0;

        // GET MAX SCORE
        for (int i = 0; i < population.Length; i++)
        {
            var fitness = (double)(population[i].GetScore() + 1);
            if (fitness > maxFitness)
            {
                name = i;
                maxFitness = (int)fitness;
            }
        }

        // CALCULATE FITNESS
        for (int i = 0; i < population.Length; i++)
        {
            var skipperScore = (double)(population[i].GetScore() + 1);

            population[i].brain.Fitness = skipperScore / maxFitness;

        }

        // SAVE BEST GENOME
        if (maxFitness > prevMaxFitness)
        {
            SaveGenome(maxFitness, population[name].brain.genome);
            prevMaxFitness = maxFitness;
        }
        #endregion

        // NEW GEN
        #region NEW GEN
        brainArr.Clear();

        // RETRIEVE BRAINS!
        for (int i = 0; i < population.Length; i++)
        {
            brainArr.Add(population[i].brain);
        }

        // GENERATE NEXT GEN GENES!
        var nextGenBrainArr = geneticAlgorithm.CreateNextGeneration(brainArr);

        // SET CURRENT GENES
        for (int i = 0; i < population.Length; i++)
        {
            population[i].SetGenes(nextGenBrainArr[i].Genes());
        }
        #endregion

        return population;
    }

    #region Helper Methods
    public T[] CreateGenomeWithExperience<T>(int size) where T : AICharacter
    {
        var AIList = new T[size];
        //look for best existing genome
        string SaveFolder = @"D:\dev\AIRIVER\NeuralGames\NeuralArena\Assets";

        var files = Directory.GetFiles(SaveFolder);
        int max = 0;
        var bestGenome = "";

        foreach (var f in files)
        {

            var filename = f.Remove(0, SaveFolder.Length);
            var parts = filename.Split('.');
            var score = int.Parse(parts[1]);

            if (score > max)
            {
                max = score;
                bestGenome = f;
            }
        }

        var genomeStr = File.ReadAllText(bestGenome);

        var genome = DeserializeGenome(genomeStr);

        AIList[0].brain.Genes = genome.Genes();
        for (int i = 1; i < AIList.Length; i++)
        {
            var newGenome = genome.DeepCopy();

            newGenome.Mutate();
            AIList[i].brain.Genes = newGenome.Genes();
        }

        return AIList;
    }

    public static void SaveGenome(int score, Genome genome)
    {
        string SaveFolder = @"D:\dev\AIRIVER\NeuralGames\NeuralArena\Assets";

        string fileNameTemplate = @"score.{0}.genome";

        string filePath = Path.Combine(SaveFolder, string.Format(fileNameTemplate, score - 1)); 

        List<string> files = new List<string>(Directory.GetFiles(SaveFolder, string.Format(filePath, "*")));

        files.ForEach(file => TryDelete(file));

        File.WriteAllText(filePath, SerializeGenome(genome));
    }

    public static string SerializeGenome(Genome genome)
    {
        var geneStrArray = new string[genome.Genes().Length];
        for (int i = 0; i < geneStrArray.Length; i++)
        {
            geneStrArray[i] = genome.Genes()[i].ToString();
        }
        return string.Join(";", geneStrArray);
    }

    public static Genome DeserializeGenome(string genome)
    {

        var geneStr = genome.Split(';');

        double[] genes = new double[geneStr.Length];

        for (int i = 0; i < geneStr.Length; i++)
        {
            genes[i] = double.Parse(geneStr[i]);
        }

        Genome gen = new Genome(ref genes);

        return gen;
    }

    public static void TryDelete(string file)
    {
        try
        {
            File.Delete(file);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarningFormat("Unable to delete : [{0}] Error: [{1}]", file, ex.Message);
        };
    }
    #endregion
}