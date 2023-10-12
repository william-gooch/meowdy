using Godot;
using System;

public abstract class Level : Node
{
    public abstract float GetMobTime();
    public abstract float GetBigRatSpawnChance();
    public abstract float GetPowerUpCooldown();
    public abstract float GetPowerUpSpawnChance();
    public abstract int GetFinalWave();
    // Wave Increments
    public abstract float GetBigRatSpawnChanceAddition();
    public abstract float GetMobTimeDeduction();
}