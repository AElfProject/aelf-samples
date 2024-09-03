import { useEffect, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";

import "./home.scss";
import useSmartContract from "@/hooks/useSmartContract"
import { Id, toast } from "react-toastify";
import { Button } from "@/components/ui/button";
import { IconO, IconX, renderIcon } from "@/components/ui/icons";
import { getIndexFromPosition, removeNotification } from "@/lib/utils";

interface PageProps {
  provider: IPortkeyProvider | null;
  currentWalletAddress?: string;
}

const HomePage = ({ provider, currentWalletAddress }: PageProps) => {
  const smartContract = useSmartContract(provider);
  const [isContractInitialized, setIsContractInitialized] =
    useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(true);
  const [isStarted, setIsStarted] = useState(false);
  const [board, setBoard] = useState(Array(9).fill(null));
  const [turnType, setTurnType] = useState("");
  const [winner, setWinner] = useState(null);
  const [moveLoadingIndex, setMoveLoadingIndex] = useState<number | null>(null);

  // step 1 - Check If Contract is Initialized or not 
  const checkIsContractInitialized = async () => {};

  // step 2 - Initialize the smart contract
  const initializeContract = async () => {};

  const changeTurn = () => {
    setMoveLoadingIndex(null);
    setTurnType((current:string) => (current === "X" ? "O" : "X"));
  };

  const getNextTurn = (board: ("X" | "O")[]) => {
    const xCount = board.filter((cell) => cell === "X").length;
    const oCount = board.filter((cell) => cell === "O").length;

    if (xCount > oCount) {
      setTurnType("O"); // O's turn since X has played more
    } else {
      setTurnType("X"); // X's turn since O has played fewer or equal moves
    }
  };

  // step 3 - Fetch Game Status function
  const getGameStatus = async (isFirstCheck?: boolean) => {};

  // step 4 - Fetch Latest Board Data from Contract 
  const getLatestBoard = async () => {};

  // step 5 - Start Game function
  const startGame = async () => {};

  // step 6 - Perform the Make Move Function 
  const makeMove = async (x: number, y: number) => {};

  // Check whether contract initialized or not
  useEffect(() => {
    smartContract && checkIsContractInitialized();
  }, [smartContract]);

  // Get Status of Game 
  useEffect(() => {
    if (currentWalletAddress) {
      getGameStatus(true);
      return;
    } else {
      setTimeout(() => setLoading(false), 2000);
    }
  }, [currentWalletAddress]);


  // ========= Conditional UI Component Start ========= //
  const SquareLoading = () => {
    return (
      <div className={"loading-inner-square"}>
        {turnType === "X" ? <IconX /> : <IconO />}
      </div>
    );
  };

  const renderSquare = (x: number, y: number, index: number) => {
    const isFilled = !!renderIcon(board[index]);
    return (
      <button
        className={`square ${isFilled ? "filled" : ""}`}
        onClick={() =>
          !isFilled && moveLoadingIndex !== index && makeMove(x, y)
        }
      >
        {moveLoadingIndex === index ? (
          <SquareLoading />
        ) : (
          renderIcon(board[index])
        )}
      </button>
    );
  };
  // ========= Conditional UI Component End ========= //

  return (
    <div className="home-container">
      <div className="tic-tac-toe-container">
        <div className="tic-tac-toe">
          {loading && <div>Loading....</div>}
          {winner && (
            <div className="winner-container">
              {winner === "X" ? <IconX /> : <IconO />}
              <h2>Congratulation Winner!</h2>
              <Button
                onClick={() => {
                  // set States in intitial value
                  setWinner(null);
                  setIsStarted(false);
                  setBoard(Array(9).fill(null));
                }}
              >
                Start Again
              </Button>
            </div>
          )}
          {!loading && !winner && !isStarted ? (
            <div className="getting-start">
              <h1>Welcome to Tic-Tac-Toe with aelf Blockchain</h1>
              <p>
                Enjoy the classic Tic-Tac-Toe game with secure,
                blockchain-powered gameplay on aelf. Make your move and watch as
                each step is recorded for transparency and fairness.
              </p>
              <div className="icons">
                <IconX />
                <IconO />
              </div>
              <Button onClick={startGame}>Start the Game</Button>
            </div>
          ) : (
            !loading &&
            !winner && (
              <div>
                <div className="tic-tac-head">
                  <div className={turnType === "X" ? "active" : ""}>
                    <IconX />
                  </div>
                  <div className={turnType === "O" ? "active" : ""}>
                    <IconO />
                  </div>
                </div>
                <div className={`board`}>
                  <div className="row">
                    {renderSquare(0, 0, 0)}
                    {renderSquare(0, 1, 1)}
                    {renderSquare(0, 2, 2)}
                  </div>
                  <div className="row">
                    {renderSquare(1, 0, 3)}
                    {renderSquare(1, 1, 4)}
                    {renderSquare(1, 2, 5)}
                  </div>
                  <div className="row">
                    {renderSquare(2, 0, 6)}
                    {renderSquare(2, 1, 7)}
                    {renderSquare(2, 2, 8)}
                  </div>
                </div>
              </div>
            )
          )}
        </div>
      </div>
    </div>
  );
};

export default HomePage;
