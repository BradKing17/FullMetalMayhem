using Godot;
using System;

public partial class Testworld : Node3D
{
	[Export]public Node3D spawnContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.map = this;
		Global.spawnContainer = spawnContainer;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
