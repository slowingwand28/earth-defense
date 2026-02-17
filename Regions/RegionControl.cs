using Godot;
using System;

public partial class RegionControl : Control
{
    // Properties of the region, exposed to the editor for easy tweaking
    [Export] public string RegionName { get; set; } = "New Region"; // Name of the region. Defaults to "New Region"
    [Export] public Color RegionColor { get; set; } = new Color(1, 1, 1); // Color of the region. Defaults to white
    [Export] public int ResourceCount { get; set; } = 100; // Number of resources in the region, can be used for upgrades or events
    [Export] public int RegionStrength { get; set; } = 100; // Strength of the region, can be used for battles or events
    [Export] public double ResourceMultiplier { get; set; } = 1.0; // Multiplier for resource gathering, can be affected by events or upgrades
    [Export] public double StrengthMultiplier { get; set; } = 1.0; // Multiplier for region strength, can be affected by events or upgrades

    private TextureRect _color; // Reference to the TextureRect node for changing color
        
        public override void _Ready() // Called when the node enters the scene tree for the first time.
        {
            _color = GetNode<TextureRect>("TextureRect");
            _color.Modulate = RegionColor;
            GuiInput += OnGuiInput; // Connect the input event to the handler
            GD.Print($"{RegionName} Ready");
        }
    
        public override void _Process(double delta) // Called every frame. 'delta' is the elapsed time since the previous frame.
        {
        }

        private void OnGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                GD.Print($"{RegionName} clicked!");
            }
        }
}
