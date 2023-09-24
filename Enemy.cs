using Godot;
using System;

public class Enemy : Area2D
{
	[Export]
	public float FlashTime { get; set; }
	[Export]
	public int MaxHealth { get; set; }
	[Export]
	public int ScoreValue { get; set; }
	[Export]
	public float DeathDelay { get; set; } = 0.5f;
	[Export]
	public float HitKnockback { get; set; }

	protected AnimatedSprite Sprite;
	protected HUD HUD;
	protected int CurrentHealth { get; set; }
	protected bool IsDead { get; private set; }

	private Node2D _player;
	private float _flashCooldown = 0;
	private Vector2 _velocity = Vector2.Zero;

	[Signal]
	public delegate void HitEventHandler(Vector2 direction);
	[Signal]
	public delegate void DiedEventHandler();

	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("walk");

		HUD = GetNode<HUD>("/root/Stage/HUD");
		CurrentHealth = MaxHealth;

		_player = GetNodeOrNull<Node2D>("/root/Stage/Player");

		Connect(nameof(HitEventHandler), this, nameof(OnHit));
		Connect(nameof(DiedEventHandler), this, nameof(OnDeath));
	}

	public virtual Vector2 Move(float delta)
	{
		_velocity *= 1 - (8f * delta);
		if (_velocity.Length() < 10)
		{
			_velocity = Vector2.Zero;
		}

		return _velocity;
	}

	public override void _Process(float delta)
	{
		Vector2 movement = Move(delta);

		Position += movement * (float)delta;

		if (_flashCooldown <= 0)
		{
			Sprite.Modulate = new Color("#ffffff");
		}
		_flashCooldown = Mathf.Max(0, _flashCooldown - delta);
	}

	protected virtual async void OnDeath()
	{
		GetNode("CollisionPolygon2D").QueueFree();
		IsDead = true;
		Sprite.Stop();
		await ToSignal(GetTree().CreateTimer(DeathDelay), "timeout");

		Hide();
		QueueFree();
		HUD.Call("AddScore", ScoreValue);
	}

	protected virtual void OnHit(Vector2 direction)
	{
		Sprite.Modulate = new Color("#960000");
		_flashCooldown = FlashTime;

		_velocity = direction * HitKnockback;

		if (CurrentHealth > 1)
		{
			//GD.Print("HIT!");
			CurrentHealth--;
		}
		else
		{
			//GD.Print("KILL!!!!");
			EmitSignal(nameof(DiedEventHandler));
		}
	}

	protected Vector2 DirectionToPlayer()
	{
		if (_player == null)
		{
			return Vector2.Down;
		}
		else
		{
			return (_player.GlobalTransform.origin - GlobalTransform.origin).Normalized();
		}
	}
}
