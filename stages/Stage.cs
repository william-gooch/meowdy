using Godot;
using System;

// TODO: Additions and bug fixes
// Wave 2 Quite a lot of small rats: Per wave data to levels. 
	// Per wave data to level files
// Update score multiplier label from shop upgrade
// Cooldowns: 
	// dependent on killing last enemy
// Shop and Cooldown labels at the top of the screen and make them flash
	// Clear enemies and center player on new level and make new level dependent on cooldown

// Nice additions for game feel
	// Drifting currency pick-up radius


public class Stage : Node
{
	private PackedScene RatScene = GD.Load<PackedScene>("res://scenes/Rat.tscn");
	private PackedScene BigRatScene = GD.Load<PackedScene>("res://scenes/BigRat.tscn");
	private PackedScene MilkScene = GD.Load<PackedScene>("res://scenes/Milk.tscn");
	private PackedScene CatnipScene = GD.Load<PackedScene>("res://scenes/Catnip.tscn");
	private Vector2 ScreenSize;
	private Player player;
	private HUD HUD;
	//Mob Timer
	private float MobTime; //Time between each mob spawn
	private float MobTimer;
	private float BigRatSpawnChance;
	//Mob Spawn points
	private Random rnd;
	private Position2D LeftSpawnPosition;
	private Position2D RightSpawnPosition;
	private Position2D TopSpawnPosition;
	private Position2D BottomSpawnPosition;
	//Power up
	private float PowerUpCooldown;
	private float CurrentPowerUpCooldown;
	private float PowerUpSpawnChance;
	private double SEMITONE_MULTIPLIER = Math.Pow(2.0, 1.0 / 12.0);
	//Waves and levels
	private Popup PopUp;
	private int CurrentWave = 1;
	private int Level = 1;
	private Level CurrentLevel;
	private int FinalWave;
	private float WAVE_TIME = 60f;
	private float WaveTimer = 60f;
	private float WAVE_COOLDOWN = 10f;
	private float WaveCooldown= 0;
	private bool Cooldown = false;
	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		var startPosition = GetNode<Position2D>("StartPosition");
		LeftSpawnPosition = GetNode<Position2D>("LeftSpawnPosition");
		RightSpawnPosition = GetNode<Position2D>("RightSpawnPosition");
		TopSpawnPosition = GetNode<Position2D>("TopSpawnPosition");
		BottomSpawnPosition = GetNode<Position2D>("BottomSpawnPosition");
		PopUp = GetNode<Popup>("Popup");
		LoadLevel("Level_" + Level);
		ScreenSize = GetViewport().Size;
		CurrentPowerUpCooldown = PowerUpCooldown;
		GD.Randomize();
		rnd = new Random();
		player = GetNode<Player>("Player");
		player.Start(startPosition.Position);
		MobTimer = 3f;
	}
	public override void _Process(float delta)
	{
		if (!Cooldown) {
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
		}

		if (WaveTimer <= 0) {
			if (!Cooldown) {
				Cooldown = true;
				HUD.SetCooldownVisibility(true);
				WaveCooldown = WAVE_COOLDOWN;
				StartNextWave();
			}
			else {
				if (WaveCooldown <= 0) {
					Cooldown = false;
					HUD.SetCooldownVisibility(false);
					WaveTimer = WAVE_TIME;
				}
			}
		}

		// Deduct time from timers and cooldowns
		CurrentPowerUpCooldown = Mathf.Max(0, CurrentPowerUpCooldown - delta);
		MobTimer = Mathf.Max(0, MobTimer - delta);
		WaveTimer = Mathf.Max(0, WaveTimer - delta);
		WaveCooldown = Mathf.Max(0, WaveCooldown - delta);
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
			switch (PowerUpType)
			{
				case 1:
					Milk milk = (Milk)MilkScene.Instance();
					milk.Position = Position;
					this.AddChildBelowNode(player, milk);
					break;
				case 2:
					Catnip catnip = (Catnip)CatnipScene.Instance();
					catnip.Position = Position;
					this.AddChildBelowNode(player, catnip);
					break;
			}
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
		this.AddChildBelowNode(player, rat);
		MobTimer = MobTime; //TODO: Reduce time as time goes on
	}
	private void PitchUpMusic() {
		GetNode<AudioStreamPlayer>("Soundtrack").PitchScale = (float)Math.Pow(SEMITONE_MULTIPLIER, CurrentWave);
	}
	private void LoadLevel(String level) {
		if (CurrentLevel != null) {
			CurrentLevel.QueueFree();
		}
		CurrentLevel = (Level)GD.Load<PackedScene>("res://stages/" + level + ".tscn").Instance();
		MobTime = CurrentLevel.GetMobTime();
		BigRatSpawnChance = CurrentLevel.GetBigRatSpawnChance();
		PowerUpCooldown = CurrentLevel.GetPowerUpCooldown();
		PowerUpSpawnChance = CurrentLevel.GetPowerUpSpawnChance();
		FinalWave = CurrentLevel.GetFinalWave();
		AddChild(CurrentLevel);
		MoveChild(CurrentLevel,0);
	}
	private void StartNextWave() {
		CurrentWave++;
		HUD.UpdateWave("Wave " + CurrentWave);
		if (CurrentWave <= 10) { // 10 total waves
			if (CurrentWave > FinalWave) {
				PopUp.Show();
				GetTree().Paused = true;
			}
		} else {
			player.GameOver();
		}
		GetNode<AudioStreamPlayer>("Soundtrack").PitchScale = (float)Math.Pow(SEMITONE_MULTIPLIER, CurrentWave);
	}
	private void _on_NextLevelButton_pressed()
	{
		Level++;
		LoadLevel("Level_" + Level);
		GetTree().Paused = false;
		PopUp.Hide();
	}
}
