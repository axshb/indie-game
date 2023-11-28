using Godot;
using System;

public partial class Player : CharacterBody2D {


	[Signal]
	public delegate void HitEventHandler();
	
	[Export]
	public int Speed {get; set;} = 350; // How fast the player will move (pixels/sec)
	
	[Export]
	public int dashSpeedAdd = 350;
	
	private bool canDash = true;

	public Vector2 ScreenSize; // Size of the game window
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		ScreenSize = GetViewportRect().Size;
		Hide();
		
		Callable dashTimerCallout = new Callable(this, MethodName.OnDashTimerTimeout);
		Timer dashTimer = GetNode<Timer>("DashTimer");
		dashTimer.Connect("timeout", dashTimerCallout);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
		var playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		playerVariables.updatePosition(this.Position);
		
		var velocity = Vector2.Zero; // The player's movement vector.
		
		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}
		
		// Selecting animations
		// TO DO: may not flip based on direction. Need to check
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else {
			animatedSprite2D.Stop();
		}

		MoveAndCollide(velocity * (float)delta);
		if (Input.IsActionJustPressed("dash") && canDash){
			GetNode<Timer>("DashTimer").Start();
			this.Speed = Speed + dashSpeedAdd;
			canDash = false;
			//velocity = new Vector2(dashSpeed * mouseDir.X, DASH_SPEED * mouseDir.Y);
		}

		// TO DO: use this when selecting animations

		/**
		if (velocity.X != 0) {
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0) {
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
		*/

	}
	
	private void OnDashTimerTimeout(){
		this.Speed = 350;
		canDash = true;
	}

	private void _on_area_2d_body_entered(Node2D body) {
		if (body.GetType() == typeof(Mob)){
			Hide(); // Player disappears after being hit.
			EmitSignal(SignalName.Hit);
			// Must be deferred as we can't change physics properties on a physics callback.
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}

	public void Start(Vector2 position) {
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
