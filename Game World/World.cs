using Godot;
using System;

public partial class World : Node2D
{
    [Signal] public delegate void TurnPassedEventHandler(); // Signal to notify when the turn has been passed
    private RegionMenu _regionMenu; // Reference to the region menu control, can be used to show and update the menu when a region is clicked
    private RegionControl _selectedRegion; // Currently selected region; upgrades apply only to this instance
    private Node _allRegions; // Node to hold all region instances, can be used for organization and management
    private Button _turnButton; // Button to pass the turn, can be used to trigger end-of-turn events
    private Label _turnLabel; // Label to display the current turn count, can be updated each turn
    private int _turnCount = 1; // Counter to keep track of the number of turns that have passed, can be used for game progression or events

     // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _regionMenu = GetNode<RegionMenu>("RegionMenu"); // Get the region menu control
        _regionMenu.Visible = false; // Hide the region menu initially
        _regionMenu.Upgrade += OnUpgradeRequested;
        _allRegions = GetNode("AllRegions"); // Get the node that holds all regions
        _turnLabel = GetNode<Label>("TurnLabel"); // Get the label to display the turn count
        _turnLabel.Text = $"Turn: {_turnCount}"; // Set the initial turn count text
        _turnButton = GetNode<Button>("TurnButton"); // Get the button to pass the turn
        _turnButton.Pressed += OnTurnButtonPressed; // Connect the button's pressed signal to emit the TurnPassed signal when clicked
        foreach (RegionControl region in _allRegions.GetChildren()) // Connect the RegionClicked signal for each region to the handler
        {
            region.RegionClicked += OnRegionClicked;
        }
        GD.Print("World Ready");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnRegionClicked(RegionControl region) // Handler for when a region is clicked
    {
        _selectedRegion = region;
        _regionMenu.Visible = true; // Show the region menu
        _regionMenu.SetMenu(region.RegionName, region.ResourceStock, region.Strength); // Set the menu information based on the clicked region
        GD.Print($"Region {region.RegionName} clicked, menu opened");
    }

    private void OnUpgradeRequested(int upgradeTypeValue)
    {
        // Ignore upgrade requests if no region is currently selected.
        if (_selectedRegion == null)
        {
            return;
        }

        // Convert the integer payload from the UI signal into the typed enum.
        RegionMenu.UpgradeType upgradeType = (RegionMenu.UpgradeType)upgradeTypeValue;

        // Apply the selected upgrade to the active region's growth/multiplier stats.
        switch (upgradeType)
        {
            case RegionMenu.UpgradeType.Resources:
                // Increase per-turn resource growth.
                _selectedRegion.BaseResourceGrowth += 1;
                break;
            case RegionMenu.UpgradeType.Strength:
                // Increase per-turn strength growth.
                _selectedRegion.BaseStrengthGrowth += 1;
                break;
            case RegionMenu.UpgradeType.Efficiency:
                // Improve both resource and strength scaling multipliers.
                _selectedRegion.ResourceMultiplier += 0.1f;
                _selectedRegion.StrengthMultiplier += 0.1f;
                break;
            default:
                GD.PushWarning($"Unknown upgrade type: {upgradeType}");
                return;
        }

        // Refresh menu values after applying the upgrade.
        _regionMenu.SetMenu(_selectedRegion.RegionName, _selectedRegion.ResourceStock, _selectedRegion.Strength);
        GD.Print($"Applied {upgradeType} upgrade to {_selectedRegion.RegionName}");
    }

    private void OnTurnButtonPressed() // Handler for when the turn button is pressed
    {
        _turnCount++; // Increment the turn count
        _turnLabel.Text = $"Turn: {_turnCount}"; // Update the turn label text
        EmitSignal(SignalName.TurnPassed); // Emit the TurnPassed signal to notify all connected nodes that the turn has passed
        GD.Print($"Turn passed. Current turn: {_turnCount}");
    }
}
