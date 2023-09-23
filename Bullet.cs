using Godot;
using System;

public class Bullet : Area2D
{
	private int speed = 500;
	public Vector2 velocity;
	private bool isDestroyed = false;
	private float deathTime = 0.5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Hide();
	}
	public override void _Process(float delta)
	{
		if (isDestroyed) {
			if (deathTime <= 0) {
				this.QueueFree();
			}
			deathTime = Mathf.Max(0, deathTime - delta);
		} else {
			velocity = velocity.Normalized() * speed;
			Position += velocity * (float)delta;
			Position = new Vector2(Position.x, Position.y);
		}
	}
	private void _on_Bullet_area_entered(object area)
	{
		if (!(area is Player | area is Bullet)) {
			CPUParticles2D DeathParticles = GetNode<CPUParticles2D>("DeathParticles");
			try {
				GetNode<Sprite>("BulletSprite").QueueFree();
			} catch(Exception e) {//TODO: BUG on hit enemy
				GD.Print("Couldn't clear bullet sprite before displaying particles.");
			}
			//GetNode<CollisionShape2D("CollisionShape2D").QueueFree();
			DeathParticles.Show();
			isDestroyed = true;
		}
	}
}

