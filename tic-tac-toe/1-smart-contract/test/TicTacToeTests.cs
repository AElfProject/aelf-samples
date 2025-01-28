using System;
using System.Threading.Tasks;
using AElf.Types;
using AElf.ContractTestKit;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;
using AElf.Cryptography.ECDSA;
using AElf.Contracts.TicTacToe;
using AElf.ContractTestBase.ContractTestKit;

namespace TicTacToe.Tests
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class TicTacToeTests : TicTacToeTestBase
    {
        [Fact]
        public void InitializeContract_Success()
        {
            // Act
            var result = TicTacToeStub.Initialize.SendAsync(new Empty()).Result;
            var status = TicTacToeStub.GetInitialStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Value.ShouldBeTrue();
        }

        [Fact]
        public void InitializeContract_AlreadyInitialized()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();

            // Act
            var result = TicTacToeStub.Initialize.SendAsync(new Empty()).Result;
            var status = TicTacToeStub.GetInitialStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Value.ShouldBeTrue();
        }

        [Fact]
        public void StartGame_Success()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();

            // Act
            var result = TicTacToeStub.StartGame.SendAsync(new Empty()).Result;
            var board = TicTacToeStub.GetBoard.CallAsync(new Empty()).Result;
            var gameStatus = TicTacToeStub.GetGameStatus.CallAsync(new Empty()).Result;

            // Assert
            result.Output.Value.ShouldBe("Game started. Player X's turn.");
            board.Rows[0].ShouldBe(",,");
            board.Rows[1].ShouldBe(",,");
            board.Rows[2].ShouldBe(",,");
            gameStatus.Status.ShouldBe("ongoing");
            gameStatus.Winner.ShouldBe("");
        }

        [Fact]
        public void StartGame_NotInitialized()
        {
            // Act
            var result = TicTacToeStub.StartGame.SendAsync(new Empty()).Result;

            // Assert
            result.Output.Value.ShouldBe("Contract not initialized.");
        }

        [Fact]
        public void MakeMove_Success()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();

            // Act
            var moveInput = new MoveInput { X = 0, Y = 0 };
            var result = TicTacToeStub.MakeMove.SendAsync(moveInput).Result;
            var board = TicTacToeStub.GetBoard.CallAsync(new Empty()).Result;
            var gameStatus = TicTacToeStub.GetGameStatus.CallAsync(new Empty()).Result;

            // Assert
            result.Output.Value.ShouldBe("Player O's turn.");
            board.Rows[0].ShouldBe("X,,");
            board.Rows[1].ShouldBe(",,");
            board.Rows[2].ShouldBe(",,");
            gameStatus.Status.ShouldBe("ongoing");
            gameStatus.Winner.ShouldBe("");
        }

        [Fact]
        public void MakeMove_InvalidMove_CellOccupied()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();
            var moveInput = new MoveInput { X = 0, Y = 0 };
            TicTacToeStub.MakeMove.SendAsync(moveInput).Wait();

            // Act
            var result = TicTacToeStub.MakeMove.SendAsync(moveInput).Result;

            // Assert
            result.Output.Value.ShouldBe("Invalid move. Cell is already occupied.");
        }

        [Fact]
        public void MakeMove_GameNotOngoing()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();

            // Act
            var moveInput = new MoveInput { X = 0, Y = 0 };
            var result = TicTacToeStub.MakeMove.SendAsync(moveInput).Result;

            // Assert
            result.Output.Value.ShouldBe("Game is not ongoing.");
        }

        [Fact]
        public void MakeMove_WinningMove()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();

            // Make moves to set up a winning condition
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 0 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 1 }).Wait(); // O
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 1 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 2 }).Wait(); // O

            // Act - winning move for X
            var result = TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 2 }).Result;
            var gameStatus = TicTacToeStub.GetGameStatus.CallAsync(new Empty()).Result;

            // Assert
            result.Output.Value.ShouldBe("Player X wins!");
            gameStatus.Status.ShouldBe("finished");
            gameStatus.Winner.ShouldBe("X");
        }

        [Fact]
        public void GetBoard_EmptyBoard()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();

            // Act
            var board = TicTacToeStub.GetBoard.CallAsync(new Empty()).Result;

            // Assert
            board.Rows.Count.ShouldBe(3);
            board.Rows[0].ShouldBe(",,");
            board.Rows[1].ShouldBe(",,");
            board.Rows[2].ShouldBe(",,");
        }

        [Fact]
        public void GetBoard_WithMoves()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 0 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 1 }).Wait(); // O

            // Act
            var board = TicTacToeStub.GetBoard.CallAsync(new Empty()).Result;

            // Assert
            board.Rows.Count.ShouldBe(3);
            board.Rows[0].ShouldBe("X,,");
            board.Rows[1].ShouldBe(",O,");
            board.Rows[2].ShouldBe(",,");
        }

        [Fact]
        public void GetGameStatus_OngoingGame()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 0 }).Wait(); // X

            // Act
            var status = TicTacToeStub.GetGameStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Status.ShouldBe("ongoing");
            status.Winner.ShouldBe("");
        }

        [Fact]
        public void GetGameStatus_Draw()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();
            TicTacToeStub.StartGame.SendAsync(new Empty()).Wait();

            // Fill the board in a way that leads to a draw
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 0 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 1 }).Wait(); // O
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 0, Y = 2 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 1 }).Wait(); // O
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 0 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 1, Y = 2 }).Wait(); // O
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 2, Y = 1 }).Wait(); // X
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 2, Y = 0 }).Wait(); // O
            TicTacToeStub.MakeMove.SendAsync(new MoveInput { X = 2, Y = 2 }).Wait(); // X

            // Act
            var status = TicTacToeStub.GetGameStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Status.ShouldBe("draw");
            status.Winner.ShouldBe("");
        }

        [Fact]
        public void GetInitialStatus_NotInitialized()
        {
            // Act
            var status = TicTacToeStub.GetInitialStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Value.ShouldBeFalse();
        }

        [Fact]
        public void GetInitialStatus_AfterInitialization()
        {
            // Arrange
            TicTacToeStub.Initialize.SendAsync(new Empty()).Wait();

            // Act
            var status = TicTacToeStub.GetInitialStatus.CallAsync(new Empty()).Result;

            // Assert
            status.Value.ShouldBeTrue();
        }
    }
}