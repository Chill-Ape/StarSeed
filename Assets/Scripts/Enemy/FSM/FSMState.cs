using System;

[Serializable]
public class FSMState
{
    public string ID;
    public FSMAction[] Actions;
    public FSMTransition[] Transitions;

    public void UpdateState(EnemyBrain enemyBrain)
    {
        ExecuteActions();
        ExecuteTransitions(enemyBrain);
    }
    
    private void ExecuteActions()
    {
        if (Actions == null || Actions.Length == 0) return;
        
        for (int i = 0; i < Actions.Length; i++)
        {
            if (Actions[i] != null)
            {
                Actions[i].Act();
            }
        }
    }

    private void ExecuteTransitions(EnemyBrain enemyBrain)
    {
        if (Transitions == null || Transitions.Length <= 0) return;
        for (int i = 0; i < Transitions.Length; i++)
        {
            if (Transitions[i]?.Decision != null)
            {
                bool value = Transitions[i].Decision.Decide();
                if (value)
                {
                    enemyBrain.ChangeState(Transitions[i].TrueState);
                }
                else
                {
                    enemyBrain.ChangeState(Transitions[i].FalseState);
                }
            }
        }
    }
}