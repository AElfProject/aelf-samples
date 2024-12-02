import { createAppSlice } from '@/app/lib/reduxToolkit/createAppSlice';
import type { PayloadAction } from '@reduxjs/toolkit';
import { TWalletInfo } from '@aelf-web-login/wallet-adapter-base';

export interface IWalletConnectSliceState {
  value: TWalletInfo | null;
}

const initialState: IWalletConnectSliceState = {
  value: null,
};

// If you are not using async thunks you can use the standalone `createSlice`.
export const walletConnectSlice = createAppSlice({
  name: 'walletConnect',
  // `createSlice` will infer the state type from the `initialState` argument
  initialState,
  // The `reducers` field lets us define reducers and generate associated actions
  reducers: (create) => ({
    setWalletInfo: create.reducer(
      (state, action: PayloadAction<TWalletInfo>) => {
        state.value = action.payload;
      },
    ),
    deleteWalletInfo: create.reducer((state) => {
      state.value = null;
    }),
  }),
  // You can define your selectors here. These selectors receive the slice
  // state as their first argument.
  selectors: {
    getWalletInfo: (walletInfo) => walletInfo.value,
    getWalletName: (walletInfo) => walletInfo.value?.name,
    getWalletAddress: (walletInfo) => walletInfo.value?.address,
  },
});

// Action creators are generated for each case reducer function.
export const { setWalletInfo, deleteWalletInfo } = walletConnectSlice.actions;

// Selectors returned by `slice.selectors` take the root state as their first argument.
export const { getWalletInfo, getWalletName, getWalletAddress } =
  walletConnectSlice.selectors;
