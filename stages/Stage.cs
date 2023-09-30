using Godot;
using System;


public class Stage : Node
{
	private PackedScene RatScene = GD.Load<PackedScene>("res://scenes/Rat.tscn");
	private PackedScene BigRatScene = GD.Load<PackedScene>("res://scenes/BigRat.tscn");
	private PackedScene MilkScene = GD.Load<PackedScene>("res://scenes/Milk.tscn");
	private PackedScene CatnipScene = GD.Load<PackedScene>("res://scenes/Catnip.tscn");
	private TileMap Background;
	private Vector2 ScreenSize;
	private Player player;
	private HUD HUD;

	//Mob Timer
	private float BASE_MOB_TIME = 2f;
	private float MOB_TIME = 2f;
	private float MobTimer;
	private float BaseBigRatSpawnChance = 0.05f;
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
	private float BasePowerUpSpawnChance = 0.2f;
	private float PowerUpSpawnChance = 0.2f;
	//Waves
	private int NUM_WAVES = 10;
	private int currentWave;
	private double SEMITONE_MULTIPLIER = Math.Pow(2.0, 1.0 / 12.0);

	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		var ObstacleScene = GD.Load<PackedScene>("res://scenes/Obstacle.tscn");
		var BigObstacleScene = GD.Load<PackedScene>("res://scenes/BigObstacle.tscn");
		var startPosition = GetNode<Position2D>("StartPosition");
		LeftSpawnPosition = GetNode<Position2D>("LeftSpawnPosition");
		RightSpawnPosition = GetNode<Position2D>("RightSpawnPosition");
		TopSpawnPosition = GetNode<Position2D>("TopSpawnPosition");
		BottomSpawnPosition = GetNode<Position2D>("BottomSpawnPosition");
		Background = GetNode<TileMap>("Background");
		ScreenSize = GetViewport().Size;
		CurrentPowerUpCooldown = PowerUpCooldown;
		GD.Randomize();
		rnd = new Random();
		player = GetNode<Player>("Player");
		player.Start(startPosition.Position);
		MobTimer = 3f;

		// Randomly Spawn Obstacles
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

		//MobSpawner
		if (MobTimer <= 0)
		{
			SpawnMob();
		}

		//Power Up Spawner
		if (CurrentPowerUpCooldown <= 0)
		{
			SpawnPowerUp();
			CurrentPowerUpCooldown = PowerUpCooldown;
		}


		// Deduct time from timers and cooldowns
		CurrentPowerUpCooldown = Mathf.Max(0, CurrentPowerUpCooldown - delta);
		MobTimer = Mathf.Max(0, MobTimer - delta);
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
			//GD.Print(PowerUpType);
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
			//GD.Print("Unlucky! No Power-Up yet");
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

	private void _on_WaveTimer_timeout()
	{
		if (currentWave < NUM_WAVES)
		{
			MOB_TIME = BASE_MOB_TIME * Mathf.Pow(0.9f, currentWave);
			BigRatSpawnChance = Mathf.Clamp((float)(BaseBigRatSpawnChance * Mathf.Pow(1.5f, currentWave)), 0, 1);
			PowerUpSpawnChance = Mathf.Clamp((float)(BasePowerUpSpawnChance * Mathf.Pow(1.5f, currentWave)), 0, 1);
			currentWave++;

			GetNode<AudioStreamPlayer>("Soundtrack").PitchScale = (float)Math.Pow(SEMITONE_MULTIPLIER, currentWave);

			HUD.UpdateWave("Wave " + currentWave);
		}
	}
}
