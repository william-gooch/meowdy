using Godot;
using System;

public class Rat : Area2D
{
	private Random rnd;
	private float MovementTimer;
	private Vector2 Velocity;
	private Vector2 ScreenSize;
	private int Speed = 200;
	private AnimatedSprite Sprite;
	//private Node HUDScene;
	private HUD HUD;
	
	public override void _Ready()
	{
		rnd = new Random();
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		HUD = GetNode<HUD>("/root/Stage/HUD");
		//HUDScene = GD.Load<PackedScene>("res://HUD.cs");
	}
	public override void _Process(float delta) {
		// Basic Random Movement AI (Changes direction every 0.2 seconds)
		Sprite.Play("walk");
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
		Sprite.FlipH = Velocity.x > 0;
		Velocity = Velocity.Normalized() * Speed;
		Position += Velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);
	}
	private void _on_Rat_area_entered(object area)
	{
		if (area is Bullet) {
			GD.Print("SLAY");
			HUD.Score += 50;
			Hide();
			this.QueueFree();
			//TODO: Emit score signal or add to score
		}
	}
}