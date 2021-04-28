using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plan:
/// - Have a modular system that isn't all over the place like to old system
/// - The character turn queue employs both players and enemies, depending on the side that is taking turns
/// - Make the system a simple Get current turn -> Input -> Preform action (incl animations) <-> Numbers stuff (damage/heal) -> Signal "end turn" in animation -> Do turn passing stuff -> Move on to next char 
/// - If it's passing turn do Get current turn -> Input -> Pass turn -> Get current turn
/// - Make the calculations and animation work together, like RBA, have a system where the animation itself has an end turn function.
/// - If the enemy gets defeated before the move ends have a flag in the damage functions, if they should continue animating or not, if it's being done to all/random enemies, 
/// if there is an extra enemy, don't end (if flag is set to true).
/// </summary>
/*
public class s_battleEngine : MonoBehaviour
{
    #region variable
    public int fullTurn;
    public int halfTurn;

    BATTLE_ENGINE_STATE battleState;
    public bool playerTurn;
    #endregion

    #region characters and queues
    public List<o_battleChar> players;
    public List<o_battleChar> opponents;
    public List<o_battleChar> allCharacters {
        get {
            List<o_battleChar> bcs = new List<o_battleChar>();
            bcs.AddRange(players);
            bcs.AddRange(opponents);
            return bcs;
        }
    }

    public Queue<o_battleChar> characterTurnQueue = new Queue<o_battleChar>();
    #endregion

    #region visuals and GUI

    #endregion

    private void Update()
    {
        if (playerTurn)
        {
            switch (battleState)
            {
                case BATTLE_ENGINE_STATE.IDLE:


                    break;
            }
        }

    }

    public void CalculateDamage() { }

    /// <summary>
    /// Do the animations and calculations here, once done, then call the turn pass function
    /// </summary>
    /// <returns></returns>
    public IEnumerator PreformAction() {

        yield return new WaitForSeconds(0.1f);
    }
    /// <summary>
    /// Turn pass depending on the turn flag state and then throw the statemachine into the ending turn state
    /// </summary>
    /// <returns></returns>
    /// 
    public IEnumerator TurnPass()
    {

        yield return new WaitForSeconds(0.1f);
    }
}
*/