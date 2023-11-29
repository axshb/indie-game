using Godot;
using System.Collections.Generic;

public partial class State : Node
{
	// Reference to the state machine, to call its `TransitionTo()` method directly.
	// That's one unorthodox detail of our state implementation, as it adds a dependency between the
	// state and the state machine objects, but we found it to be most efficient for our needs.
	// The state machine node will set it.
	public StateMachine stateMachine = null;

	// Virtual function. Receives events from the `_UnhandledInput()` callback.
	public virtual void HandleInput(InputEvent @event)
	{
		;// Your code here
	}

	// Virtual function. Corresponds to the `_Process()` callback.
	public virtual void Update(double delta)
	{
		;// Your code here
	}

	// Virtual function. Corresponds to the `_PhysicsProcess()` callback.
	public virtual void PhysicsUpdate(double delta)
	{
		;// Your code here
	}

	// Virtual function. Called by the state machine upon changing the active state.
	// The `msg` parameter is a dictionary with arbitrary data the state can use to initialize itself.
	public virtual void Enter(Dictionary<string, object> msg = null)
	{
		;// Your code here
	}

	// Virtual function. Called by the state machine before changing the active state.
	// Use this function to clean up the state.
	public virtual void Exit()
	{
		;// Your code here
	}
}
