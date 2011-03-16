using System;
using UpdateControls;
using System.Collections.Generic;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace UpdateControls.Light.UnitTest
{
    public class Tournament
	{
        private IndependentList<Game> _games = new IndependentList<Game>();
		private Independent<Game> _selectedGame = new Independent<Game>();

		public Game SelectedGame
		{
			get { return _selectedGame; }
			set { _selectedGame.Value = value; }
		}

        public Game NewGame()
        {
            Game game = new Game();
            _games.Add(game);
            return game;
        }

        public void DeleteGame(Game game)
        {
            _games.Remove(game);
        }

        public IEnumerable<Game> Games
        {
            get { return _games; }
        }
	}
}
