using Godot;
using System;

public partial class Region : Node2D
{
    [Export] public string RegionName { get; set; } = "New Region";
    [Export] public Color RegionColor { get; set; } = new Color(1, 1, 1); // Default to white
    private ColorRect _color;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _color = GetNode<ColorRect>("ColorRect");
            _color.Color = RegionColor;
            GD.Print($"{RegionName} Ready");
        }
    
        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
}
