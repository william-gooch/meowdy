using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	private PackedScene RatScene;
	private Vector2 ScreenSize;
	private Player player;
	private int bulletSpeed = 500;
	private float BulletCooldown = 0.5f;
	private float CurrentBulletCooldown = 0f;
	private float MOB_TIME = 2f;
	private float MobTimer;
	private Random rnd;
	private Position2D LeftSpawnPosition;
	private Position2D RightSpawnPosition;
	private Position2D TopSpawnPosition;
	private Position2D BottomSpawnPosition;

	public override void _Ready()
	{
		BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		RatScene = GD.Load<PackedScene>("res://Rat.tscn");
		var startPosition = GetNode<Position2D>("StartPosition");
		LeftSpawnPosition = GetNode<Position2D>("LeftSpawnPosition");
		RightSpawnPosition = GetNode<Position2D>("RightSpawnPosition");
		TopSpawnPosition = GetNode<Position2D>("TopSpawnPosition");
		BottomSpawnPosition = GetNode<Position2D>("BottomSpawnPosition");
		GD.Randomize();
		rnd = new Random();
		player = GetNode<Player>("Player");
		player.Start(startPosition.Position);
		MobTimer = 3f;
	}
	
	public override void _Process(float delta)
	{
		//Bullet Shooting Physics
		Bullet bullet = (Bullet)BulletScene.Instance();
		if (CurrentBulletCooldown <= 0) {
			GetBulletVelocity(bullet);
			if (bullet.velocity != Vector2.Zero) {
				this.AddChild(bullet);
				CurrentBulletCooldown = BulletCooldown;
			}
		}
		
		//Spawning Mobs in 4 locations
		if (MobTimer <= 0) {
			Rat rat = (Rat)RatScene.Instance();
			int location = rnd.Next(1,5);
			switch(location) {
				case 1:
					rat.Position = LeftSpawnPosition.Position;
					break;
				case 2:
					rat.Position = RightSpawnPosition.Position;
					break;
				case 3:
					rat.Position = TopSpawnPosition.Position;
					break;
				case 4:
					rat.Position = BottomSpawnPosition.Position;
					break;
				default:
					break;
			}
			this.AddChild(rat);
			MobTimer = MOB_TIME; //TODO: Reduce time as time goes on
		}
		
		// Deduct time from timers and cooldowns
		CurrentBulletCooldown = Mathf.Max(0, CurrentBulletCooldown - delta);
		MobTimer = Mathf.Max(0, MobTimer - delta);
	}
	
	//Get Bullet Velocity from inputs
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
