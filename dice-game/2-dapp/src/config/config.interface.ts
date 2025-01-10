import { TChainId } from '@aelf-web-login/wallet-adapter-base';

export interface IDiceGameConfig {
  DICE_CONTRACT_ADDRESS: string;
  TOKEN_CONTRACT_ADDRESS: string;
  CHAIN_ID: TChainId;
  EXPLORE_URL: string;
}
