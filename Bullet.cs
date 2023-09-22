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
		//velocity.y -= 1;
		velocity = velocity.Normalized() * speed;
		Position += velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);
	}
	private void _on_Bullet_body_entered(object body)
	{
		//Hide();
		//GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
	//TODO: this.QueueFree();
}
