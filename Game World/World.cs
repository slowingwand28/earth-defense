using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
    public enum DifficultyPreset
    {
        Easy,
        Balanced,
        Hard,
        Custom
    }

    [Export] public DifficultyPreset _difficultyPreset = DifficultyPreset.Balanced; // One-click preset selection in inspector
    [Export] public int _attackStrength = 26; // Base strength of each attack wave
    [Export] public int _attackIncreasePerWave = 3; // Increases pressure each attack so late game is harder
    [Export] public int _attackFrequency = 3; // Attack every N turns
    [Export] public int _winTurn = 46; // Survive until this turn to win
    [Export] public int _attackDamageFloorBonus = 1; // Extra flat damage applied per attacked region
    [Export] public AudioStream _attackSfx; // Played whenever an attack wave occurs
    [Export] public AudioStream _upgradeSfx; // Played when an upgrade is successfully purchased
    [Signal] public delegate void TurnPassedEventHandler(); // Signal to notify when the turn has been passed
    [Signal] public delegate void GameOverEventHandler(EndState endState); // Signal to notify when the game is over, can be used to trigger endgame events or screens
    [Signal] public delegate void PauseGameEventHandler(); // Signal to notify when the game is paused, can be used to trigger pause menu or effects
    private RegionMenu _regionMenu; // Reference to the region menu control, can be used to show and update the menu when a region is clicked
    private RegionControl _selectedRegion; // Currently selected region; upgrades apply only to this instance
    private Node _allRegions; // Node to hold all region instances, can be used for organization and management
    private Button _turnButton; // Button to pass the turn, can be used to trigger end-of-turn events
    private Button _pauseButton; // Button to pause the game, can be used to trigger the pause signal and show the pause menu
    private Label _turnLabel; // Label to display the current turn count, can be updated each turn
    private AudioStreamPlayer _sfxPlayer; // Shared one-shot player for world-level SFX
    private Panel _frame; // UI panel to tween for visual representation of attacks
    private int _turnCount = 1; // Counter to keep track of the number of turns that have passed, can be used for game progression or events
    public enum EndState
    {
        Defeat,
        Victory
    }

     // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ApplyDifficultyPreset();
        _regionMenu = GetNode<RegionMenu>("RegionMenu"); // Get the region menu control
        _regionMenu.Visible = false; // Hide the region menu initially
        _regionMenu.Upgrade += OnUpgradeRequested;
        _allRegions = GetNode("AllRegions"); // Get the node that holds all regions
        _turnLabel = GetNode<Label>("TurnLabel"); // Get the label to display the turn count
        _turnLabel.Text = $"Turn: {_turnCount}"; // Set the initial turn count text
        _frame = GetNode<Panel>("Frame"); // Get the panel to use for attack visual effects
        _sfxPlayer = new AudioStreamPlayer{Name = "SfxPlayer"}; // Create a new AudioStreamPlayer for playing sound effects
        AddChild(_sfxPlayer);
        _turnButton = GetNode<Button>("TurnButton"); // Get the button to pass the turn
        _pauseButton = GetNode<Button>("PauseButton"); // Get the button to pause the game
        _turnButton.Pressed += OnTurnButtonPressed; // Connect the button's pressed signal to emit the TurnPassed signal when clicked
        _pauseButton.Pressed += OnPauseButtonPressed; // Connect the button's pressed signal to emit the PauseGame signal when clicked
        foreach (RegionControl region in _allRegions.GetChildren()) // Connect the RegionClicked signal for each region to the handler
        {
            region.RegionClicked += OnRegionClicked;
        }
        GD.Print("World initialized.");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnRegionClicked(RegionControl region) // Handler for when a region is clicked
    {
        _selectedRegion = region;
        RefreshSelectedRegionMenu(); // Open and populate the region menu
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
        int upgradeCost = _selectedRegion.GetUpgradeCost(upgradeType);

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
                _selectedRegion.ResourceUpgradeLevel += 1;
                break;
            case RegionMenu.UpgradeType.Strength:
                // Increase per-turn strength growth.
                _selectedRegion.BaseStrengthGrowth += 1;
                _selectedRegion.StrengthUpgradeLevel += 1;
                break;
            case RegionMenu.UpgradeType.Efficiency:
                // Improve both resource and strength scaling multipliers.
                _selectedRegion.ResourceMultiplier += 0.10f;
                _selectedRegion.StrengthMultiplier += 0.10f;
                _selectedRegion.EfficiencyUpgradeLevel += 1;
                break;
            default:
                GD.PushWarning($"Unknown upgrade type: {upgradeType}");
                return;
        }

        // Refresh menu values after applying the upgrade.
        RefreshSelectedRegionMenu();
        PlaySfx(_upgradeSfx);
    }

    private void OnTurnButtonPressed() // Handler for when the turn button is pressed
    {
        _turnCount++; // Increment the turn count
        _turnLabel.Text = $"Turn: {_turnCount}"; // Update the turn label text
        EmitSignal(SignalName.TurnPassed); // Emit the TurnPassed signal to notify all connected nodes that the turn has passed

        // Keep the open menu in sync with the selected region's latest turn values.
        if (_selectedRegion != null)
        {
            RefreshSelectedRegionMenu();
        }

        if (_turnCount % _attackFrequency == 0) // Attacks every configured number of turns
        {
            PlaySfx(_attackSfx);
            attack();
            if (ActiveRegions().Count == 0) // Check if all regions have been conquered after the attack
            {
                EmitSignal(SignalName.GameOver, (int)EndState.Defeat); // Emit game over signal with defeat state
                _turnButton.Disabled = true; // Disable the turn button to prevent further turns after game over
                GD.Print("Game over: defeat.");
                return;
            }

            _attackStrength += _attackIncreasePerWave;
        }
        if (_turnCount == _winTurn) // Check for a win condition at the configured turn
        {
            EmitSignal(SignalName.GameOver, (int)EndState.Victory); // Emit game over signal with victory state
            _turnButton.Disabled = true; // Disable the turn button to prevent further turns after game over
            GD.Print("Game over: victory.");
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

        // Keep pressure high even with many surviving regions by applying a per-target floor.
        var attackDamage = Mathf.Max(1, Mathf.RoundToInt((float)_attackStrength / targets.Count) + _attackDamageFloorBonus);
        foreach (RegionControl region in targets) // Loop through target regions and apply damage based on the attack strength
        {
            if (region.Strength <= attackDamage) // If the region's strength is less than or equal to the attack damage, it is conquered
            {
                region.Conquered(); // Mark the region as conquered
            } 
            else // Otherwise, reduce the region's strength by the attack damage
            {
                region.Strength -= attackDamage; // Reduce the region's strength by the attack damage
            }
        }
        attackVisual(); // Trigger the attack visual effects
    }

    private void RefreshSelectedRegionMenu()
    {
        if (_selectedRegion == null)
        {
            return;
        }

        _regionMenu.OpenMenu(
            _selectedRegion.RegionName,
            _selectedRegion.ResourceStock,
            _selectedRegion.Strength,
            _selectedRegion.GetUpgradeCost(RegionMenu.UpgradeType.Resources),
            _selectedRegion.GetUpgradeCost(RegionMenu.UpgradeType.Strength),
            _selectedRegion.GetUpgradeCost(RegionMenu.UpgradeType.Efficiency));
    }

    private void attackVisual() // Implementation for attack visual effects
    {
        var tween = GetTree().CreateTween(); // Create a new tween for animating the attack effect
        tween.TweenProperty(_frame, "modulate:a", 1, 2);
        tween.TweenProperty(_frame, "modulate:a", 0, 2).SetDelay(1); // Fade the frame in and out to visually represent the attack wave
    }

    private void OnPauseButtonPressed() // Handler for when the pause button is pressed
    {
        EmitSignal(SignalName.PauseGame); // Emit the pause signal to notify UI controller
    }

    private void PlaySfx(AudioStream stream)
    {
        if (stream == null || _sfxPlayer == null)
        {
            return;
        }

        _sfxPlayer.Stream = stream;
        _sfxPlayer.Play();
    }

    private void ApplyDifficultyPreset()
    {
        switch (_difficultyPreset)
        {
            case DifficultyPreset.Easy:
                _attackStrength = 22;
                _attackIncreasePerWave = 2;
                _attackFrequency = 4;
                _winTurn = 42;
                _attackDamageFloorBonus = 0;
                break;
            case DifficultyPreset.Balanced:
                _attackStrength = 26;
                _attackIncreasePerWave = 3;
                _attackFrequency = 3;
                _winTurn = 46;
                _attackDamageFloorBonus = 1;
                break;
            case DifficultyPreset.Hard:
                _attackStrength = 30;
                _attackIncreasePerWave = 4;
                _attackFrequency = 3;
                _winTurn = 50;
                _attackDamageFloorBonus = 2;
                break;
            case DifficultyPreset.Custom:
            default:
                // Leave exported values unchanged for manual tuning.
                break;
        }
    }
}
