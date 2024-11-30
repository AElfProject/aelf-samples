import { NetworkEnum } from '@aelf-web-login/wallet-adapter-base';
import { IDiceGameConfig } from '@/config/config.interface';

/**
 * Testnet config
 */

// Step A - Configure Dice Contract
const DICE_CONTRACT_ADDRESS = 'add_your_deployed_dice_contract';
const TOKEN_CONTRACT_ADDRESS =
  'ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx';
const CHAIN_ID = 'tDVW';
const EXPLORE_URL = 'https://tdvw-test-node.aelf.io';

const DICE_GAME_CONFIG: IDiceGameConfig = {
  DICE_CONTRACT_ADDRESS,
  TOKEN_CONTRACT_ADDRESS,
  CHAIN_ID,
  EXPLORE_URL,
};

// Portkey
const NETWORK_TYPE = NetworkEnum.TESTNET;
const GRAPHQL_SERVER =
  'https://dapp-aa-portkey-test.portkey.finance/Portkey_DID/PortKeyIndexerCASchema/graphql';
const CONNECT_SERVER = 'https://auth-aa-portkey-test.portkey.finance';
const SERVICE_SERVER = 'https://aa-portkey-test.portkey.finance';
const RPC_SERVER_AELF = 'https://aelf-test-node.aelf.io';
const RPC_SERVER_TDVV = 'https://tdvv-public-node.aelf.io';
const RPC_SERVER_TDVW = 'https://tdvw-test-node.aelf.io';

const PORTKEY_CONFIG = {
  NETWORK_TYPE,
  GRAPHQL_SERVER,
  CONNECT_SERVER,
  SERVICE_SERVER,
  RPC_SERVER_AELF,
  RPC_SERVER_TDVV,
  RPC_SERVER_TDVW,
};

export { DICE_GAME_CONFIG, PORTKEY_CONFIG };
