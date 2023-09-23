using Godot;
using System;

public class HealthBar : HBoxContainer
{
	private int MAX_HEALTH = 9;
	private Texture Full;
	private Texture Empty;

	public override void _Ready()
	{
		Full = (Texture)GD.Load("res://assets/heart/heart1.png");
		Empty = (Texture)GD.Load("res://assets/heart/heart2.png");
	}
	public override void _Process(float delta) {

	}
	public void UpdateHealth(int Health) {
		for (int i = 0;i < GetChildCount(); i++) {
			if (Health > 0) {
				((TextureRect)(GetChild(i))).Texture = Full;
				Health--;
			} else {
				((TextureRect)(GetChild(i))).Texture = Empty;
			}
		}
	}
}
