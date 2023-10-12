using Godot;
using System;

public class Level_2 : Level
{
	public override float GetMobTime() {
		return 2f;
	}
	
	public override float GetBigRatSpawnChance()
	{
		return 0.3f;
	}
	public override float GetPowerUpCooldown()
	{
		return 11f;
	}

	public override float GetPowerUpSpawnChance()
	{
		return 0.3f;
	}
	public override int GetFinalWave()
	{
		return 6;
	}
}
