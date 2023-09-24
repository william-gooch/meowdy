using Godot;
using System;

public class Bullet : Area2D
{
	[Export]
	public int Speed { get; set; } = 800;

	public Vector2 direction;

	private bool isDestroyed = false;
	private float deathTime = 0.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	public override void _Process(float delta)
	{
		if (isDestroyed) {
			if (deathTime <= 0) {
				this.QueueFree();
			}
			deathTime = Mathf.Max(0, deathTime - delta);
		} else {
			direction = direction.Normalized() * Speed;
			Position += direction * (float)delta;
			Position = new Vector2(Position.x, Position.y);
		}
	}
	private async void _on_Bullet_area_entered(object area)
	{
		if (!(area is Player | area is Bullet)) {
			CPUParticles2D DeathParticles = GetNode<CPUParticles2D>("DeathParticles");
			DeathParticles.Show();
			try {
				isDestroyed = true;
				await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
				this.RemoveChild(GetNode<Sprite>("BulletSprite"));
				this.RemoveChild(GetNode<CollisionShape2D>("CollisionShape2D"));
			} catch(Exception e) {//TODO: BUG on hit enemy
				GD.Print("Couldn't clear bullet sprite before displaying particles.");
			}
		}
	}
}

