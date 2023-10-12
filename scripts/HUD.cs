using Godot;
using System;

//References:
//license: Freeware, Non-Commercial
//link: https://www.fontspace.com/merkur-font-f11606

public class HUD : Node
{
	[Export]
	public int MAX_HEALTH { get; set; } = 9;
	[Export]
	public int CATNIP_PRICE { get; set; } = 2;
	[Export]
	public int MILK_PRICE { get; set; } = 3;
	[Export]
	public int SPECIAL_UPGRADE_PRICE { get; set; } = 5;
	[Export]
	public int SHOT_SPEED_UPGRADE_PRICE { get; set; } = 7;
	[Export]
	public int SHOT_RANGE_UPGRADE_PRICE { get; set; } = 5;
	[Export]
	public int SCORE_MULTIPLIER_UPGRADE_PRICE { get; set; } = 5;
	[Export]
	public float SHOP_AVAILABILITY { get; set; } = 10f;
	
	public int Score = 0;
	public int DashCooldownPercentage = 100;
	public int ChargeShotCooldownPercentage = 100;
	public int Health;
	public int Gold = 0;
	public int Multiplier = 1;
	private Label ScoreLabel;
	private Label GoldLabel;
	private Label WaveLabel;
	private Label MultiplierLabel;
	private Label CooldownLabel;
	private ProgressBar DashCooldownBar;
	private ProgressBar ChargeShotCooldownBar;
	private HBoxContainer HealthBar;
	private Button ShopButton;
	private Popup Shop;
	private TextureButton PauseButton;
	private Player player;
	private PackedScene CatnipScene = GD.Load<PackedScene>("res://scenes/Catnip.tscn");
	private HitAudio HitAudio;
	
	private float ShopAvailability;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health = MAX_HEALTH;
		ScoreLabel = GetNode<Label>("Score");
		GoldLabel = GetNode<Label>("Gold");
		MultiplierLabel = GetNode<Label>("Multiplier");
		WaveLabel = GetNode<Label>("WaveNotifier");
		CooldownLabel = GetNode<Label>("Cooldown");
		DashCooldownBar = GetNode<ProgressBar>("DashCooldownBar");
		ChargeShotCooldownBar = GetNode<ProgressBar>("ChargeShotCooldownBar");
		HealthBar = GetNode<HBoxContainer>("HealthBar");
		Shop = GetNode<Popup>("Shop");
		PauseButton = GetNode<TextureButton>("PauseButton");
		ShopButton = GetNode<Button>("ShopButton");
		player = GetParent().GetNode<Player>("Player");
		HitAudio = Shop.GetNode<HitAudio>("HitAudio");
	}
	
	public override void _Process(float delta) {
		if (ShopAvailability <= 0) {
			ShopButton.Hide();
		}
		DashCooldownBar.Value = DashCooldownPercentage;
		ChargeShotCooldownBar.Value = ChargeShotCooldownPercentage;
		ShopAvailability = Mathf.Max(0, ShopAvailability - delta);
	}
	
	public void AddScore(int Addition) {
		Score += Addition * Multiplier;
		ScoreLabel.Text = Score.ToString();
	}
	
	public void AddGold(int Addition) {
		Gold += Addition;
		GoldLabel.Text = Gold + " Gold";
	}
	
	public void DeductGold(int Reduction) {
		Gold -= Reduction;
		GoldLabel.Text = Gold + " Gold";
	}
	
	public void SetMultiplier (int Val) {
		Multiplier = Mathf.Clamp(Val, 1, 9);
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
		ShopButton.Show();
		ShopAvailability = SHOP_AVAILABILITY;
	}
	private void _on_Done_pressed()
	{
		Shop.Hide();
		PauseButton.Show();
		GetTree().Paused = false;
	}
	private void _on_BuyHealth_pressed()
	{
		if (Health < MAX_HEALTH & 
			Gold >= MILK_PRICE) {
			DeductGold(MILK_PRICE);
			AddHealth();
		} else {
			HitAudio.Hit();
		}
	}
	private void _on_BuyCatnip_pressed()
	{
		if (Gold >= CATNIP_PRICE) {
			Catnip catnip = CatnipScene.Instance<Catnip>();
			catnip.Position = player.GlobalPosition;
			Owner.AddChild(catnip);
			DeductGold(CATNIP_PRICE);
		} else {
			HitAudio.Hit();
		}
	}
	private void _on_UpgradeSpecial_pressed()
	{
		if (Gold >= SPECIAL_UPGRADE_PRICE & 
			player.SpecialLevel < 3) { // Only 1 upgrade available.
			player.SpecialLevel++;
			DeductGold(SPECIAL_UPGRADE_PRICE);
		} else {
			HitAudio.Hit();
		}
	}
	private void _on_UpgradeShotSpeed_pressed() // min 0.2f
	{
		if (Gold >= SHOT_SPEED_UPGRADE_PRICE & 
			player.BULLET_COOLDOWN > 0.2f) {
				player.BULLET_COOLDOWN -= 0.1f;
				DeductGold(SHOT_SPEED_UPGRADE_PRICE);
		}
		HitAudio.Hit();
	}
	private void _on_UpgradeRange_pressed()
	{
		if (Gold >= SHOT_RANGE_UPGRADE_PRICE &
			player.bulletTime < 1f) { //Maximum 1f;

			player.bulletTime += 0.05f;
			DeductGold(SHOT_RANGE_UPGRADE_PRICE);
		} else {
			HitAudio.Hit();
		}
	}
	private void _on_UpgradeMultiplier_pressed()
	{
		if (Gold >= SCORE_MULTIPLIER_UPGRADE_PRICE &
			Multiplier < 5) {
			Multiplier++;
			DeductGold(SCORE_MULTIPLIER_UPGRADE_PRICE);
		}
		else {
			HitAudio.Hit();
		}
	}
	private void _on_ShopButton_pressed()
	{
		Shop.Show();
		PauseButton.Hide();
		GetTree().Paused = true;
		ShopButton.Hide();
	}
	public void SetCooldownVisibility(bool visible)
	{
		GD.Print(visible);
		CooldownLabel.Visible = visible;
		if (!visible) {
			ShopButton.Hide();
		}
	}
}
