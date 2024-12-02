module.exports = {
    apps: [
      {
        name: "create-aelf-dapp",
        script: "server.js",
        instances: "2",
        exec_mode: "cluster",
        env: {
          NODE_ENV: process.env.NODE_ENV || "production",
          PORT: process.env.PORT || 3005
        }
      }
    ]
  };
