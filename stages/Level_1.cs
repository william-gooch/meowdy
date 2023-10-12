using Godot;
using System;

public class Level_1 : Level
{
	public override float GetMobTime() {
		return 2f;
	}
	
	public override float GetBigRatSpawnChance()
	{
		return 0f;
	}
	public override float GetPowerUpCooldown()
	{
		return 11f;
	}

	public override float GetPowerUpSpawnChance()
	{
		return 0.2f;
	}
	public override int GetFinalWave()
	{
		return 2;
	}
	//Wave Changes
	public override float GetBigRatSpawnChanceAddition() {
		return 0.01f;
	}
	public override float GetMobTimeDeduction() {
		return 0.5f;
	}
}
