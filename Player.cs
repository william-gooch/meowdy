using Godot;
using System;
using System.Threading.Tasks;

public class Player : Area2D
{
	[Export]
	public float Acceleration { get; set; } = 3000; // How fast the player will accelerate (pixels/sec/sec)
	[Export]
	public float ImpulseDamping { get; set; } = 12f; // The damping factor when velocity is above MaxSpeed
	[Export]
	public float StopDamping { get; set; } = 15f; // The damping factor when no input is pressed.
	[Export]
	public int MaxSpeed { get; set; } = 300; // The maximum speed of the player (pixels/sec).
	[Export]
	public int DashSpeed { get; set; } = 1600; // The speed given to the player when they dash (pixels/sec).
	[Export]
	public float DashCooldown { get; set; } = 2f; // The time it takes to regain your dash (sec).
	[Export]
	public float HitKnockback { get; set; } = 600; // The speed at which you're knocked back when hit.
	[Export]
	public float DashInvulnerabilityTime { get; set; } = 0.5f; // Amount of time you are invulnerable when you dash (sec)
	[Export]
	public float HitInvulnerabilityTime { get; set; } = 1f; // Amount of time you are invulnerable when you are hit (sec)
	[Export]
	public float SpawnInvulnerabilityTime { get; set; } = 1f; // Amount of time you are invulnerable when you spawn in (sec)

	private HitAudio Audio;
	private Vector2 ScreenSize; // Size of the game window.
	private Vector2 Velocity = Vector2.Zero;
	private AnimatedSprite Sprite;
	private Vector2 PressDirection = Vector2.Zero;
	private float CurrentDashCooldown = 0f;
	private HUD HUD;
	private float InvulnerabilityCooldown; //TODO: Changes colour of sprite when !0, for taking damage
	private bool Dash = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		HUD = GetNode<HUD>("/root/Stage/HUD");
		Audio = GetNode<HitAudio>("HitAudio");

		InvulnerabilityCooldown = SpawnInvulnerabilityTime;
		Sprite.Modulate = new Color("#ffffff")
		{
			a = 0.5f
		};
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var movement = GetMovement();

		if (Input.IsActionJustPressed("dash") && CurrentDashCooldown <= 0f)
		{
			Velocity = movement * DashSpeed;
			CurrentDashCooldown = DashCooldown;
			InvulnerabilityCooldown = DashInvulnerabilityTime;
			//Sprite.Modulate = new Color("#ffffff")
			//{
			//a = 0.5f
			//};
			Sprite.Play("dash");
			Dash = true;
		}
		// Update DashCooldownBar
		HUD.DashCooldownPercentage = 100 - (int)((CurrentDashCooldown / DashCooldown) * 100);

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
			if (Velocity.Length() < 10)
			{
				Velocity = Vector2.Zero;
			}
		}

		// Sprite.SpeedScale = Mathf.Exp(Velocity.Length() / MaxSpeed) / Mathf.E;
		if (!Dash)
		{
			if (Velocity.Length() > 0)
			{
				Sprite.Play("new-walk");
				Sprite.FlipH = Velocity.x > 0;
			}
			else
			{
				Sprite.Play("idle");
			}
		}

		Position += Velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
			y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
		);

		if (InvulnerabilityCooldown <= 0)
		{
			Sprite.Modulate = new Color("#ffffff");
		}
		InvulnerabilityCooldown = Mathf.Max(0, InvulnerabilityCooldown - delta);
		CurrentDashCooldown = Mathf.Max(0, CurrentDashCooldown - delta); // Decrease cooldown, make sure it doesn't go below 0.
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
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
	private void _on_Player_area_entered(object area)
	{
		if (InvulnerabilityCooldown > 0)
		{
			return;
		}

		if (area is Enemy | area is Obstacle | area is BigObstacle)
		{
			if (HUD.Health > 1)
			{
				InvulnerabilityCooldown = HitInvulnerabilityTime;
				Sprite.Modulate = new Color("#960000")
				{
					a = 0.5f
				};
				HUD.Call("DeductHealth");

				Velocity = (GlobalPosition - (area as Node2D).GlobalPosition).Normalized() * HitKnockback;
				Audio.Hit();
			}
			else
			{
				GameOver();
			}
		}
	}
	private void _on_AnimatedSprite_animation_finished()
	{
		if (Sprite.Animation == "dash")
		{
			Dash = false;
		}
	}

	private void GameOver()
	{
		int Score = HUD.Score;
		var leaderboard = GetNode<LeaderboardManager>("/root/LeaderboardManager");
		leaderboard.CurrentScore = Score;

		GetTree().ChangeScene("res://leaderboard/LeaderboardScreen.tscn");
	}
}
