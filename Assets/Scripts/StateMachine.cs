using System;
using System.Collections.Generic;

public class StateMachine 
{
    IState currentState;
    Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    List<Transition> currentTransitions = new List<Transition>();
    List<Transition> anyTransitions = new List<Transition>();
    static List<Transition> emptyTransitions = new List<Transition>();

    public IState CurrentState { get => currentState; }

    public void Update()
    {
        var trans = GetTransition();
        if (trans != null)
        {       
            SetState(trans.To);
        }
        currentState?.StateUpdate();
    }


    public void FixedUpdate()
    {
        currentState?.FixedStateUpdate();
    }

    Transition GetTransition()
    {
        foreach (var item in anyTransitions)
        {
            if (item.Condition())
            {             
                return item;
            }
        }
        foreach (var item in currentTransitions)
        {
            if (item.Condition())
            {
                return item;
            }
        }
        return null;
    }

    public void SetState(IState state)
    {
        if (state == currentState)
        {
            return;
        }
        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
        {
            currentTransitions = emptyTransitions;
        }
        currentState.OnEnter();
    }  

    public void ForceEnter(IState state)
    {
        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
        {
            currentTransitions = emptyTransitions;
        }
        currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        if (transitions.TryGetValue(from.GetType(), out var Transitions) == false)
        {
            Transitions = new List<Transition>();
            transitions[from.GetType()] = Transitions;
        }
        Transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        anyTransitions.Add(new Transition(state, predicate));
    }  
}

public class Transition
{
    public Func<bool> Condition { get; }
    public IState To { get; }

    public Transition(IState to, Func<bool> condition)
    {
        To = to;
        Condition = condition;
    }
}

public interface IState
{    
    void StateUpdate();
	void FixedStateUpdate(); 
    void OnEnter();
    void OnExit();   
}



