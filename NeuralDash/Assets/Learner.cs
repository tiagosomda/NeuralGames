using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using UnityEngine.UI;
using System.IO;

public class Learner : MonoBehaviour
{
    public bool createGenomesFromExisting;

    public float crossOverProbability;
    public float mutationProbability;
    public int currentIteration = 0;
    public int nextIteration = 0;
    public int maxIterations = 200000;

    private List<Brain> thisGenBrains;
    private GeneticAlgorithm geneticAlgorithm;
    private double prevMaxFitness = -1;

    //public GameManager gm;

    public AICharacter[] skipperArray;

    private GeneticAlgorithm ga;

    public void Awake()
    {
        geneticAlgorithm = GeneticAlgorithm.CreateAlgorithm(mutationProbability, crossOverProbability);
    }

    public void AddStudents(AICharacter[] students)
    {
        skipperArray = students;
    }

    public void ActivateBrain()
    {
        for (int i = 0; i < skipperArray.Length; i++)
        {
            var input = skipperArray[i].GetSensorReading();
            if (input == null || !skipperArray[i].isRunning)
            {
                continue;
            }

            var output = skipperArray[i].brain.ProcessSensorData(input);

            //send input action back to player
            skipperArray[i].ProcessBrainOutput(output);
        }
    }

    public void Begin()
    {
        NewGeneration();
    }

    public void NewGeneration()
    {
        if(currentIteration == 0)
        {
            if (createGenomesFromExisting)
            {
                CreateGenomeWithExperience();
            }

            currentIteration = 0;
            foreach (var skipper in skipperArray)
            {
                skipper.ResetSkipper();
            }
        }
        else if (currentIteration > 0)
        {
            if (thisGenBrains == null)
            {
                thisGenBrains = new List<Brain>();
            }
            thisGenBrains.Clear();

            for (int i = 0; i < skipperArray.Length; i++)
            {
                thisGenBrains.Add(skipperArray[i].brain);
            }

            var nextBrainGen = geneticAlgorithm.CreateNextGeneration(thisGenBrains);

            for (int i = 0; i < nextBrainGen.Count; i++)
            {
                skipperArray[i].brain.SetNetworkWeights(nextBrainGen[i].Genes());
            }

            if (currentIteration >= maxIterations)
            {
                return;
            }
        }
    }

    public void EndGeneration()
    {
        if (nextIteration > currentIteration)
        {
            Debug.Log("THIS HAPPENED!!!!");
            return;
        }

        nextIteration++;

        int name = 0;
        var maxScore = 0;

        for (int i = 0; i < skipperArray.Length; i++)
        {
            var skipperScore = (double)(skipperArray[i].GetScore() + 1);
            if (skipperScore > maxScore)
            {
                name = i;
                maxScore = (int)skipperScore;
            }
        }

        for (int i = 0; i < skipperArray.Length; i++)
        {
            var skipperScore = (double)(skipperArray[i].GetScore() + 1);
            
            skipperArray[i].brain.Fitness = skipperScore / maxScore;

        }

        if (maxScore > prevMaxFitness)
        {
            SaveGenome(maxScore, skipperArray[name].brain.genome);
            prevMaxFitness = maxScore;
        }

        currentIteration++;

        foreach (var skipper in skipperArray)
        {
            skipper.ResetSkipper();
        }
    }

    #region Helper Methods
    public void CreateGenomeWithExperience()
    {
        //look for best existing genome
        string SaveFolder = @"E:\Dev\Projects\NeuralGames\NeuralDash\Assets\_game\savedGenomes\";

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

        skipperArray[0].brain.Genes = genome.Genes();
        for (int i = 1; i < skipperArray.Length; i++)
        {
            var newGenome = genome.DeepCopy();

            newGenome.Mutate();
            skipperArray[i].brain.Genes = newGenome.Genes();
        }
    }

    public static void SaveGenome(int score, Genome genome)
    {
        string SaveFolder = @"E:\Dev\Projects\NeuralGames\NeuralDash\Assets\_game\savedGenomes\";

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