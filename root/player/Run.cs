using Godot;
using System;

public partial class Run : State
{
	public override void Update(double delta){
		
		Player player = GetParent().GetParent() as Player;

		Vector2 inputMovement = new Vector2(
			(Input.IsActionPressed("move_right") ? 1 : 0) - (Input.IsActionPressed("move_left") ? 1 : 0),
			(Input.IsActionPressed("move_down") ? 1 : 0) - (Input.IsActionPressed("move_up") ? 1 : 0)
		);
		
		inputMovement = inputMovement.Normalized() * player.Speed;
		player.Velocity = inputMovement;
		player.MoveAndCollide(inputMovement * (float)delta);
		
		player.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("run2");
		
		StateMachine stateMachine = GetParent() as StateMachine;
		Timer dashTimer = GetParent().GetParent().GetNode<Timer>("DashTimer"); 
		
		if (Input.IsActionJustPressed("dash") && dashTimer.TimeLeft <= 0){
			stateMachine.TransitionTo("Dash");
			
		} else if (inputMovement == Vector2.Zero){
			stateMachine.TransitionTo("Idle");
		}
	}
}
