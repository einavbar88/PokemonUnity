using System;
public class FightMove
{
    public BaseFightMoves Base { get; set; }
    public int PP { get; set; }

    public FightMove(BaseFightMoves baseFightMoves)
    {
        Base = baseFightMoves;
        PP = baseFightMoves.PP;
    }
} 
