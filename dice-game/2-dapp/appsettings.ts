export const APP_SETTINGS: IAppSettings = {
  "openTelemetry": {
    "serviceName": "create-aelf-dapp",
    "serviceVersion": "v1.0",
    // https://opentelemetry.io/docs/languages/sdk-configuration/otlp-exporter/
    "collectorEndpoint": "/v1/traces",
    "tracerName": "create-aelf-dapp-tracer",
    ignoreUrls: [
      /localhost:8093\/sockjs-node/,
      /localhost:8093\/monitoring/,
      /localhost:8093\/__nextjs_original-stack-frame/,
    ],
    propagateTraceHeaderCorsUrls: [
      // /.*/,
      /gcptest-indexer-api\.aefinder\.io/,
      // "https://httpbin.org",
      // "https://otel.aelf.com/v1/traces",
      // "gcptest-indexer-api.aefinder.io/*"
    ],
  },
  "chainGPT": {
    // For vercel: key, secret, etc. get from .env
    "apiKey": process.env.CHAINGPT_API_KEY as string,
  },
  "sentry": {
    // For vercel: key, secret, etc. get from .env
    "dsn": {
      "server": process.env.SENTRY_SERVER_DSN as string,
      "edge": process.env.SENTRY_EDGE_DSN as string,
      "client": process.env.SENTRY_CLIENT_DSN as string
    }
  }
};

export interface IAppSettings {
  openTelemetry: {
    serviceName: string;
    serviceVersion: string;
    collectorEndpoint: string;
    tracerName: string;
    ignoreUrls: RegExp[];
    propagateTraceHeaderCorsUrls: (string | RegExp)[];
  };
  chainGPT: {
    apiKey: string;
  };
  sentry: {
    dsn: {
      server: string;
      edge: string;
      client: string;
    };
  };
}
