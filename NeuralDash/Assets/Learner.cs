using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using UnityEngine.UI;
using System.IO;

public class Learner : MonoBehaviour
{
    public GameManager gm;
    public bool createGenomesFromExisting;
    public int simulationCount;
    public float crossOverProbability;
    public float mutationProbability;
    public float characterSpacing;

    public GameObject skipperPrefab;
    public Character[] skipperArray;


    public GameObject DataItemPrefab;
    public GameObject DataItemParent;
    public static GameObject _DataItemPrefab;
    public static GameObject _DataItemParent;

    public static List<string> data_names;
    public static List<Text> data_values;

    private BackpropagationNetwork[] skipperBrain;
    private Genome[] skipperGenome;
    private BackpropagationNetwork network;

    private GeneticAlgorithm ga;

    private double prevMaxFitness = -1;

    public bool isLearning = false;
    public bool noneRunning = false;

    private bool isSimulationRunning;

    int currentIteration = 0;
    public Genome genome;

    int nextIteration = 0;

    public void Start()
    {
        isLearning = false;

        var input = 3;  // speed, distance, size
        var output = 1; // jump,crouch,normal (as double value)
        var hidden = new int[] { 4, 4 }; // 2 hidden layers with for inputs each

        skipperBrain = new BackpropagationNetwork[simulationCount];
        skipperArray = new Character[simulationCount];
        skipperGenome = new Genome[simulationCount];

        for (int i = 0; i < simulationCount; i++)
        {
            skipperBrain[i] = CreateNetwork(input, output, hidden);
            skipperBrain[i].Initialize();

            var skipperGameObject = Instantiate(skipperPrefab);
            skipperGameObject.transform.SetParent(gameObject.transform);

            var pos = skipperGameObject.transform.position;
            pos.x -= characterSpacing * i;
            skipperGameObject.transform.position = pos;

            skipperGameObject.name = "[" + i + "]";
            skipperArray[i] = skipperGameObject.GetComponent<Character>();
        }

        ga = new GeneticAlgorithm(crossOverProbability, mutationProbability, simulationCount, 2000 * 50, 64);
        ga.Elitism = true;
        //ga.FitnessFunction = new GAFunction(PlayGame);
        //ga.Go();
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetCurrentGen(0);
            SetBestScore(0, 0, "---");
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

            // run on neural network
            float output = Mathf.Abs((float)skipperBrain[i].Run(input)[0]);

            //send input action back to player
            skipperArray[i].HandleNeuronOutput(output);
        }
    }

    public void StartLearning()
    {
        Debug.Log("Starting learning process...");
        //ga.Go();
        ga.CreateFitnessTable();

        CreateGenomes();

        currentIteration = 0;
        NewGeneration();

        foreach (var skipper in skipperArray)
        {
            skipper.ResetSkipper();
        }
    }

    public void CreateGenomes()
    {
        ga.CreateGenomes();
        if (createGenomesFromExisting)
        {
            CreateGenomeWithExperience();
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
        ga.SetGenomeAtIndex(0, genome);
        for (int i = 1; i < simulationCount; i++)
        {
            var newGenome = genome.DeepCopy();

            newGenome.Mutate();
            ga.SetGenomeAtIndex(i, newGenome);
        }
    }

    public void NewGeneration()
    {
        SetCurrentGen(currentIteration);

        if (currentIteration > 0)
        {
            var totalFfitness = ga.SortInFitnessOrder();
            ga.AddTotalFitness(totalFfitness);
            ga.CreateNextGeneration();
            ga.SetTotalFitness(0);
            var maxIterations = ga.GetMaxIterations();

            if (currentIteration >= maxIterations)
            {
                return;
            }
        }

        for (int i = 0; i < simulationCount; i++)
        {
            skipperGenome[i] = ga.GetGenomeAtIndex(i);
            setNetworkWeights(skipperBrain[i], skipperGenome[i].Genes());
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
            var score = skipperArray[i].GetScore();
            skipperGenome[i].Fitness = score + 1;
            ga.AddTotalFitness(skipperGenome[i].Fitness);

            if (score > maxScore)
            {
                name = i;
                maxScore = (int)score;
            }
        }

        if (maxScore > prevMaxFitness)
        {
            SaveGenome(string.Format("iter{0}.gen{1}", currentIteration, name), maxScore, skipperGenome[name]);
            SetBestScore(currentIteration, name, maxScore.ToString());
            prevMaxFitness = maxScore;
        }

        currentIteration++;


        foreach (var skipper in skipperArray)
        {
            skipper.ResetSkipper();
        }
    }

    public void SetBestScore(int gen, int name, string score)
    {
        CharData.PanelRight("BEST SCORE : ", score, Color.black);
    }

    public void SetCurrentGen(int iteration)
    {
        CharData.PanelRight("CURRENT GEN : ", iteration.ToString(), Color.black);
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
                synapse.SourceNeuron.SetBias(weights[index++]);
            }
        }
    }

    public static void SetDataItem(string name, string value)
    {
        SetDataItem(name, value, Color.black);
    }

    public static void SetDataItem(string name, string value, Color color)
    {
        CharData.PanelRight(name, value, color);
    }

    public static void SaveGenome(string name, int score, Genome genome)
    {
        string SaveFolder = @"E:\Dev\Projects\NeuralGames\NeuralDash\Assets\_game\savedGenomes\";

        string filePath = Path.Combine(SaveFolder, "score." + (score - 1) + ".{0}.genome.dna");

        var files = Directory.GetFiles(SaveFolder, string.Format(filePath, "*"));

        if (files.Length > 0)
        {
            foreach (var f in files)
            {
                File.Delete(f);
            }
        }

        File.WriteAllText(string.Format(filePath, name), SerializeGenome(genome));
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
}