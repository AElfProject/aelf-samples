# AElf Templates

A collection of templates smart contracts and dApps for the AElf blockchain platform.

## Installation

Install the templates using the .NET CLI:

```bash
dotnet new install AElf.Samples
```

## Available Templates

- `aelf-hello-world` - Hello World contract template
- `aelf-lottery` - Lottery Game contract template
- `aelf-simple-dao` - Simple DAO contract template
- `aelf-nft-sale` - NFT Sale contract template
- `aelf-dao` - BuildersDAO contract template
- `aelf-tictactoe` - TicTacToe game contract template
- `aelf-todo` - ToDo list contract template
- `aelf-expense` - Expense tracker contract template
- `aelf-staking` - Single pool staking contract template

## Usage

Create a new project using any template:

```bash
# Create a new Hello World project
dotnet new aelf-hello-world -n MyHelloWorld

# Create a new Lottery Game project
dotnet new aelf-lottery -n MyLottery

# Create a new Simple DAO project
dotnet new aelf-simple-dao -n MySimpleDAO

# Create a new NFT Sale project
dotnet new aelf-nft-sale -n MyNFTSale

# Create a new BuildersDAO project
dotnet new aelf-dao -n MyDAO

# Create a new TicTacToe project
dotnet new aelf-tictactoe -n MyGame

# Create a new ToDo project
dotnet new aelf-todo -n MyToDo

# Create a new Expense Tracker project
dotnet new aelf-expense -n MyExpenseTracker

# Create a new Staking project
dotnet new aelf-staking -n MyStaking
```

## Development

Each template contains:
- Smart contract source code
- Unit tests
- Proto files for contract interface definition
- Configuration files for contract deployment

## License

This project is licensed under the MIT License. 