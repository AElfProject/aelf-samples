// This file configures the initialization of Sentry for edge features (middleware, edge routes, and so on).
// The config you add here will be used whenever one of the edge features is loaded.
// Note that this config is unrelated to the Vercel Edge Runtime and is also required when running locally.
// https://docs.sentry.io/platforms/javascript/guides/nextjs/

import * as Sentry from "@sentry/nextjs";

Sentry.init({
  // dsn: 'https://8f885a00435b7b3402d6e7359c8435d2@o4507309896302592.ingest.us.sentry.io/4507309898727424',
  dsn: 'https://a05761160e2e11ba80888e9a44c71586@o4507309896302592.ingest.us.sentry.io/4507616574701568',

  // Adjust this value in production, or use tracesSampler for greater control
  tracesSampleRate: 1,

  // Setting this option to true will print useful information to the console while you're setting up Sentry.
  debug: false,

  // Make sure to set this to disable the automatic OpenTelemetry setup
  skipOpenTelemetrySetup: true,
});
