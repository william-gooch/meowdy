using Godot;
using System;

public class Catnip : Area2D
{
	[Export]
	public float CatnipTimer = 9.5f;
	
	private HUD HUD;
	private int speed;
	private Player player;
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
				player.MaxSpeed = speed;
				QueueFree();
			}
			currentTime = Mathf.Max(0, currentTime - delta);
		}
	}
	private async void _on_Catnip_area_entered(object area)
	{
		if (area is Player)
		{
			player = (Player)area;
			speed = player.MaxSpeed;
			RemoveChild(GetNode("AnimatedSprite"));
			RemoveChild(GetNode("CollisionShape2D"));
			RemoveChild(GetNode("Sprite"));
			player.MaxSpeed = (int)(speed * 1.5);
			currentTime = CatnipTimer;
			isActive = true;
		}
	}
	private void _on_Timer_timeout()
	{
		if (player == null)
		{
			QueueFree();
		}
	}
}
