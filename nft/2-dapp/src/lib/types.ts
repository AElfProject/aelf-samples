import { IChain } from "@portkey/provider-types";

export type IContract = ReturnType<IChain["getContract"]>;
