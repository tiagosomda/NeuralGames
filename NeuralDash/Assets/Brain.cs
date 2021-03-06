﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using System;

public partial class Brain {
    public BackpropagationNetwork neuralNetwork;
    public Genome genome;

    public double[] Genes
    {
        get
        {
            return genome.m_genes;
        }

        set
        {
            genome.m_genes = value;
        }
    }

    public double Fitness
    {
        get
        {
            return genome.Fitness;
        }
        set
        {
            genome.Fitness = value;
        }
    }

    public static Brain CreateAIEntity(int input, int output, int[] hidden, bool createGenes)
    {
        var brain = new Brain();

        var networkWeightCount = brain.CreateNeuralNetwork(input, output, hidden);

        brain.neuralNetwork.Initialize();

        brain.genome = new Genome(networkWeightCount, createGenes);

        brain.SetNetworkWeights(brain.Genes);

        return brain;
    }

    public void SetNetworkWeights(double[] weights)
    {
        // Setup the network's weights.
        int index = 0;

        foreach (BackpropagationConnector connector in neuralNetwork.Connectors)
        {
            foreach (BackpropagationSynapse synapse in connector.Synapses)
            {
                synapse.Weight = weights[index++];
                //synapse.SourceNeuron.SetBias(weights[index++]);
                synapse.SourceNeuron.Bias = weights[index++];
            }
        }
    }

    private int CreateNeuralNetwork(int input, int output, int[] hidden)
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
        neuralNetwork = new BackpropagationNetwork(inputLayer, outputLayer);

        #region retrieve network weight count
        int netWeightCount = 0;

        foreach (BackpropagationConnector connector in neuralNetwork.Connectors)
        {
            foreach (BackpropagationSynapse synapse in connector.Synapses)
            {
                netWeightCount += 2;
            }
        }
        #endregion

        return netWeightCount;
    }

    public double[] ProcessSensorData(double[] input)
    {
        if(input == null)
        {
            return null;
        }

        return neuralNetwork.Run(input);
    }
}
