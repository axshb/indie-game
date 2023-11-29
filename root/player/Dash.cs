using Godot;
using System.Collections.Generic;

public partial class Dash : State
{
	
	private bool canDash = true;
	
	public override void _Ready() {
		
		Timer dashTimer = GetParent().GetParent().GetNode<Timer>("DashTimer"); 
		Callable dashTimerCallout = new Callable(this, MethodName.OnDashTimerTimeout);
		dashTimer.Connect("timeout", dashTimerCallout);
	}
	
	public override void Enter(Dictionary<string, object> msg = null){
		// This method is called when the Dash state is entered
		Player player = GetParent().GetParent() as Player;
		player.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("dash2");
	}
	
	public override void Update(double delta){
		Player player = GetParent().GetParent() as Player;
		Timer dashTimer = GetParent().GetParent().GetNode<Timer>("DashTimer");
		
		// Set the velocity based on input -- keep movement after state transition
		Vector2 inputMovement = new Vector2(
			(Input.IsActionPressed("move_right") ? 1 : 0) - (Input.IsActionPressed("move_left") ? 1 : 0),
			(Input.IsActionPressed("move_down") ? 1 : 0) - (Input.IsActionPressed("move_up") ? 1 : 0)
		);
		inputMovement = inputMovement.Normalized() * player.Speed;
		player.Velocity = inputMovement;

		if (canDash){
			canDash = false;
			// Apply dash speed
			player.Speed += player.DashSpeedAdd;
			
			// Start the dash timer
			dashTimer.Start();
			
			// Set the velocity based on input
			inputMovement = inputMovement.Normalized() * 2; // n * 2m = n * m * 2
			player.Velocity = inputMovement;
			

			
		}
		
		// Move and collide
		player.MoveAndCollide(player.Velocity * (float)delta);
	}
	
	private void OnDashTimerTimeout(){
		// reset speed
		Player player = GetParent().GetParent() as Player;
		player.Speed = 350;
		canDash = true;
					
		// State transitions
		StateMachine stateMachine = GetParent() as StateMachine;
		stateMachine.TransitionTo("Run");
	}
}
