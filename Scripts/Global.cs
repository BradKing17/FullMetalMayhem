using Godot;
using System;

[GlobalClass]
public partial class Global : Node
{

	public static string userName = "";
	public static  Node3D map;

	public static  Node3D spawnContainer;




[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Shoot(Vector3 pos, Vector3 direction, PackedScene projectile)
	{

		GD.Print("Server spawning projectile!");
		var bullet = projectile.Instantiate<RigidBody3D>();


		spawnContainer.AddChild(bullet, true);
		bullet.GlobalPosition = pos + direction * 2;
		bullet.ApplyCentralImpulse(direction);
	}
}
