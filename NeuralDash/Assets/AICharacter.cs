using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICharacter : MonoBehaviour {

    public int InputLayer;
    public int[] HiddenLayers;
    public int OutputLayer;

    public Brain brain;

    public bool isOperative;


    public void InitializeAI()
    {
        brain = Brain.CreateAIEntity(InputLayer, OutputLayer, HiddenLayers, true);
    }

    public void SetGenes(double[] weights)
    {
        brain.SetNetworkWeights(weights);
    }

    public abstract double[] GetSensorReading();

    public abstract void ProcessBrainOutput(double[] actions);

    // THESE SHOULD NOT BE PART OF THE CHARACTER AI CLASS
    public bool isRunning;
    public abstract int GetScore();

    public abstract void ResetSkipper();

    public void ActivateBrain()
    {
        var input = GetSensorReading();
        var output = brain.ProcessSensorData(input);
        //send input action back to player
        ProcessBrainOutput(output);
    }
}
