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
	public int Gold = 0;
	public int Multiplier = 1;
	private Label ScoreLabel;
	private Label GoldLabel;
	private Label WaveLabel;
	private Label MultiplierLabel;
	private ProgressBar DashCooldownBar;
	private ProgressBar ChargeShotCooldownBar;
	private HBoxContainer HealthBar;
	private Popup Shop;
	private TextureButton PauseButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health = MAX_HEALTH;
		ScoreLabel = GetNode<Label>("Score");
		GoldLabel = GetNode<Label>("Gold");
		MultiplierLabel = GetNode<Label>("Multiplier");
		WaveLabel = GetNode<Label>("WaveNotifier");
		DashCooldownBar = GetNode<ProgressBar>("DashCooldownBar");
		ChargeShotCooldownBar = GetNode<ProgressBar>("ChargeShotCooldownBar");
		HealthBar = GetNode<HBoxContainer>("HealthBar");
		Shop = GetNode<Popup>("Shop");
		PauseButton = GetNode<TextureButton>("PauseButton");
	}
	
	public override void _Process(float delta) {
		DashCooldownBar.Value = DashCooldownPercentage;
		ChargeShotCooldownBar.Value = ChargeShotCooldownPercentage;
	}
	
	public void AddScore(int Addition) {
		Score += Addition * Multiplier;
		ScoreLabel.Text = Score.ToString();
	}
	
	public void AddGold(int Addition) {
		Gold += Addition * Multiplier;
		GoldLabel.Text = Gold + " Gold";
	}
	
	public void SetMultiplier (int Val) {
		Multiplier = Val;
		MultiplierLabel.Text = "x" + Val;
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
		Shop.Show();
		PauseButton.Hide();
		GetTree().Paused = true;
	}
	private void _on_Done_pressed()
	{
		Shop.Hide();
		PauseButton.Show();
		GetTree().Paused = false;
	}
	private void _on_BuyHealth_pressed()
	{
		AddHealth();
	}
}
