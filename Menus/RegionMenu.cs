using Godot;
using System;

public partial class RegionMenu : Control
{
	private Label _regionNameLabel; // Label to display the region name
	private Label _resourceCountLabel; // Label to display the resource count
	private Label _regionStrengthLabel; // Label to display the region strength
	private Button _exitButton; // Button to close the menu

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_regionNameLabel = GetNode<Label>("RegionName");
		_resourceCountLabel = GetNode<Label>("RegionResources");
		_regionStrengthLabel = GetNode<Label>("RegionStrength");
		_exitButton = GetNode<Button>("Exit");
		_exitButton.Pressed += OnExitButtonPressed;
		GD.Print("RegionMenu Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnExitButtonPressed() // Handler for the exit button press event, could be replaced with a lambda for simplicity
	{
		QueueFree(); // Remove the menu from the scene tree, effectively closing it
		GD.Print("RegionMenu closed");
	}

	public void SetMenu(string name, int resources, int defense) // Method to update the menu with the selected region's information
	{
		_regionNameLabel.Text = $"Region: {name}"; // Update the region name label
		_resourceCountLabel.Text = $"Resources: {resources}"; // Update the resource count label
		_regionStrengthLabel.Text = $"Strength: {defense}"; // Update the region strength label
		GD.Print($"RegionMenu updated for {name}");
	}
}
