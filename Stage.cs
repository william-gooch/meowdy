using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	private Vector2 ScreenSize;
	private Player player;
	private int bulletSpeed = 500;
	private float BulletCooldown = 0.5f;
	private float CurrentBulletCooldown = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		GD.Randomize();
		player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);
	}
	
	public override void _Process(float delta)
	{
		Bullet bullet = (Bullet)BulletScene.Instance();
		if (CurrentBulletCooldown <= 0) {
			GetBulletVelocity(bullet);
			if (bullet.velocity != Vector2.Zero) {
				this.AddChild(bullet);
				CurrentBulletCooldown = BulletCooldown;
			}
		}
		CurrentBulletCooldown = Mathf.Max(0, CurrentBulletCooldown - delta);
	}
	
	private void GetBulletVelocity(Bullet bullet) {
		if (Input.IsActionPressed("shoot_up"))
		{
			bullet.Position = player.Position;
			bullet.velocity.y -= 1;
		}
		if (Input.IsActionPressed("shoot_down"))
		{
			bullet.Position = player.Position;
			bullet.velocity.y += 1;
		}
		if (Input.IsActionPressed("shoot_left"))
		{
			bullet.Position = player.Position;
			bullet.velocity.x -= 1;
		}
		if (Input.IsActionPressed("shoot_right"))
		{
			bullet.Position = player.Position;
			bullet.velocity.x += 1;
		}
	}
}
