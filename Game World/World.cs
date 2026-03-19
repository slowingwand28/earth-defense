using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
    [Export] public int _attackStrength = 24; // Base strength of each attack wave
    [Export] public int _attackIncreasePerWave = 2; // Increases pressure each attack so late game is harder
    [Export] public int _attackFrequency = 4; // Attack every N turns
    [Export] public int _winTurn = 45; // Survive until this turn to win
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
        int upgradeCost = RegionMenu.GetUpgradeCost(upgradeType);

        // Prevent upgrades when resources are insufficient.
        if (_selectedRegion.ResourceStock < upgradeCost)
        {
            GD.Print($"{_selectedRegion.RegionName} cannot afford {upgradeType} upgrade.");
            _regionMenu.SetMenu(_selectedRegion.RegionName, _selectedRegion.ResourceStock, _selectedRegion.Strength);
            return;
        }

        // Apply the selected upgrade to the active region's growth/multiplier stats.
        _selectedRegion.ResourceStock -= upgradeCost; // Deduct the cost before applying the upgrade

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
                _selectedRegion.ResourceMultiplier += 0.15f;
                _selectedRegion.StrengthMultiplier += 0.15f;
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

        // Keep the open menu in sync with the selected region's latest turn values.
        if (_selectedRegion != null)
        {
            _regionMenu.SetMenu(_selectedRegion.RegionName, _selectedRegion.ResourceStock, _selectedRegion.Strength);
        }

        GD.Print($"Turn passed. Current turn: {_turnCount}");

        if (_turnCount % _attackFrequency == 0) // Attacks every configured number of turns
        {
            GD.Print("An attack event has occurred!");
            attack();
            if (ActiveRegions().Count == 0) // Check if all regions have been conquered after the attack
            {
                GD.Print("All regions have been conquered! Game Over.");
                _turnButton.Disabled = true; // Disable the turn button to end the game
                return;
            }

            _attackStrength += _attackIncreasePerWave;
        }
        if (_turnCount == _winTurn) // Check for a win condition at the configured turn
        {
            GD.Print($"Congratulations! You have survived {_winTurn} turns and won the game!");
            _turnButton.Disabled = true; // Disable the turn button to end the game
        }
    }

    private List<RegionControl> ActiveRegions() // Helper method to get a list of all active regions, can be used for events that target only active regions
    {
        var activeRegions = new List<RegionControl>();
        foreach (RegionControl region in _allRegions.GetChildren())
        {
            if (region.Active)
            {
                activeRegions.Add(region);
            }
        }
        return activeRegions;
    }

    private void attack()
    {
        var targets = ActiveRegions(); // Get only the active regions
        if (targets.Count == 0)
        {
            return;
        }

        var attackDamage = _attackStrength / targets.Count; // Distribute attack strength evenly among active regions
        foreach (RegionControl region in targets) // Loop through target regions and apply damage based on the attack strength
        {
            if (region.Strength <= attackDamage) // If the region's strength is less than or equal to the attack damage, it is conquered
            {
                region.Conquered(); // Mark the region as conquered
            } 
            else // Otherwise, reduce the region's strength by the attack damage
            {
                region.Strength -= attackDamage; // Reduce the region's strength by the attack damage
                GD.Print($"{region.RegionName} was attacked! New Strength: {region.Strength}");
            }
        }
    }
}
