using Godot;
using System;

[GlobalClass]
public partial class Global : Node
{

	public static string userName = "";
	public static  Node3D map;

	public static  Node3D spawnContainer;

	public int points = 0;

    public override void _Ready()
    {
        points = 0;
    }


	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void Shoot(Vector3 pos, Vector3 direction, PackedScene projectile)
	{

		GD.Print("Server spawning projectile!");
		var bullet = projectile.Instantiate<RigidBody3D>();


		spawnContainer.AddChild(bullet, true);
		bullet.GlobalPosition = pos;
		bullet.ApplyCentralImpulse(direction * 100);
	}


	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void SpawnExplosionEffect(Vector3 position, string effectPath)
	{
		var effect = GD.Load<PackedScene>(effectPath);

		if (effect == null)
		{
			GD.PrintErr("Could not load explosion: " + effectPath);
			return;
		}

		var explosion = effect.Instantiate<Node3D>();

		GetTree().CurrentScene.AddChild(explosion);

		explosion.GlobalPosition = position;

		explosion.Connect(
			"finished",
			Callable.From(explosion.QueueFree)
		);

		explosion.Call("play");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Death(Node3D killed)
	{
		if(killed.IsInGroup("Enemies"))
		{
			points++;
			GD.Print(points);
		}
		GD.Print("AAAAAAAAAAAAAAAAAH");
		killed.QueueFree();
	}
}
