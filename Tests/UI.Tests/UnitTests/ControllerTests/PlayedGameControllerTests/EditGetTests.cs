using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class EditGetTests : PlayedGameControllerTestBase
	{
	    private EditPlayedGameViewModel _expectedEditPlayedGameViewModel;
	    private EditPlayedGameInfo _expectedEditPlayedGameInfo;

        [SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();

		    _expectedEditPlayedGameViewModel = new EditPlayedGameViewModel();

            AutoMocker.ClassUnderTest.Expect(mock =>
		            mock.MakeBaseCreatePlayedGameViewModel<EditPlayedGameViewModel>(Arg<ApplicationUser>.Is.Anything))
		        .Return(_expectedEditPlayedGameViewModel);

		    _expectedEditPlayedGameInfo = new EditPlayedGameInfo
		    {
                UserPlayer = new PlayerInfoForUser(),
                OtherPlayers = new List<PlayerInfoForUser>(),
                RecentPlayers = new List<PlayerInfoForUser>(),
		        DatePlayed = DateTime.UtcNow.Date,
		        Notes = "some notes",
		        GameDefinitionId = 3,
                GameDefinitionName = "some game definition name",
		        BoardGameGeekGameDefinitionId = 4,
                PlayerRanks = new List<PlayerRankWithName>()
            };
            AutoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetInfoForEditingPlayedGame(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
		        .Return(_expectedEditPlayedGameInfo);

            PlayerList = new List<Player> { new Player { Id = 42, Name = "Smitty Werbenjagermanjensen" } };
			PlayerSelectList = PlayerList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			GameDefinitionList = new List<GameDefinition> { new GameDefinition { Id = 1, Name = "Betrayal At The House On The Hill" } };
			GameDefinitionSelectList = GameDefinitionList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			ExpectedPopulatedCompletedGameViewModel = new PlayedGameEditViewModel { GameDefinitions = GameDefinitionSelectList, Players = PlayerSelectList };
		}

	    [Test]
	    public void It_Returns_The_Edit_View_Model_For_The_Specified_Played_Game()
	    {
	        //--arrange
	        int playedGameId = 1;

	        //--act
	        var result = AutoMocker.ClassUnderTest.Edit(playedGameId, CurrentUser) as ViewResult;

            //--assert
	        AutoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock =>
	            mock.GetInfoForEditingPlayedGame(Arg<int>.Is.Equal(playedGameId),
	                Arg<ApplicationUser>.Is.Same(CurrentUser)));

            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(MVC.PlayedGame.Views.CreateOrEdit);
	        var editPlayedGameViewModel = result.Model as EditPlayedGameViewModel;
	        editPlayedGameViewModel.EditMode.ShouldBe(true);
	        editPlayedGameViewModel.PlayedGameId.ShouldBe(playedGameId);
            editPlayedGameViewModel.ShouldNotBeNull();
	        editPlayedGameViewModel.OtherPlayers.ShouldBeSameAs(_expectedEditPlayedGameInfo.OtherPlayers);
	        editPlayedGameViewModel.RecentPlayers.ShouldBeSameAs(_expectedEditPlayedGameInfo.RecentPlayers);
	        editPlayedGameViewModel.UserPlayer.ShouldBeSameAs(_expectedEditPlayedGameInfo.UserPlayer);

            editPlayedGameViewModel.DatePlayed.ShouldBe(_expectedEditPlayedGameInfo.DatePlayed);
	        editPlayedGameViewModel.BoardGameGeekGameDefinitionId.ShouldBe(_expectedEditPlayedGameInfo.BoardGameGeekGameDefinitionId);
	        editPlayedGameViewModel.GameDefinitionId.ShouldBe(_expectedEditPlayedGameInfo.GameDefinitionId);
	        editPlayedGameViewModel.GameDefinitionName.ShouldBe(_expectedEditPlayedGameInfo.GameDefinitionName);
	        editPlayedGameViewModel.Notes.ShouldBe(_expectedEditPlayedGameInfo.Notes);
	        editPlayedGameViewModel.PlayerRanks.ShouldBeSameAs(_expectedEditPlayedGameInfo.PlayerRanks);
        }
	}
}