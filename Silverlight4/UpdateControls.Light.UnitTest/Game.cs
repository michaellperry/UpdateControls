using System;
using System.Collections.Generic;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace UpdateControls.Light.UnitTest
{
    public class Game
	{
        private Independent<int> _score = new Independent<int>();
        private IndependentList<Move> _moves = new IndependentList<Move>();

        public int Score
        {
            get { return _score; }
            set { _score.Value = value; }
        }

        public Move NewMove()
        {
            Move move = new Move();
            _moves.Add(move);
            return move;
        }

        public void DeleteMove(Move move)
        {
            _moves.Remove(move);
        }

        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
	}
}
