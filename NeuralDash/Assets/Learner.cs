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

    private List<AICharacter> characters;
    private List<Brain> thisGenBrains;
    private GeneticAlgorithm geneticAlgorithm;
    private double prevMaxFitness = -1;
    private bool stopRunning;

    public GameManager gm;
    public int simulationCount;
    public float characterSpacing;

    public GameObject skipperPrefab;
    public Character[] skipperArray;


    public GameObject DataItemPrefab;
    public GameObject DataItemParent;
    public static GameObject _DataItemPrefab;
    public static GameObject _DataItemParent;

    public static List<string> data_names;
    public static List<Text> data_values;


    private GeneticAlgorithm ga;

    public bool isLearning = false;
    public bool noneRunning = false;

    public int input = 3;
    public int output = 1;
    public int[] hidden = new int[2]{ 4, 4 };

    public Genome genome;

    public void Awake()
    {
        geneticAlgorithm = GeneticAlgorithm.CreateAlgorithm(mutationProbability, crossOverProbability);
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void Start()
    {
        isLearning = false;

        skipperArray = new Character[simulationCount];

        for (int i = 0; i < simulationCount; i++)
        {
            var skipperGameObject = Instantiate(skipperPrefab);
            skipperGameObject.transform.SetParent(gameObject.transform);

            var pos = skipperGameObject.transform.position;
            pos.x -= characterSpacing * i;
            skipperGameObject.transform.position = pos;

            skipperGameObject.name = "[" + i + "]";
            skipperArray[i] = skipperGameObject.GetComponent<Character>();
        }

        ga = GeneticAlgorithm.CreateAlgorithm(mutationProbability, crossOverProbability);
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartLearning();
            gm.ResetGame();
            isLearning = true;
        }

        if (!isLearning)
        {
            return;
        }

        foreach (var skipper in skipperArray)
        {
            if (skipper.isRunning)
            {
                noneRunning = false;
                break;
            }

            noneRunning = true;
        }

        if (noneRunning)
        {
            isLearning = false;
            EndGeneration();
            NewGeneration();
            return;
        }

        for (int i = 0; i < simulationCount; i++)
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

    public void StartLearning()
    {
        Debug.Log("Starting learning process...");
        //ga.Go();
        //ga.CreateFitnessTable();

        if (createGenomesFromExisting)
        {
            CreateGenomeWithExperience();
        }

        currentIteration = 0;
        NewGeneration();

        foreach (var skipper in skipperArray)
        {
            skipper.ResetSkipper();
        }
    }

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
        for (int i = 1; i < simulationCount; i++)
        {
            var newGenome = genome.DeepCopy();

            newGenome.Mutate();
            skipperArray[i].brain.Genes = newGenome.Genes();
        }
    }

    public void NewGeneration()
    {
        if (currentIteration > 0)
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

            //var totalFfitness = ga.SortInFitnessOrder();
            //ga.AddTotalFitness(totalFfitness);
            //ga.CreateNextGeneration();
            //ga.SetTotalFitness(0);
            //var maxIterations = ga.GetMaxIterations();

            if (currentIteration >= maxIterations)
            {
                return;
            }
        }

        isLearning = true;
        noneRunning = false;
        gm.ResetGame();
        gm.SetSpawning(true);
    }

    public void EndGeneration()
    {
        if (nextIteration > currentIteration)
        {
            return;
        }

        nextIteration++;

        gm.SetSpawning(false);
        gm.GameOver();

        int name = 0;
        var maxScore = 0;
        for (int i = 0; i < simulationCount; i++)
        {
            var skipperScore = (double)(skipperArray[i].GetScore() + 1);
            var gameScore = (double)(gm.score + 1);
            skipperArray[i].brain.Fitness = skipperScore / gameScore;

            if (skipperScore > maxScore)
            {
                name = i;
                maxScore = (int)skipperScore;
            }
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

    public BackpropagationNetwork CreateNetwork(int input, int output, int[] hidden)
    {
        LinearLayer inputLayer = new LinearLayer(input);
        SigmoidLayer outputLayer = new SigmoidLayer(output);

        // minimum size
        if (hidden == null)
        {
            hidden = new int[] { input + 1, input + 1 };
        }

        var hiddenLayers = new SigmoidLayer[hidden.Length];

        // plus two because of the input and the output layers
        var connectors = new BackpropagationConnector[hidden.Length + 2];

        // create the hidden layers
        for (int k = 0; k < hidden.Length; k++)
        {
            hiddenLayers[k] = new SigmoidLayer(hidden[k]);
        }

        // back propagation from first hidden layer to input
        connectors[0] = new BackpropagationConnector(inputLayer, hiddenLayers[0]);

        // back propagation between the hidden layers
        for (int k = 1; k < hidden.Length; k++)
        {
            connectors[k] = new BackpropagationConnector(hiddenLayers[k - 1], hiddenLayers[k]);
        }
        // back propagation from output to last hidden layer
        connectors[hidden.Length - 1] = new BackpropagationConnector(hiddenLayers[hidden.Length - 1], outputLayer);

        // The network
        var network = new BackpropagationNetwork(inputLayer, outputLayer);

        return network;
    }

    public void setNetworkWeights(BackpropagationNetwork aNetwork, double[] weights)
    {
        // Setup the network's weights.
        int index = 0;

        foreach (BackpropagationConnector connector in aNetwork.Connectors)
        {
            foreach (BackpropagationSynapse synapse in connector.Synapses)
            {
                synapse.Weight = weights[index++];
                //synapse.SourceNeuron.SetBias(weights[index++]);
                synapse.SourceNeuron.Bias = weights[index++];
            }
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
}