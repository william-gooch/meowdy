using Godot;
using System;

public class Catnip : Area2D
{
	[Export]
	public float CatnipTimer = 20f;
	
	private HUD HUD;
	private float currentTime;
	private bool isActive = false;
	
	
	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		AnimatedSprite Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("default");
	}
	public override void _Process(float delta)
	{
		if (isActive) {
			if (currentTime <= 0) {
				HUD.SetMultiplier(HUD.Multiplier-1);
				QueueFree();
			}
			currentTime = Mathf.Max(0, currentTime - delta);
		}
	}
	private async void _on_Catnip_area_entered(object area)
	{
		if (area is Player)
		{
			HUD.SetMultiplier(HUD.Multiplier+1);
			RemoveChild(GetNode("AnimatedSprite"));
			RemoveChild(GetNode("CollisionShape2D"));
			RemoveChild(GetNode("Sprite"));
			currentTime = CatnipTimer;
			isActive = true;
		}
	}
	private void _on_Timer_timeout()
	{
		if (!isActive)
		{
			QueueFree();
		}
	}
}
