using Godot;
using System;

public class Enemy : Area2D
{
	[Export]
	public int MaxHealth { get; set; }
	[Export]
	public int ScoreValue { get; set; }

	protected AnimatedSprite Sprite;
	protected HUD HUD;
	protected int CurrentHealth { get; set; }

	private Node2D _player;

	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DiedEventHandler();
	
	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("walk");

		HUD = GetNode<HUD>("/root/Stage/HUD");
		CurrentHealth = MaxHealth;

		_player = GetNodeOrNull<Node2D>("/root/Stage/Player");

		Connect(nameof(DiedEventHandler), this, nameof(OnDeath));
	}

	public virtual Vector2 Move(float delta)
	{
		return Vector2.Zero;
	}

	public override void _Process(float delta) {
		Vector2 movement = Move(delta);
		Sprite.FlipH = movement.x > 0;
	}

	protected virtual void OnDeath()
	{
		Hide();
		QueueFree();
		HUD.Call("AddScore", ScoreValue);
	}

	private void OnEnemyCollide(object area)
	{
		if (area is Bullet) {
			if (CurrentHealth > 1) {
				GD.Print("HIT!");
				CurrentHealth--;
				EmitSignal(nameof(HitEventHandler));
			} else {
				GD.Print("KILL!!!!");
				EmitSignal(nameof(DiedEventHandler));
			}
		}
	}

	protected Vector2 DirectionToPlayer() {
		if(_player == null) {
			return Vector2.Down;
		} else {
			return (_player.GlobalTransform.origin - GlobalTransform.origin).Normalized();
		}
	}
}
