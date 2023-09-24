using Godot;
using System;

public class Stage : Node
{
	private PackedScene BulletScene;
	private PackedScene RatScene;
	private PackedScene BigRatScene;
	private PackedScene MilkScene = GD.Load<PackedScene>("res://Milk.tscn");
	private PackedScene CatnipScene = GD.Load<PackedScene>("res://Catnip.tscn");
	private TileMap Background;
	private Vector2 ScreenSize;
	private Player player;
	private HUD HUD;

	//Bullet Physics
	public float CHARGE_SHOT_COOLDOWN { get; set; } = 3f; // Press "F" key for charge shot
	private float BulletCooldown = 0.5f;
	private float CurrentBulletCooldown = 0f;
	private float CurrentChargeShotCooldown = 0f;

	//Mob Timer
	private float MOB_TIME = 2f;
	private float MobTimer;
	private float BigRatSpawnChance = 0.05f;
	//Mob Spawn points
	private Random rnd;
	private Position2D LeftSpawnPosition;
	private Position2D RightSpawnPosition;
	private Position2D TopSpawnPosition;
	private Position2D BottomSpawnPosition;

	private int NUM_OBSTACLES = 6;

	//Power up
	private float PowerUpCooldown = 10f;
	private float CurrentPowerUpCooldown;
	private float PowerUpSpawnChance = 0.1f;
	//Waves
	private int NUM_WAVES = 10;
	private int currentWave;

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
	TODO: CurrentPowerUpCooldown = PowerUpCooldown;
		GD.Randomize();
		rnd = new Random();
		player = GetNode<Player>("Player");
		player.Start(startPosition.Position);
		MobTimer = 3f;

		// Spawn Obstacles
		// TODO: Prevent spawn on player or give i frames at start
		for (int i = 0; i < NUM_OBSTACLES; i++)
		{
			Area2D obstacle;
			if (rnd.Next(1, 5) == 1)
			{
				obstacle = (BigObstacle)BigObstacleScene.Instance();
			}
			else
			{
				obstacle = (Obstacle)ObstacleScene.Instance();
			}
			obstacle.Position = new Vector2(
				rnd.Next(0, (int)ScreenSize.x),
				rnd.Next(0, (int)ScreenSize.y)
			);
			this.AddChildBelowNode(Background, obstacle);
		}
	}

	public override void _Process(float delta)
	{
		//Bullet Shooting Physics
		if (CurrentBulletCooldown <= 0)
		{
			if (ShootFromMovement())
			{
				CurrentBulletCooldown = BulletCooldown;
			}
		}

		//MobSpawner
		if (MobTimer <= 0)
		{
			SpawnMob();
		}

		// Charge Shot Physics
		if (CurrentChargeShotCooldown <= 0f & Input.IsActionPressed("charge_shot"))
		{
			Shoot(Vector2.Up);
			Shoot(Vector2.Down);
			Shoot(Vector2.Left);
			Shoot(Vector2.Right);
			CurrentChargeShotCooldown = CHARGE_SHOT_COOLDOWN;
		}

		//Power Up Spawner
		if (CurrentPowerUpCooldown <= 0)
		{
			SpawnPowerUp();
			CurrentPowerUpCooldown = PowerUpCooldown;
		}


		// Deduct time from timers and cooldowns
		CurrentChargeShotCooldown = Mathf.Max(0, CurrentChargeShotCooldown - delta);
		CurrentBulletCooldown = Mathf.Max(0, CurrentBulletCooldown - delta);
		CurrentPowerUpCooldown = Mathf.Max(0, CurrentPowerUpCooldown - delta);
		MobTimer = Mathf.Max(0, MobTimer - delta);

		// HUD update
		HUD.ChargeShotCooldownPercentage = 100 - (int)((CurrentChargeShotCooldown / CHARGE_SHOT_COOLDOWN) * 100);
	}

	// Spawns Power Up, checks spawnchance.
	private void SpawnPowerUp()
	{
		int SpawnChance = rnd.Next(1, 101);
		if (SpawnChance <= PowerUpSpawnChance * 100)
		{
			Vector2 Position = new Vector2(
				rnd.Next(0, (int)ScreenSize.x),
				rnd.Next(0, (int)ScreenSize.y)
			);
			int PowerUpType = rnd.Next(1, 3);
			GD.Print(PowerUpType);
			switch (PowerUpType)
			{
				case 1:
					Milk milk = (Milk)MilkScene.Instance();
					milk.Position = Position;
					this.AddChildBelowNode(Background, milk);
					break;
				case 2:
					Catnip catnip = (Catnip)CatnipScene.Instance();
					catnip.Position = Position;
					this.AddChildBelowNode(Background, catnip);
					break;
			}
		}
		else
		{
			GD.Print("Unlucky! No Power-Up yet");
		}
	}

	private void SpawnMob()
	{
		//Spawning Mobs in 4 locations
		int location = rnd.Next(1, 5);
		int type = rnd.Next(1, 101);
		Area2D rat;
		if (type <= BigRatSpawnChance * 100)
		{
			rat = (BigRat)BigRatScene.Instance();
		}
		else
		{
			rat = (Rat)RatScene.Instance();
		}
		switch (location)
		{
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
		this.AddChildBelowNode(Background, rat);
		MobTimer = MOB_TIME; //TODO: Reduce time as time goes on
	}

	//Get Bullet Velocity from inputs
	private bool ShootFromMovement()
	{
		bool HasShot = false;
		Vector2 direction = Vector2.Zero;
		if (Input.IsActionPressed("shoot_up"))
		{
			direction.y -= 1;
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_down"))
		{
			direction.y += 1;
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_left"))
		{
			direction.x -= 1;
			HasShot = true;
		}
		if (Input.IsActionPressed("shoot_right"))
		{
			direction.x += 1;
			HasShot = true;
		}

		if (HasShot)
		{
			Shoot(direction);
		}
		return HasShot; // cannot use else case to allow for diagonals. 
	}
	private void Shoot(Vector2 direction)
	{
		Bullet bullet = BulletScene.Instance<Bullet>();
		bullet.Position = player.Position;
		bullet.direction = direction;
		if (bullet.GetParent() == null && bullet.direction != Vector2.Zero)
		{
			AddChild(bullet);
		}
	}

	private void _on_WaveTimer_timeout()
	{
		if (currentWave < NUM_WAVES)
		{
			MOB_TIME = (float)(MOB_TIME * 0.9);
			BigRatSpawnChance = Mathf.Clamp((float)(BigRatSpawnChance * 1.5), 0, 1);
			PowerUpSpawnChance = Mathf.Clamp((float)(PowerUpSpawnChance * 1.5), 0, 1);
			currentWave++;
			HUD.UpdateWave("Wave " + currentWave);
		}
	}
	private void _on_WaveTimer_timeout()
	{
		if (currentWave < NUM_WAVES) {
			MOB_TIME = (float)(MOB_TIME * 0.9);
			BigRatSpawnChance = Mathf.Clamp((float)(BigRatSpawnChance * 1.5),0,1);
			PowerUpSpawnChance = Mathf.Clamp((float)(PowerUpSpawnChance * 1.5),0,1);
			currentWave++;
			HUD.UpdateWave("Wave "+currentWave);
		}
	}
}
