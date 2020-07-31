using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSMController:MonoBehaviour
{
    public List<BaseState> listState = new List<BaseState>();
    public BaseState currentState;
    public GameObject actor;

    public void AddState(BaseState state)
    {
        listState.Add(state);
        if (listState.Count == 1)
            currentState = state;
    }

    private bool ExitAndChangeState(BaseState newState)
    {
        if (listState.Contains(newState))
        {
            if (newState != currentState)
            {
                if (currentState != null) currentState.Exit();
                currentState = newState;
            }
            return true;
        }
        return false;
    }

    public void GoToState(BaseState newState)
    {
        if (ExitAndChangeState(newState))
            currentState.Enter();
    }

    public void UpdateState()
    {
        if (currentState != null) currentState.Update();
    }

    public void FixedUpdateState()
    {
        if (currentState != null) currentState.FixedUpdate();
    }

    public void LateUpdateState()
    {
        if (currentState != null) currentState.LateUpdate();
    }
}