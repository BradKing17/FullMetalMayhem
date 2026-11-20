using Godot;
using System;

public partial class Player : CharacterBody3D
{


	[Export] private Camera3D playerCamera;

	[Export] private Node3D playerMesh;

	[Export] private float speed = 5.0f;
	[Export] private float rotationSpeed = 5.0f;

	bool immobilized = false;

	[Export] private Label3D namePlate;
	[Export] private Label sessionIdLabel;
	[Export] private Control menu;
	[Export] private Button leaveGameButton;

	[Export] private PackedScene projectile;

	[Export] private WeaponBase primaryWeapon;
	[Export] private WeaponBase secondaryWeapon;
	[Export] private float force = 50.0f;

	Networking networking;
	Global global;

	
	public override void _EnterTree()
	{
		SetMultiplayerAuthority((int)int.Parse(Name));
	}
	public override void _Ready()
	{
		menu.Hide();
		global = GetNode<Global>("/root/Global");
		networking = GetNode<Networking>("/root/Networking");
		GD.Print(Name);
		namePlate.Text = "Player " + Name;
		sessionIdLabel.Text = (string)networking.tubeClient.Get("session_id");
		GD.Print((string)networking.tubeClient.Get("session_id"));


		leaveGameButton.Pressed += () => networking.LeaveServer();

		if(IsMultiplayerAuthority())
		{
			playerCamera.Current = true;
		}
		else
		{
			SetProcess(false);
			SetPhysicsProcess(false);
		}
		
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(Input.IsActionJustPressed("pause"))
		{
			OpenMenu(menu.Visible);
		}
		if(Input.IsActionJustPressed("fire"))
		{
			primaryWeapon.Shoot();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!immobilized){
			Vector3 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * (float)delta;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * speed;
				velocity.Z = direction.Z * speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
			}

			Velocity = velocity;

			if(!IsMultiplayerAuthority())
			{
				return;
			}
			if (playerCamera == null) return;

			Vector2 mousePos = GetViewport().GetMousePosition();
			Vector3 rayOrigin = playerCamera.ProjectRayOrigin(mousePos);
			Vector3 rayDir = playerCamera.ProjectRayNormal(mousePos);

			// Create a flat horizontal plane at the player's current Y height
			Plane groundPlane = new Plane(Vector3.Up, GlobalPosition.Y);
			Vector3? intersection = groundPlane.IntersectsRay(rayOrigin, rayDir);

			if (intersection.HasValue)
			{
				Vector3 lookTarget = new Vector3(intersection.Value.X, GlobalPosition.Y, intersection.Value.Z);

				//playerMesh.LookAt(lookTarget, Vector3.Up);

				Vector3 lookDirection = (GlobalPosition - lookTarget).Normalized();
				float targetAngle = Mathf.Atan2(-lookDirection.X, -lookDirection.Z);
				playerMesh.GlobalRotation = new Vector3(
					playerMesh.GlobalRotation.X,
					Mathf.LerpAngle(playerMesh.GlobalRotation.Y, targetAngle, rotationSpeed * (float)delta),
					playerMesh.GlobalRotation.Z
				);
			}
			MoveAndSlide();
		}
	}

	public void OpenMenu(bool isVisible)
	{
		menu.Visible = !isVisible;
		immobilized = menu.Visible;
		if(menu.Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}


}
