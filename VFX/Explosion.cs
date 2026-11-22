using Godot;
using System;
using System.Threading.Tasks;

public partial class Explosion : Node3D
{
	[Export] private GpuParticles3D debris;
	[Export] private GpuParticles3D fire;
	[Export] private GpuParticles3D smoke;

	public void Explode()
	{
		debris.Emitting = true;
		fire.Emitting = true;
		smoke.Emitting = true;
	
		QueueFree();
	}
}
