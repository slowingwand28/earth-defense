using Godot;
using System;

public partial class RegionMenu : Control
{
	public enum UpgradeType
	{
		Resources,
		Strength,
		Efficiency
	}

	[Signal] public delegate void UpgradeEventHandler(int upgradeType); // Signal uses int for Variant compatibility; values map to UpgradeType enum
	private Label _regionNameLabel; // Label to display the region name
	private Label _resourceCountLabel; // Label to display the resource count
	private Label _regionStrengthLabel; // Label to display the region strength
	private Label _resourcesCostLabel; // Label to display the resources upgrade cost
	private Label _strengthCostLabel; // Label to display the strength upgrade cost
	private Label _efficiencyCostLabel; // Label to display the efficiency upgrade cost
	private Button _upgradeResourcesButton; // Button to upgrade resources
	private Button _upgradeStrengthButton; // Button to upgrade strength
	private Button _upgradeEfficiencyButton; // Button to upgrade efficiency
	private Button _exitButton; // Button to close the menu

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_regionNameLabel = GetNode<Label>("RegionName");
		_resourceCountLabel = GetNode<Label>("RegionResources");
		_regionStrengthLabel = GetNode<Label>("RegionStrength");
		_resourcesCostLabel = GetNode<Label>("Infrastructure Cost");
		_strengthCostLabel = GetNode<Label>("Military Cost");
		_efficiencyCostLabel = GetNode<Label>("Efficiency Cost");
		_upgradeResourcesButton = GetNode<Button>("Infrastructure Button");
		_upgradeStrengthButton = GetNode<Button>("Military Button");
		_upgradeEfficiencyButton = GetNode<Button>("Efficiency Button");
		_exitButton = GetNode<Button>("Exit");
		_upgradeResourcesButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Resources); // Connect the resources upgrade button to emit an upgrade signal with the type Resources
		_upgradeStrengthButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Strength); // Connect the strength upgrade button to emit an upgrade signal with the type Strength
		_upgradeEfficiencyButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Efficiency); // Connect the efficiency upgrade button to emit an upgrade signal with the type Efficiency
		_exitButton.Pressed += OnExitButtonPressed;
		GD.Print("RegionMenu Ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnExitButtonPressed() // Handler for the exit button press event, could be replaced with a lambda for simplicity
	{
		Visible = false; // Hide the menu when the exit button is pressed
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
