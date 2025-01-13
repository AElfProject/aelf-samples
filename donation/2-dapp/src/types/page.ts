import { IPortkeyProvider } from "@portkey/provider-types";

// Props type definition for the component
export interface PageProps {
  provider: IPortkeyProvider | null; // Blockchain provider instance
  currentWalletAddress?: string; // Wallet address of the current user
}
