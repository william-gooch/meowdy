using Godot;
using System;

public class Catnip : Area2D
{
	private HUD HUD;
	public override void _Ready()
	{
		HUD = GetNode<HUD>("/root/Stage/HUD");
		AnimatedSprite Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Sprite.Play("default");
	}
	private async void _on_Catnip_area_entered(object area)
	{
		if (area is Player) {
			int CurrentSpeed = ((Player)area).MaxSpeed;
			this.RemoveChild(GetNode<AnimatedSprite>("AnimatedSprite"));
			this.RemoveChild(GetNode<CollisionShape2D>("CollisionShape2D"));
			((Player)area).MaxSpeed = (int)(CurrentSpeed * 1.5);
			GD.Print("Speeding up");
			await ToSignal(GetTree().CreateTimer(9.5f), "timeout"); //TODO: Potential optimisation
			GD.Print("Catnip wears off");
			((Player)area).MaxSpeed = CurrentSpeed;
			this.QueueFree();
		}
	}
}
