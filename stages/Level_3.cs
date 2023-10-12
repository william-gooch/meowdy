using Godot;
using System;

public class Level_3 : Level
{
	public override float GetMobTime() {
		return 2f;
	}
	
	public override float GetBigRatSpawnChance()
	{
		return 0.4f;
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
	//Wave Changes
	public override float GetBigRatSpawnChanceAddition() {
		return 0.1f;
	}
	public override float GetMobTimeDeduction() {
		return 0.2f;
	}
}
