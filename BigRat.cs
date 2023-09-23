using Godot;
using System;

public class BigRat : Area2D
{
	private Random rnd;
	private float MovementTimer;
	private Vector2 Velocity;
	private int Speed = 170;
	private AnimatedSprite Sprite;
	private HUD HUD;
	private int ScoreValue = 10; //Score added on death
	private int Health = 2;
	private float FlashCooldown;
	private float HIT_FRAMES = 0.5f;
	
	public override void _Ready()
	{
		rnd = new Random();
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("walk");
		HUD = GetNode<HUD>("/root/Stage/HUD");
	}
	public override void _Process(float delta) {
		// Basic Random Movement AI (Changes direction every 0.2 seconds)
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
		
		if (FlashCooldown <= 0) {
			Sprite.Modulate = new Color("#ffffff");
		}
		FlashCooldown = Mathf.Max(0, FlashCooldown - delta);
	}
	private void _on_BigRat_area_entered(object area)
	{
		if (area is Bullet) {
			if (Health > 1) {
				Sprite.Play("bloody_walk");
				Health--;
				FlashCooldown = HIT_FRAMES;
				Sprite.Modulate = new Color("#960000");
			} else {
				HUD.Call("AddScore", ScoreValue);
				Hide();
				this.QueueFree();
			}
		}
	}
}
