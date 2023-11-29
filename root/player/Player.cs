using Godot;
using System;

public partial class Player : CharacterBody2D {


	[Signal]
	public delegate void HitEventHandler();
	
	[Export]
	public int Speed {get; set;} = 350; // How fast the player will move (pixels/sec)
	
	[Export]
	public int DashSpeedAdd = 350;
	
	
	public Vector2 velocity;
	
	private bool canDash = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Hide();
	}
	
	public int GetSpeed(){
		return Speed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
		var playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		playerVariables.updatePosition(this.Position);
		
		// Get the position of the cursor in global coordinates
		Vector2 cursorPosition = GetGlobalMousePosition();
		
		// Determine the direction from the player to the cursor
		Vector2 direction = (cursorPosition - GlobalPosition).Normalized();
		
		// Flip the player based on the direction
		if (direction.X < 0){
			// Player is facing left
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
		} else{
			// Player is facing right
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
		}
	}

	private void _on_area_2d_body_entered(Node2D body) {
		if (body.GetType() == typeof(Mob)){
			Hide(); // Player disappears after being hit.
			EmitSignal(SignalName.Hit);
			// Must be deferred as we can't change physics properties on a physics callback.
			GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		}
	}
	
	public void Start(Vector2 position) {
		Position = position;
		Show();
		GetNode<CollisionPolygon2D>("CollisionPolygon2D").Disabled = false;
	}
}
