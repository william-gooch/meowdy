using Godot;
using System;

public class Bullet : Area2D
{
	[Export]
	public int Speed { get; set; } = 800;

	public Vector2 direction;

	public bool Destroyed { get; private set; } = false;
	private float deathTime = 0.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	public override void _Process(float delta)
	{
		if (Destroyed)
		{
			if (deathTime <= 0)
			{
				this.QueueFree();
			}
			deathTime = Mathf.Max(0, deathTime - delta);
		}
		else
		{
			direction = direction.Normalized();
			Position += direction * Speed * (float)delta;
			Position = new Vector2(Position.x, Position.y);
		}
	}
	private async void _on_Bullet_area_entered(object area)
	{
		if (area is Enemy | area is BigObstacle | area is Obstacle)
		{
			if (area is Enemy)
			{
				(area as Enemy).EmitSignal(nameof(Enemy.HitEventHandler), this.direction);
			}

			CPUParticles2D DeathParticles = GetNode<CPUParticles2D>("DeathParticles");
			await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
			if (HasNode("BulletSprite"))
			{
				GetNode<Sprite>("BulletSprite").QueueFree();
			}
			if (HasNode("CollisionShape2D"))
			{
				GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
			}
			DeathParticles.Show();
			Destroyed = true;
		}
	}
}

