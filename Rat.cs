using Godot;
using System;

public class Rat : Area2D
{
	private Random rnd;
	private float MovementTimer;
	private Vector2 Velocity;
	private Vector2 ScreenSize;
	private int Speed = 200;
	
	public override void _Ready()
	{
		rnd = new Random();
	}
	public override void _Process(float delta) {
		// Basic Movement AI
		MovementTimer = Mathf.Max(0, MovementTimer - delta);
		int MovementDirection = rnd.Next(1,5);
		if (MovementTimer <= 0) {
			Velocity = Vector2.Zero;
			switch(MovementDirection){
				case 1:
					Velocity.x += 1;
					break;
				case 2:
					Velocity.x -= 1;
					break;
				case 3:
					Velocity.y += 1;
					break;
				case 4:
					Velocity.y -= 1;
					break;
			}
			MovementTimer = 0.2f;
		}
		Velocity = Velocity.Normalized() * Speed;
		Position += Velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);
	}
	private void _on_Rat_area_entered(object area)
	{
		if (area is Bullet) {
			GD.Print("SLAY");
			Hide();
			this.QueueFree();
			//TODO: Emit score signal or add to score
		}
	}
}
