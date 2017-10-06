using UnityEngine;
using System.Collections;

public enum BattleState
{
    Choice = 0, Pre = 1, Fight = 2, Post = 3
}
public class BattleStateMachineScript
{
    public BattleState Battle_State;
    public ArmyUnitScript OneArmy;
    public ArmyUnitScript TwoArmy;
    private bool isOneWin;

    public BattleStateMachineScript()
    {
        this.Battle_State = BattleState.Choice;
        this.OneArmy = null;
        this.TwoArmy = null;
        this.isOneWin = false;
    }

    #region CLICK_Battle
    public void ClickPreRealBattle()
    {
        if (Battle_State == BattleState.Choice)
        { Battle_State = BattleState.Pre; }
        else
        { Debug.Log("error in ClickPreRealBattle in StateMachineScript"); }
    }
    public void ClickFightRealBattle()
    {
        if (Battle_State == BattleState.Pre)
        { Battle_State = BattleState.Fight; }
        else
        { Debug.Log("error in ClickFightRealBattle in StateMachineScript"); }
    }
    public void ClickPostRealBattle(bool isOneWin)
    {
        if (Battle_State == BattleState.Fight)
        {
            Battle_State = BattleState.Post;
            this.isOneWin = isOneWin;
        }
        else
        { Debug.Log("error in ClickPostRealBattle in StateMachineScript"); }
    }
    public void ClickEndRealBattle()
    {
        if (Battle_State == BattleState.Post)
        {
            this.OneArmy = null;
            this.TwoArmy = null;
        }
        else
        { Debug.Log("error in ClickEndRealBattle in StateMachineScript"); }
    }
    public bool GetIsOneWin()
    { return isOneWin; }
    #endregion
}
