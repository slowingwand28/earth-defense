using Godot;
using System;

public partial class World : Node2D
{
    private PackedScene _regionMenuScene; // PackedScene for the region menu, allows us to instance it when needed
    private Node _allRegions; // Node to hold all region instances, can be used for organization and management

     // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _regionMenuScene = GD.Load<PackedScene>("res://Menus/region_menu.tscn"); // Load the region menu scene for later use
        _allRegions = GetNode("AllRegions"); // Get the node that holds all regions
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
        var menuInstance = _regionMenuScene.Instantiate<RegionMenu>(); // Create an instance of the region menu
        AddChild(menuInstance); // Add the menu to the scene tree to display it
        menuInstance.Position = GetViewport().GetVisibleRect().Size / 2; // Position the menu in the center of the screen
        menuInstance.SetMenu(region.RegionName, region.ResourceCount, region.RegionStrength); // Set the menu information based on the clicked region
        GD.Print($"Region {region.RegionName} clicked, menu opened");
    }
}
