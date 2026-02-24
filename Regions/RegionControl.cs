using Godot;
using System;

public partial class RegionControl : Control
{
    // Properties of the region, exposed to the editor for easy tweaking
    [Export] public string RegionName { get; set; } = "New Region"; // Name of the region. Defaults to "New Region"
    [Export] public Color RegionColor { get; set; } = new Color(1, 1, 1); // Color of the region. Defaults to white
    [Export] public int ResourceStock { get; set; } = 20; // Number of resources in the region, can be spent on upgrades or events
    [Export] public int Strength { get; set; } = 10; // Strength of the region, can be used for battles or events
    [Export] public int BaseResourceGrowth { get; set; } = 2; // Amount of resources that grow each turn, can increase over time with upgrades
    [Export] public int BaseStrengthGrowth { get; set; } = 1; // Amount of strength that grows each turn, can increase over time with upgrades
    [Export] public float ResourceMultiplier { get; set; } = 1.0f; // Multiplier for resource growth, can be set per region and affected by events
    [Export] public float StrengthMultiplier { get; set; } = 1.0f; // Multiplier for strength growth, can be set per region and affected by events
    [Signal] public delegate void RegionClickedEventHandler(string regionName, int resourceCount, int regionStrength); // Signal to notify when this region is clicked

    private TextureRect _color; // Reference to the TextureRect node for changing color
    private Node2D _world; // Reference to the world node, can be used for interactions with the world
        
    public override void _Ready() // Called when the node enters the scene tree for the first time.
    {
        _world = GetTree().Root.GetNode<Node2D>("World"); // Get a reference to the world node, assuming it's at the root of the scene tree
        _color = GetNode<TextureRect>("TextureRect");
        _color.Modulate = RegionColor;
        GuiInput += OnGuiInput; // Connect the input event to the handler
        _world.Connect("TurnPassed", new Callable(this, nameof(OnTurnPassed))); // Connect the TurnPassed signal from the world to the handler
        GD.Print($"{RegionName} Ready");
    }

    public override void _Process(double delta) // Called every frame. 'delta' is the elapsed time since the previous frame.
    {
    }

    private void OnGuiInput(InputEvent @event) // Handler for GUI input events, such as mouse clicks
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed && 
            mouseEvent.ButtonIndex == MouseButton.Left) // Check if the left mouse button was pressed
        {
            EmitSignal(SignalName.RegionClicked, RegionName, ResourceStock, Strength); // Emit a signal to notify that this region was clicked
            GD.Print($"{RegionName} clicked!");
        }
    }

    private void OnTurnPassed() // Handler for when the turn is passed, can be used to update resources or strength based on multipliers
    {
        ResourceStock += Mathf.RoundToInt(BaseResourceGrowth * ResourceMultiplier); // Update resource count based on the multiplier
        Strength += Mathf.RoundToInt(BaseStrengthGrowth * StrengthMultiplier); // Update region strength based on the multiplier
        GD.Print($"{RegionName} updated for new turn: Resources={ResourceStock}, Strength={Strength}");
    }
}
