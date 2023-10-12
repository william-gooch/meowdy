using Godot;
using System;

public class Level_3 : Level
{
	public override float GetMobTime() {
		return 1f;
	}
	
	public override float GetBigRatSpawnChance()
	{
		return 0.5f;
	}
	public override float GetPowerUpCooldown()
	{
		return 5f;
	}

	public override float GetPowerUpSpawnChance()
	{
		return 0.3f;
	}
	public override int GetFinalWave()
	{
		return 10;
	}
}
