using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	private PackedScene RatScene;
	private PackedScene BigRatScene;
	private TileMap Background;
	private Vector2 ScreenSize;
	private Player player;
	private HUD HUD;
	
	//Bullet Physics
	public float CHARGE_SHOT_COOLDOWN{get; set;} = 3f; // Press "F" key for charge shot
	private float BulletCooldown = 0.5f;
	private float CurrentBulletCooldown = 0f;
	private float CurrentChargeShotCooldown = 0f;
	
	//Mob Timer
	private float MOB_TIME = 2f;
	private float MobTimer;
	private float BigRatSpawnChance = 0.3f;
	
	//Mob Spawn points
	private Random rnd;
	private Position2D LeftSpawnPosition;
	private Position2D RightSpawnPosition;
	private Position2D TopSpawnPosition;
	private Position2D BottomSpawnPosition;
	
	private int NUM_OBSTACLES = 6;

	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		RatScene = GD.Load<PackedScene>("res://Rat.tscn");
		BigRatScene = GD.Load<PackedScene>("res://BigRat.tscn");
		var ObstacleScene = GD.Load<PackedScene>("res://Obstacle.tscn");
		var BigObstacleScene = GD.Load<PackedScene>("res://BigObstacle.tscn");
		var startPosition = GetNode<Position2D>("StartPosition");
		LeftSpawnPosition = GetNode<Position2D>("LeftSpawnPosition");
		RightSpawnPosition = GetNode<Position2D>("RightSpawnPosition");
		TopSpawnPosition = GetNode<Position2D>("TopSpawnPosition");
		BottomSpawnPosition = GetNode<Position2D>("BottomSpawnPosition");
		Background = GetNode<TileMap>("Background");
		ScreenSize = GetViewport().Size;
		GD.Randomize();
		rnd = new Random();
		player = GetNode<Player>("Player");
		player.Start(startPosition.Position);
		MobTimer = 3f;
		
		// Spawn Obstacles
		// TODO: Prevent spawn on player or give i frames at start
		for (int i = 0; i < NUM_OBSTACLES; i++) {
			Area2D obstacle;
			if (rnd.Next(1,5) == 1) {
				obstacle = (BigObstacle)BigObstacleScene.Instance();
			} else {
				obstacle = (Obstacle)ObstacleScene.Instance();
			}
			obstacle.Position = new Vector2(
				rnd.Next(0,(int)ScreenSize.x),
				rnd.Next(0,(int)ScreenSize.y)
			);
			this.AddChildBelowNode(Background,obstacle);
		}
	}
	
	public override void _Process(float delta)
	{
		//Bullet Shooting Physics
		if (CurrentBulletCooldown <= 0) {
			if (ShootFromMovement()) {
				CurrentBulletCooldown = BulletCooldown;
			}
		}
		if (MobTimer <= 0) {
			SpawnMob();
		}
		
		// Charge Shot Physics
		if (CurrentChargeShotCooldown <= 0f & Input.IsActionPressed("charge_shot")) {
			ShootUp((Bullet)BulletScene.Instance());
			ShootDown((Bullet)BulletScene.Instance());
			ShootLeft((Bullet)BulletScene.Instance());
			ShootRight((Bullet)BulletScene.Instance());
			CurrentChargeShotCooldown = CHARGE_SHOT_COOLDOWN;
		}
		
		// Deduct time from timers and cooldowns
		CurrentChargeShotCooldown = Mathf.Max(0, CurrentChargeShotCooldown - delta);
		CurrentBulletCooldown = Mathf.Max(0, CurrentBulletCooldown - delta);
		MobTimer = Mathf.Max(0, MobTimer - delta);
		
		// HUD update
		HUD.ChargeShotCooldownPercentage = 100 - (int)((CurrentChargeShotCooldown/CHARGE_SHOT_COOLDOWN)*100);
	}
	private void SpawnMob() {
		//Spawning Mobs in 4 locations
		int location = rnd.Next(1,5);
		int type = rnd.Next(1,101);
		Area2D rat;
		if (type <= BigRatSpawnChance*100) {
			rat = (BigRat)BigRatScene.Instance();
		} else {
			rat = (Rat)RatScene.Instance();
		}
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
		this.AddChildBelowNode(Background,rat);
		MobTimer = MOB_TIME; //TODO: Reduce time as time goes on
	}
	
	//Get Bullet Velocity from inputs
	private bool ShootFromMovement() {
		bool HasShot = false;
		Bullet bullet = (Bullet)BulletScene.Instance();
		if (Input.IsActionPressed("shoot_up"))
		{
			ShootUp(bullet);
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_down"))
		{
			ShootDown(bullet);
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_left"))
		{
			ShootLeft(bullet);
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_right"))
		{
			ShootRight(bullet);
			HasShot = true;
		}
		return HasShot; // cannot use else case to allow for diagonals. 
	}
	private void Shoot(Bullet bullet) {
		//GD.Print(bullet.velocity);
		if (bullet.GetParent() == null && bullet.velocity != Vector2.Zero) {
			AddChild(bullet);
		}
	}
	private void ShootUp(Bullet bullet) {
		bullet.Position = player.Position;
		bullet.velocity.y -= 1;
		Shoot(bullet);
	}
	private void ShootDown(Bullet bullet) {
		bullet.Position = player.Position;
		bullet.velocity.y += 1;
		Shoot(bullet);
	}
	private void ShootLeft(Bullet bullet) {
		bullet.Position = player.Position;
		bullet.velocity.x -= 1;
		Shoot(bullet);
	}
	private void ShootRight(Bullet bullet) {
		bullet.Position = player.Position;
		bullet.velocity.x += 1;
		Shoot(bullet);
	}
}
