using Godot;
using System;

public partial class RegionMenu : Control
{
	private Label _regionNameLabel; // Label to display the region name
	private Label _resourceCountLabel; // Label to display the resource count
	private Label _regionStrengthLabel; // Label to display the region strength
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_regionNameLabel = GetNode<Label>("RegionName");
		_resourceCountLabel = GetNode<Label>("ResourceCount");
		_regionStrengthLabel = GetNode<Label>("RegionStrength");
		GD.Print("RegionMenu Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
