using Godot;
using System;

public partial class Explosive : RigidBody3D
{
	[Export] private float explosiveRadius = 100.0f; 
	[Export] private Area3D detonateArea;

	[Export] private float damage = 10.0f;

	[Export] private PackedScene explosionEffect;

	[Export] private MeshInstance3D mesh;

	Global global;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		detonateArea.BodyEntered += Detonate;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	
	public void Detonate(Node3D body)
	{
	if (!Multiplayer.IsServer())
        return;

    if (body.IsInGroup("Enemies"))
    {
        if (body is EnemyBase enemy)
        {
            enemy.TakeDamage(damage);
        }
    }

    Vector3 explosionPosition = GlobalPosition;

    global.Rpc(
        nameof(global.SpawnExplosionEffect),
        explosionPosition,
        explosionEffect.ResourcePath
    );

    QueueFree();
}
}
