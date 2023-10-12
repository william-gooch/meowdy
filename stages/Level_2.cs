using Godot;
using System;

public class Level_2 : Level
{
	public override float GetMobTime() {
		return 2f;
	}
	
	public override float GetBigRatSpawnChance()
	{
		return 0.1f;
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
	//Wave Changes
	public override float GetBigRatSpawnChanceAddition() {
		return 0.05f;
	}
	public override float GetMobTimeDeduction() {
		return 0.2f;
	}
}
