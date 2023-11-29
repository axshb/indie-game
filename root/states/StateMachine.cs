using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	// Emitted when transitioning to a new state.
	[Signal]
	public delegate void TransitionedEventHandler(string stateName);

	// Path to the initial active state. We export it to be able to pick the initial state in the inspector.
	[Export]
	public NodePath InitialState { get; set; }

	// The current active state. At the start of the game, we get the `initial_state`.
	private State state;

	public override void _Ready()
	{
		state = GetNode<State>(InitialState);
		// The state machine assigns itself to the State objects' stateMachine property.
		foreach (Node child in GetChildren())
		{
			if (child is State stateNode)
			{
				stateNode.stateMachine = this;
			}
		}
		state.Enter();
	}

	// The state machine subscribes to node callbacks and delegates them to the state objects.
	public override void _UnhandledInput(InputEvent @event)
	{
		state.HandleInput(@event);
	}

	public override void _Process(double delta)
	{
		state.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		state.PhysicsUpdate(delta);
	}

	// This function calls the current state's Exit() function, then changes the active state,
	// and calls its Enter function.
	// It optionally takes a `msg` dictionary to pass to the next state's Enter() function.
	public void TransitionTo(string targetStateName, Dictionary<string, object> msg = null)
	{
		// Safety check, you could use an assert() here to report an error if the state name is incorrect.
		// We don't use an assert here to help with code reuse. If you reuse a state in different state machines
		// but you don't want them all, they won't be able to transition to states that aren't in the scene tree.
		if (!HasNode(targetStateName))
		{
			return;
		}

		state.Exit();
		state = GetNode<State>(new NodePath(targetStateName));
		state.Enter(msg);
		EmitSignal("Transitioned", state.Name);
	}
}
