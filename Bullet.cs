using Godot;
using System;

public class Bullet : Area2D
{
	private int speed = 500;
	public Vector2 velocity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Hide();
	}
	public override void _Process(float delta)
	{
		velocity = velocity.Normalized() * speed;
		Position += velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);
	}
	private void _on_Bullet_area_entered(object area)
	{
		if (!(area is Player | area is Bullet)) {
			Hide();
			this.QueueFree();
		}
	}
}

