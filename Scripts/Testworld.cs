using Godot;
using System;

public partial class Testworld : Node3D
{
	[Export]public Node3D spawnContainer;
	[Export] public Godot.Collections.Array<PackedScene> enemies;

	[Export] public Timer spawnTimer;

	Global global;


	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");

		Global.map = this;
		Global.enemyScenes = enemies;
		Global.spawnContainer = spawnContainer;

		spawnTimer.Timeout += () => global.SpawnEnemy();
	}


	public override void _Process(double delta)
	{
		
	}


}
