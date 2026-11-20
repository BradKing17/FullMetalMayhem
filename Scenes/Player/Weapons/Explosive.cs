using Godot;
using System;

public partial class Explosive : RigidBody3D
{
	[Export] private float explosiveRadius = 100.0f; 
	[Export] private Area3D detonateArea;

	[Export] private float damage = 10.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		detonateArea.AreaEntered += Detonate;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Detonate(Node3D body)
	{
		if(body.IsInGroup("Enemies"))
		{
			body.Call("TakeDamage", damage);
		}

		QueueFree();
	}
}
