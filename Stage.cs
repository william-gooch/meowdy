using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	private Vector2 ScreenSize;
	private Player player;
	private int bulletSpeed = 500;

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
		if (Input.IsActionJustPressed("shoot_up"))
		{
			bullet.Position = player.Position;
			bullet.velocity.y -= 1;
		}
		if (Input.IsActionJustPressed("shoot_down"))
		{
			bullet.Position = player.Position;
			bullet.velocity.y += 1;
		}
		if (Input.IsActionJustPressed("shoot_left"))
		{
			bullet.Position = player.Position;
			bullet.velocity.x -= 1;
		}
		if (Input.IsActionJustPressed("shoot_right"))
		{
			bullet.Position = player.Position;
			bullet.velocity.x += 1;
		}
		if (bullet.velocity != Vector2.Zero) {
			this.AddChild(bullet);
		}
	}
}
