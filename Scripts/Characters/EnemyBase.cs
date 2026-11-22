using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class EnemyBase : CharacterBody3D
{

	[Export] private float health = 100.0f;

	[Export] private Player target;

	[Export] private float speed = 10.0f;

	Global global;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float targetDist = 10000;
		Array<Node> players = GetTree().GetNodesInGroup("Players");
		foreach (Player player in players)
		{		
			var newDist = (player.GlobalPosition - GlobalPosition).Length();
			if(newDist < targetDist)
			{
				targetDist = newDist;
				target = player;
			}
		}

		Vector3 direction = GlobalPosition.DirectionTo(target.GlobalPosition);
			Velocity = direction * speed;

		MoveAndSlide();

	}
	public void TakeDamage(float damage)
	{
		if (!Multiplayer.IsServer())
			return;


		health -= damage;

		if (health <= 0)
		{
			Die();
		}
	}

	public void Die()
	{
		if (!Multiplayer.IsServer())
			return;

		global.points++;

		global.Rpc(nameof(global.UpdatePoints), global.points);

		QueueFree();
	}

}
