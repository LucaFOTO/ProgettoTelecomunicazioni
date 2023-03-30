using System;

public class NeuralNetwork
{
    /// <summary>
    /// A neural network; in the network the layers are organized in an array ranging from 0 ( input ) to n ( output ), in  between there is a number of hidden layers that each represents a part of a final decision
    /// </summary>
    /// <value>Property <c>Layer[] : layers</c> represents the layers in the neural network.</value>

    Layer[] layers;
    NeuralNetwork(int numberOfHiddenLayers, int[] numberOfNodesPerLayer)
    {
        /// <summary>
        /// A neural network; in the network the layers are organized in an array ranging from <c>0</c> ( input ) to <c>Array.length - 1</c> ( output ), in  between there is a number of hidden layers that each represents a part of a final decision
        /// </summary>
        /// <param name="int : numberOfHiddenLayers">Number of hidden layers (layers between the input and the output layer )</param>
        /// <param name="int[NumberOfHiddenLayers + 2] : numberOfNodesPerLayer">An integer array containing the number of nodes of every layer, including the input (<c>index 0</c>) and the output layer (<c>index Array.length - 1</c>)</param>
        /// <exception cref="LayerLengthMismatch">
        /// Thrown when the number of layers in the network does not match the length of the array containing the number of nodes per layer.
        /// </exception>

        if ((numberOfHiddenLayers + 2) != numberOfNodesPerLayer.Length)
        {
            throw new LayerLengthMismatch("The number of layers in the network does not match the length of the array containing the number of nodes per layer");
        }

        layers = new Layer [(2 + numberOfHiddenLayers)];
        
        layers[0] = new Layer(numberOfNodesPerLayer[0], 0);
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i] = new Layer(numberOfNodesPerLayer[i], numberOfNodesPerLayer[i - 1]);
        }
    }
    public void randomWeights(int a)
    {
        /// <summary>
        /// Randomize weights of every node excluding the input ones
        /// </summary>
        /// <param name="int : numberOfNodes">Number of nodes in this layer</param>
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].randomWeights();
        }
    }
    public void calculate()
    {
        /// <summary>
        /// Calculates the values of all the nodes in the network and finds the output ( values of output nodes ).
        /// </summary>

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].calculate(layers[i - 1]);
        }
    }
    public void loadInput(double[] values)
    {
        /// <summary>
        /// Loads values in the input layer using an array of doubles.
        /// </summary>
        /// <exception cref="LayerLengthMismatch">
        /// Thrown when the array lengths of the values and the nodes in the layer do not correspond. (Actually the esception is thrown by the <c>Layer.setNodesValues</c> method)
        /// </exception>
        /// <param name="double[] : values">Array of doubles containing the values to be loaded in the input layer</param>
        
        layers[0].setNodesValues(values);
    }
    public double[] getOutput(){
        /// <summary>
        /// Gets the values of the output layer
        /// </summary>
        /// <returns>double[] : Array of doubles containing the values of the output layer</returns>
        return layers[layers.Length - 1].getNodesValues(); 
    }
}

class Layer
{
    /// <summary>
    /// Group of nodes that represents a series of decisions
    /// </summary>
    /// <value>Property <c>Node[] : nodes</c> represents the nodes in the layer.</value>

    Node[] nodes;
    public Layer(int numberOfNodes, int numberOfNodesInPreviousLayer)
    {
        /// <summary>
        /// Group of nodes that represents a series of decisions
        /// </summary>
        /// <param name="int : numberOfNodes">Number of nodes in this layer</param>
        /// <param name="int : numberOfNodesInPreviousLayer">Number of nodes in the previous layer</param>
        
        nodes = new Node[numberOfNodes];
        
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new Node(numberOfNodesInPreviousLayer);
        }
    }
    public void randomWeights()
    {
        /// <summary>
        /// Randomizes the weights of every node in the layer
        /// </summary>
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].randomWeights();
        }
    }
    public void calculate(Layer previousLayer)
    {
        /// <summary>
        /// !!!IMPORTANT!!!
        /// This function shouldnt be used on the input layer as it has nothing to calculate. 
        ///
        /// Calculates the values of the nodes in the layer using the values of the previous layer.
        ///
        /// </summary>
        /// <property name="Layer : previousLayer">Layer containing the values of the previous layer</property>

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].calculate(previousLayer.getNodesValues());
        }
    }
    public Node[] getNodes()
    {
        /// <summary>
        /// Gets the nodes in the layer
        /// </summary>
        /// <returns>Node[] : Array of nodes in the layer</returns>
        return nodes;
    }
    public double[] getNodesValues()
    {
        /// <summary>
        /// Gets the values of the nodes in the layer
        /// </summary>
        /// <returns>double[] : Array of doubles containing the values of the nodes in the layer</returns>
        
        double[] values = new double[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            values[i] = nodes[i].getValue();
        }
        return values;
    }
    public void setNodes(Node[] nodes)
    {
        /// <summary>
        /// Sets the nodes in the layer
        /// !(Mostly usless)!
        /// </summary>
        /// <param name="Node[] : nodes">Array of nodes in the layer</param>
        this.nodes = nodes;
    }
    public void setNodesValues(double[] values)
    {
        /// <summary>
        /// Sets the values of the nodes in the layer
        /// </summary>
        /// <param name="double[] : values">Array of doubles containing the values to be loaded in the nodes</param>
        /// <exception cref="LayerLengthMismatch">
        /// Thrown when the array lengths of the values and the nodes in the layer do not correspond.
        /// </exception>
        
        if (nodes.Length != values.Length)
        {
            throw new LayerLengthMismatch("The length of the input values array is different from the number of nodes in the input layer");
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].setValue(values[i]);
        }
    }
}

class Node
{
    /// <summary>
    /// Single node of the neural network
    /// </summary>
    /// <value>Property <c>double : value</c> represents how much he thinks the decision represented by this node is right.</value>
    /// <value>Property <c>double : bias</c> represents a base value the node value has ( value = ValuePrevNode_n * WeightPrevNode_n + bias ).</value>
    /// <value>Property <c>double[] : connectedWeights</c> represents a list of every weight of every connection of the previous layer nodes.</value>

    double bias;
    double value;
    double[] connectedWeights;
    public Node(int NumberOfNodesPreviousLayer)
    {
        /// <summary>
        /// Single node of the neural network
        /// </summary>
        /// <param name="int : NumberOfNodesPreviousLayer">Number of nodes in the previous layer</param>

        connectedWeights = new double[NumberOfNodesPreviousLayer];

        for (int i = 0; i < connectedWeights.Length; i++)
        {
            connectedWeights[i] = 0;
        }
        bias = 0;
        value = 0;
    }
    public void randomWeights()
    {
        /// <summary>
        /// Randomizes the weights of the node.
        /// </summary>
        
        Random random = new Random();
        for (int i = 0; i < connectedWeights.Length; i++)
        {
            connectedWeights[i] = random.NextDouble();
        }
    }
    public void calculate(double[] previousLayerValues)
    {
        /// <summary>
        /// !!!IMPORTANT!!!
        /// This function shouldnt be used on the nodes of the input layer as it has nothing to calculate. 
        ///
        /// Calculate and sets the value of this node trough the values of the previous nodes, their weights and the bias.
        ///
        /// </summary>
        /// <param name="double[] : previousLayerValues">Array containing previous layer's nodes values</param>

        double tempValue = 0f;

        for (int i = 0; i < previousLayerValues.Length; i++)
        {
            tempValue += previousLayerValues[i] * connectedWeights[i];
        }

        this.value = sigmoid(tempValue + bias);
    }
    public void setBias(double bias)
    {
        /// <summary>
        /// Sets the bias of the node.
        /// </summary>
        /// <param name="double : bias">Bias to be set</param>

        this.bias = bias;
    }
    public double getBias()
    {
        /// <summary>
        /// Gets the bias of the node.
        /// </summary>
        /// <returns>double : bias of this node</returns>

        return this.bias;
    }
    public double modBias(double biasVariation)
    {
        /// <summary>
        /// Gets the bias of the node.
        /// </summary>
        /// <param name="double : biasVariation">Bias variation to be added</param>

        this.bias += biasVariation;
    }
    public void setValue(double value)
    {
        /// <summary>
        /// Sets the value of the node.
        /// </summary>
        /// <param name="double : value">Value to be set</param>

        this.value = value;
    }
    public double getValue()
    {
        /// <summary>
        /// Gets the value of the node.
        /// </summary>
        /// <returns>double : value of this node</returns>

        return this.value;
    }
    private double sigmoid(double value)
    {
        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="double : value">Value to be processed</param>
        /// <returns>float : processed value</returns>

        value = Math.Exp(value);
        return value / (1.0f + value);
    }
}

class LayerLengthMismatch : Exception
{
    /// <summary>
    /// Exception thrown when the length of two arrays regarding the layers do not match.
    /// </summary>
    public LayerLengthMismatch(string message)
    {
    }
}