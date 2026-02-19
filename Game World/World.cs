using Godot;
using System;

public partial class World : Node2D
{
    [Signal] public delegate void TurnPassedEventHandler(); // Signal to notify when the turn has been passed
    private PackedScene _regionMenuScene; // PackedScene for the region menu, allows us to instance it when needed
    private Node _allRegions; // Node to hold all region instances, can be used for organization and management
    private Button _turnButton; // Button to pass the turn, can be used to trigger end-of-turn events
    private Label _turnLabel; // Label to display the current turn count, can be updated each turn
    private int _turnCount = 1; // Counter to keep track of the number of turns that have passed, can be used for game progression or events

     // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _regionMenuScene = GD.Load<PackedScene>("res://Menus/region_menu.tscn"); // Load the region menu scene for later use
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

    private void OnRegionClicked(string regionName, int resourceCount, int regionStrength) // Handler for when a region is clicked
    {
        var menuInstance = _regionMenuScene.Instantiate<RegionMenu>(); // Create an instance of the region menu
        AddChild(menuInstance); // Add the menu to the scene tree to display it
        menuInstance.Position = GetViewport().GetVisibleRect().Size / 2; // Position the menu in the center of the screen
        menuInstance.SetMenu(regionName, resourceCount, regionStrength); // Set the menu information based on the clicked region
        GD.Print($"Region {regionName} clicked, menu opened");
    }

    private void OnTurnButtonPressed() // Handler for when the turn button is pressed
    {
        _turnCount++; // Increment the turn count
        _turnLabel.Text = $"Turn: {_turnCount}"; // Update the turn label text
        EmitSignal(SignalName.TurnPassed); // Emit the TurnPassed signal to notify all connected nodes that the turn has passed
        GD.Print($"Turn passed. Current turn: {_turnCount}");
    }
}
