using Godot;
using System;

public class DeathParticles : CPUParticles2D
{
	public override void _Ready()
	{
		Hide();
	}
}
