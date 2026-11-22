using Godot;
using System;
using System.ComponentModel;

public partial class WeaponBase : Node3D
{

	[Export] private float fireRate = 0.5f;
	[Export] private int ammo = 100;

	[Export] private PackedScene projectile;
	[Export] private Node3D muzzle;

	Global global;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Shoot()
	{

		if (!IsMultiplayerAuthority())
			return;


		if (Multiplayer.IsServer())
		{
			global.Shoot(
				muzzle.GlobalPosition,
				muzzle.GlobalTransform.Basis.Z,
				projectile
			);
		}
		else
		{
			RpcId(1, nameof(RequestShoot),
				muzzle.GlobalPosition,
				muzzle.GlobalTransform.Basis.Z
			);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestShoot(Vector3 pos, Vector3 direction)
	{
		global.Shoot(pos, direction, projectile);
	}
}
