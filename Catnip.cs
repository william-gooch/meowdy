using Godot;
using System;

public class Catnip : Area2D
{
	private HUD HUD;
	private int speed;
	private Player player;
	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		AnimatedSprite Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("default");
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
			//GD.Print("Speeding up");
			await ToSignal(GetTree().CreateTimer(9.5f), "timeout"); //TODO: Potential optimisation
			//GD.Print("Catnip wears off");
			player.MaxSpeed = speed;
			QueueFree();
		}
	}
	private void _on_Timer_timeout()
	{
		if (player != null)
		{
			player.MaxSpeed = speed;
		}
		this.QueueFree();
	}
}
