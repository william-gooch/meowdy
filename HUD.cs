using Godot;
using System;

//References:
//license: Freeware, Non-Commercial
//link: https://www.fontspace.com/merkur-font-f11606

public class HUD : Node
{
	public int Score = 0;
	public int DashCooldownPercentage = 100;
	public int ChargeShotCooldownPercentage = 100;
	public int MAX_HEALTH = 9;
	public int Health;
	private Label ScoreLabel;
	private Label WaveLabel;
	private ProgressBar DashCooldownBar;
	private ProgressBar ChargeShotCooldownBar;
	private HBoxContainer HealthBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health = MAX_HEALTH;
		ScoreLabel = GetNode<Label>("Score");
		WaveLabel = GetNode<Label>("WaveNotifier");
		DashCooldownBar = GetNode<ProgressBar>("DashCooldownBar");
		ChargeShotCooldownBar = GetNode<ProgressBar>("ChargeShotCooldownBar");
		HealthBar = GetNode<HBoxContainer>("HealthBar");
	}
	
	public override void _Process(float delta) {
		DashCooldownBar.Value = DashCooldownPercentage;
		ChargeShotCooldownBar.Value = ChargeShotCooldownPercentage;
	}
	
	public void AddScore(int Addition) {
		Score += Addition;
		ScoreLabel.Text = Score.ToString();
	}
	
	public void DeductHealth() {
		Health--;
		HealthBar.Call("UpdateHealth", Health);
	}
	
	public void AddHealth() {
		if (Health < 9) {
			Health++;
			HealthBar.Call("UpdateHealth", Health);
		}
	}
	public void UpdateWave(string text) {
		WaveLabel.Text = text;
	}
}
