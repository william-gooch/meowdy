using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	public Vector2 ScreenSize;
	public Player player;

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
		if (Input.IsActionJustPressed("shoot_up"))
		{
			Bullet bullet = (Bullet)BulletScene.Instance();
			bullet.Position = player.Position;
			this.AddChild(bullet);
			bullet.LaunchBullet();
		}
		if (Input.IsActionJustPressed("shoot_down"))
		{
			Bullet bullet = (Bullet)BulletScene.Instance();
			bullet.Position = player.Position;
			this.AddChild(bullet);
			bullet.LaunchBullet();
		}
		if (Input.IsActionJustPressed("shoot_left"))
		{
			Bullet bullet = (Bullet)BulletScene.Instance();
			bullet.Position = player.Position;
			this.AddChild(bullet);
			bullet.LaunchBullet();
		}
		if (Input.IsActionJustPressed("shoot_right"))
		{
			Bullet bullet = (Bullet)BulletScene.Instance();
			bullet.Position = player.Position;
			this.AddChild(bullet);
			bullet.LaunchBullet();
		}
	}
}
