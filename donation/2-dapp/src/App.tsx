import { useState } from "react";
import { Routes, Route } from "react-router-dom";
import { IPortkeyProvider } from "@portkey/provider-types";
import { ToastContainer } from 'react-toastify';

import Header from "./components/layout/header";
import HomePage from "./pages/home";
import CreateCampaign from "./pages/create-campaign";
import CampaignDetails from "./pages/campaign-details";
import UserProfile from "./pages/user-profile";
import "./app.scss";
import 'react-toastify/dist/ReactToastify.css';

const App = () => {
  const [isConnected, setIsConnected] = useState(false);
  const [currentWalletAddress, setCurrentWalletAddress] = useState<string>();
  const [provider, setProvider] = useState<IPortkeyProvider | null>(null);

  return (
    <div className="app-layout">
      <ToastContainer />
      <Header
        isConnected={isConnected}
        currentWalletAddress={currentWalletAddress}
        setIsConnected={setIsConnected}
        setCurrentWalletAddress={setCurrentWalletAddress}
        setProvider={setProvider}
        provider={provider}
      />
      <Routes>
        <Route path="/" element={<HomePage provider={provider} currentWalletAddress={currentWalletAddress}/>} />
        <Route path="/create-campaign" element={<CreateCampaign provider={provider} currentWalletAddress={currentWalletAddress}/>} />
        <Route path="/edit-campaign/:id" element={<CreateCampaign provider={provider} currentWalletAddress={currentWalletAddress}/>} />
        <Route path="/:id" element={<CampaignDetails provider={provider} currentWalletAddress={currentWalletAddress}/>} />
        {currentWalletAddress && <Route path="/user-profile" element={<UserProfile provider={provider} currentWalletAddress={currentWalletAddress}/>} />}
      </Routes>
    </div>
  );
};

export default App;
