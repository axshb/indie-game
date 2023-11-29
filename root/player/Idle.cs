using Godot;
using System.Collections.Generic;

public partial class Idle : State
{
	public override void Enter(Dictionary<string, object> msg = null)
	{
		CharacterBody2D player = GetParent().GetParent() as CharacterBody2D;
		player.Velocity = Vector2.Zero;
		player.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle2");
	}
	
	public override void Update(double delta){
		StateMachine stateMachine = GetParent() as StateMachine;
		if (Input.IsActionJustPressed("dash")){
			stateMachine.TransitionTo("Dash");
			
		} else if (Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left") || 
					Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down")){
			stateMachine.TransitionTo("Run");
		}
	}
}
