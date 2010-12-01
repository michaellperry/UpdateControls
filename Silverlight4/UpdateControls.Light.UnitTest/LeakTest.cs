using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.XAML;
using UpdateControls.XAML.Wrapper;
using System.Windows.Data;

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
			Tournament tournament = new Tournament();
			Game firstSelectedGame = new Game();
			tournament.SelectedGame = firstSelectedGame;
			ObjectInstance<Tournament> tournamentWrapper = ForView.Wrap(tournament) as ObjectInstance<Tournament>;
			ObjectProperty selectedGameProperty = tournamentWrapper.GetPropertyByName("SelectedGame");
			ObjectInstance<Game> firstSelectedGameWrapper = tournamentWrapper.GetValue(selectedGameProperty.ClassProperty.DependencyProperty) as ObjectInstance<Game>;

			tournament.SelectedGame = firstSelectedGame;
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
    }
}