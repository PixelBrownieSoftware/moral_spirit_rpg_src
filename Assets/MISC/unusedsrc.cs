/*
 * 
 * 
 * 
added on 09/10/2020

if (!middleOfAction)
            {
                if (isPlayerTurn)
                {
                    currentCharacter = playerCharacterTurnQueue.Peek();

                    if (currentCharacter.hitPoints == 0 || currentCharacter.skillPoints == -currentCharacter.maxSkillPoints)
                    {
                        playerCharacterTurnQueue.Dequeue();
                        playerCharacterTurnQueue.Enqueue(currentCharacter);
                    }

                    if (!playerUpTurn)
                    {
                        //s_soundmanager.sound.PlaySound(ref upTurn, false);
                        playerUpTurn = true;
                    }
                    PlayerChoice();
                    if (netTurn <= 0)
                    {
                        pressTurn = oppositionCharacterTurnQueue.Count;
                        isPlayerTurn = false;
                    }
                }
                else
                {
                    currentCharacter = oppositionCharacterTurnQueue.Peek();
                    EnemySelectAttack(ref currentCharacter);

                    if (netTurn <= 0)
                    {
                        pressTurn = playerCharacterTurnQueue.Count;
                        isPlayerTurn = true;
                    }
                }
            }
            if (currentMove != null && !middleOfAction)
            {
                s_battleAction move = currentMove;
                s_move mov = move.move;

                if (!middleOfAction)
                {
                    middleOfAction = true;
                    if (move != null)
                    {
                        actionDisp.text = move.user.name + " used " + move.move.name + " on " + move.target.name + "\n";
                        switch (mov.moveType)
                        {
                            case MOVE_TYPE.PHYSICAL:
                                move.user.hitPoints -= mov.spCost;
                                break;

                            case MOVE_TYPE.SPECIAL:
                                move.user.skillPoints -= mov.spCost;
                                break;

                            case MOVE_TYPE.TALK:
                                if (isPlayerTurn)
                                    playerMoralePoints -= mov.spCost;
                                else
                                    enemyMoralePoints -= mov.spCost;
                                break;
                        }
                        StartCoroutine(PlayAniamtion(move.move.animation, move));
                    }
                }
            }
 
 
 */
