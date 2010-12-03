using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.XAML;
using UpdateControls.XAML.Wrapper;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace UpdateControls.Light.UnitTest
{
    [TestClass]
	public class LeakTest
	{
		[TestMethod]
		public void CanWrapObject()
		{
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(new Tournament()) as ObjectInstance<Tournament>;

			Assert.IsNotNull(tournamentWrapper);
		}

		[TestMethod]
		public void CanAccessWrappedProperty()
		{
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(new Tournament()) as ObjectInstance<Tournament>;
			ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");

			Assert.IsNotNull(selectedGameProperty);
		}

		[TestMethod]
		public void PropertyValueIsAChildWrapper()
		{
			Tournament tournament = new Tournament();
			tournament.SelectedGame = new Game();
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
			ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
			ObjectInstance<Game> selectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			Assert.IsNotNull(selectedGameWrapper);
		}

		[TestMethod]
		public void WhenPropertyIsNotChanged_PropertyValueIsTheSameChildWrapper()
		{
            UnitTestDispatcher.On = true;

			Tournament tournament = new Tournament();
			Game firstSelectedGame = new Game();
			tournament.SelectedGame = firstSelectedGame;
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
			ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
			ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			tournament.SelectedGame = firstSelectedGame;
            UnitTestDispatcher.ExecuteAll();
			ObjectInstance<Game> secondSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			Assert.AreSame(firstSelectedGameWrapper, secondSelectedGameWrapper);
		}

		[TestMethod]
		public void WhenPropertyIsChanged_PropertyValueIsADifferentChildWrapper()
		{
            UnitTestDispatcher.On = true;

			Tournament tournament = new Tournament();
			Game firstSelectedGame = new Game();
			tournament.SelectedGame = firstSelectedGame;
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
			ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
			ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			Game secondSelectedGame = new Game();
			tournament.SelectedGame = secondSelectedGame;
            UnitTestDispatcher.ExecuteAll();
			ObjectInstance<Game> secondSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			Assert.AreNotSame(firstSelectedGameWrapper, secondSelectedGameWrapper);
		}

        [TestMethod]
        public void CanGetGrandchildProperty()
        {
            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;
            int firstScore = (int)firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Score").ClassProperty.DependencyProperty);

            Assert.AreEqual(42, firstScore);
        }

        [TestMethod]
        public void WhenGrandchildIsChanged_GrandchildIsUpdated()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;
            int firstScore = (int)firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Score").ClassProperty.DependencyProperty);

            firstSelectedGame.Score = 77;
            UnitTestDispatcher.ExecuteAll();
            int secondScore = (int)firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Score").ClassProperty.DependencyProperty);

            Assert.AreEqual(77, secondScore);
        }

        [TestMethod]
        public void WhenOldGrandchildIsChanged_GrandchildIsNotUpdated()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;
            int firstScore = (int)firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Score").ClassProperty.DependencyProperty);

            Game secondSelectedGame = new Game();
            tournament.SelectedGame = secondSelectedGame;
            UnitTestDispatcher.ExecuteAll();

            firstSelectedGame.Score = 77;
            UnitTestDispatcher.ExecuteAll();
            int secondScore = (int)firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Score").ClassProperty.DependencyProperty);

            Assert.AreEqual(42, secondScore);
        }

        [TestMethod]
        public void CanWrapCircularModels()
        {
            UnitTestDispatcher.On = true;

            Parent jenny = new Parent();
            Parent mike = new Parent();
            Child kaela = new Child();
            jenny.AddChild(kaela);
            mike.AddChild(kaela);
            kaela.Mom = jenny;
            kaela.Dad = mike;
            ForView.Wrap(jenny);

            bool finished = UnitTestDispatcher.ExecuteAll();
            Assert.IsTrue(finished);
        }

        [TestMethod]
        public void ChildIsNotDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

            Assert.IsFalse(firstSelectedGameWrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenChildIsNotChanged_ChildIsNotDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

            tournament.SelectedGame = firstSelectedGame;
            UnitTestDispatcher.ExecuteAll();

            Assert.IsFalse(firstSelectedGameWrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenChildIsChanged_OldChildIsDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.Score = 42;
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

            Game secondSelectedGame = new Game();
            secondSelectedGame.Score = 99;
            tournament.SelectedGame = secondSelectedGame;
            UnitTestDispatcher.ExecuteAll();

            Assert.IsTrue(firstSelectedGameWrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenChildIsChanged_OldGrandchildIsDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game firstSelectedGame = new Game();
            firstSelectedGame.NewMove();
            tournament.SelectedGame = firstSelectedGame;
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
            ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;
            ObservableCollection<object> moves = firstSelectedGameWrapper.GetValue(firstSelectedGameWrapper.GetPropertyByName("Moves").ClassProperty.DependencyProperty) as ObservableCollection<object>;
            UnitTestDispatcher.ExecuteAll();
            ObjectInstance<Move> moveWrapper = moves[0] as ObjectInstance<Move>;

            Assert.IsFalse(moveWrapper.IsDisposed);

            Game secondSelectedGame = new Game();
            tournament.SelectedGame = secondSelectedGame;
            UnitTestDispatcher.ExecuteAll();

            Assert.IsTrue(moveWrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenNewChildIsAdded_ChildInCollectionIsNotDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game game1 = tournament.NewGame();
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            UnitTestDispatcher.ExecuteAll();
            ObservableCollection<object> games = tournamentWrapper.GetValue(tournamentWrapper.GetPropertyByName("Games").ClassProperty.DependencyProperty) as ObservableCollection<object>;
            ObjectInstance<Game> game1Wrapper = games[0] as ObjectInstance<Game>;

            Game game2 = tournament.NewGame();
            UnitTestDispatcher.ExecuteAll();

            Assert.IsNotNull(game1Wrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenChildIsRemoved_ChildInCollectionIsDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game game1 = tournament.NewGame();
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            UnitTestDispatcher.ExecuteAll();
            ObservableCollection<object> games = tournamentWrapper.GetValue(tournamentWrapper.GetPropertyByName("Games").ClassProperty.DependencyProperty) as ObservableCollection<object>;
            ObjectInstance<Game> game1Wrapper = games[0] as ObjectInstance<Game>;

            tournament.DeleteGame(game1);
            UnitTestDispatcher.ExecuteAll();

            Assert.IsTrue(game1Wrapper.IsDisposed);
        }

        [TestMethod]
        public void WhenChildIsRemoved_GrandchildIsDisposed()
        {
            UnitTestDispatcher.On = true;

            Tournament tournament = new Tournament();
            Game game1 = tournament.NewGame();
            Move move = game1.NewMove();
            ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
            UnitTestDispatcher.ExecuteAll();
            ObservableCollection<object> games = tournamentWrapper.GetValue(tournamentWrapper.GetPropertyByName("Games").ClassProperty.DependencyProperty) as ObservableCollection<object>;
            ObjectInstance<Game> game1Wrapper = games[0] as ObjectInstance<Game>;
            ObservableCollection<object> moves = game1Wrapper.GetValue(game1Wrapper.GetPropertyByName("Moves").ClassProperty.DependencyProperty) as ObservableCollection<object>;
            ObjectInstance<Move> moveWrapper = moves[0] as ObjectInstance<Move>;

            tournament.DeleteGame(game1);
            UnitTestDispatcher.ExecuteAll();

            Assert.IsTrue(moveWrapper.IsDisposed);
        }
    }
}