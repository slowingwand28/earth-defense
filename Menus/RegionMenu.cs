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

	public static int GetUpgradeCost(UpgradeType upgradeType) => upgradeType switch // Single source of truth for upgrade costs
	{
		UpgradeType.Resources => 12,
		UpgradeType.Strength => 14,
		UpgradeType.Efficiency => 22,
		_ => 0
	};
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
		// Get references to all the UI elements in the menu and connect button signals to emit upgrade events
		_regionNameLabel = GetNode<Label>("Panel/Content/RegionName");
		_resourceCountLabel = GetNode<Label>("Panel/Content/StatsRow/RegionResources");
		_regionStrengthLabel = GetNode<Label>("Panel/Content/StatsRow/RegionStrength");
		_resourcesCostLabel = GetNode<Label>("Panel/Content/CostRow/Infrastructure Cost");
		_strengthCostLabel = GetNode<Label>("Panel/Content/CostRow/Military Cost");
		_efficiencyCostLabel = GetNode<Label>("Panel/Content/CostRow/Efficiency Cost");
		_upgradeResourcesButton = GetNode<Button>("Panel/Content/UpgradeButtonRow/Infrastructure Button");
		_upgradeStrengthButton = GetNode<Button>("Panel/Content/UpgradeButtonRow/Military Button");
		_upgradeEfficiencyButton = GetNode<Button>("Panel/Content/UpgradeButtonRow/Efficiency Button");
		_exitButton = GetNode<Button>("Panel/Content/Exit");
		_upgradeResourcesButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Resources); // Connect the resources upgrade button to emit an upgrade signal with the type Resources
		_upgradeStrengthButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Strength); // Connect the strength upgrade button to emit an upgrade signal with the type Strength
		_upgradeEfficiencyButton.Pressed += () => EmitSignal(SignalName.Upgrade, (int)UpgradeType.Efficiency); // Connect the efficiency upgrade button to emit an upgrade signal with the type Efficiency
		_exitButton.Pressed += OnExitButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnExitButtonPressed() // Handler for the exit button press event, could be replaced with a lambda for simplicity
	{
		Visible = false; // Hide the menu when the exit button is pressed
	}

	public void SetMenu(string name, int resources, int defense) // Method to update the menu with the selected region's information
	{
		_regionNameLabel.Text = $"Region: {name}"; // Update the region name label
		_resourceCountLabel.Text = $"Resources: {resources}"; // Update the resource count label
		_regionStrengthLabel.Text = $"Strength: {defense}"; // Update the region strength label

		int resourcesCost = GetUpgradeCost(UpgradeType.Resources);
		int strengthCost = GetUpgradeCost(UpgradeType.Strength);
		int efficiencyCost = GetUpgradeCost(UpgradeType.Efficiency);

		_resourcesCostLabel.Text = $"Cost: {resourcesCost}";
		_strengthCostLabel.Text = $"Cost: {strengthCost}";
		_efficiencyCostLabel.Text = $"Cost: {efficiencyCost}";

		_upgradeResourcesButton.Disabled = resources < resourcesCost;
		_upgradeStrengthButton.Disabled = resources < strengthCost;
		_upgradeEfficiencyButton.Disabled = resources < efficiencyCost;
	}
}
