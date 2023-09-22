using Godot;
using System;

public class Player : Area2D
{
	[Export]
	public float Acceleration { get; set; } = 3000; // How fast the player will accelerate (pixels/sec/sec)
	[Export]
	public float ImpulseDamping { get; set; } = 10f; // The damping factor when velocity is above MaxSpeed
	[Export]
	public float StopDamping { get; set; } = 15f; // The damping factor when no input is pressed.
	[Export]
	public int MaxSpeed { get; set; } = 400; // The maximum speed of the player (pixels/sec).

	private Vector2 ScreenSize; // Size of the game window.
	private Vector2 Velocity = Vector2.Zero;
	private AnimatedSprite Sprite;

	private Vector2 PressDirection = Vector2.Zero;

	[Signal]
	public delegate void Hit();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var movement = GetMovement();

		if (Input.IsActionJustPressed("dash"))
		{
			Velocity = movement * MaxSpeed * 3;
		}

		if (movement.Length() > 0)
		{
			Velocity += movement.Normalized() * (Acceleration * delta);
			if (Velocity.Length() > MaxSpeed)
			{
				Velocity *= 1 - (ImpulseDamping * delta);
			}
		}
		else
		{
			Velocity *= 1 - (StopDamping * delta);
		}

		// if (Velocity.Length() < 50)
		// {
		// Velocity = Vector2.Zero;
		// }

		if (Velocity.Length() > 0)
		{
			Sprite.Play();
		}
		else
		{
			Sprite.Stop();
		}

		Position += Velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
			y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
		);
	}
	private void _on_Player_body_entered(object body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(nameof(Hit));
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	private Vector2 GetMovement()
	{
		Vector2 movement = Vector2.Zero;

		if (Input.IsActionPressed("move_right") && Input.IsActionPressed("move_left"))
		{
			if (Input.IsActionJustPressed("move_right"))
				PressDirection.x = 1;
			if (Input.IsActionJustPressed("move_left"))
				PressDirection.x = -1;
			movement.x += PressDirection.x;
		}
		else if (Input.IsActionPressed("move_right"))
			movement += Vector2.Right;
		else if (Input.IsActionPressed("move_left"))
			movement += Vector2.Left;
		else
			PressDirection.x = 0;

		if (Input.IsActionPressed("move_down") && Input.IsActionPressed("move_up"))
		{
			if (Input.IsActionJustPressed("move_down"))
				PressDirection.y = 1;
			if (Input.IsActionJustPressed("move_up"))
				PressDirection.y = -1;
			movement.y += PressDirection.y;
		}
		else if (Input.IsActionPressed("move_down"))
			movement += Vector2.Down;
		else if (Input.IsActionPressed("move_up"))
			movement += Vector2.Up;
		else
			PressDirection.y = 0;

		return movement;
	}
}
