// This file configures the initialization of Sentry on the client.
// The config you add here will be used whenever a users loads a page in their browser.
// https://docs.sentry.io/platforms/javascript/guides/nextjs/

import * as Sentry from "@sentry/nextjs";

Sentry.init({
  // dsn: 'https://8f885a00435b7b3402d6e7359c8435d2@o4507309896302592.ingest.us.sentry.io/4507309898727424',
  dsn: 'https://a05761160e2e11ba80888e9a44c71586@o4507309896302592.ingest.us.sentry.io/4507616574701568',

  // Adjust this value in production, or use tracesSampler for greater control
  tracesSampleRate: 1,

  // Setting this option to true will print useful information to the console while you're setting up Sentry.
  debug: false,

  replaysOnErrorSampleRate: 1.0,

  // This sets the sample rate to be 10%. You may want this to be 100% while
  // in development and sample at a lower rate in production
  replaysSessionSampleRate: 0.1,

  // You can remove this option if you're not planning to use the Sentry Session Replay feature:
  integrations: [
    Sentry.replayIntegration({
      // Additional Replay configuration goes in here, for example:
      maskAllText: true,
      blockAllMedia: true,
    }),
    Sentry.browserTracingIntegration()
  ],
  // https://docs.sentry.io/platforms/javascript/guides/nextjs/tracing/instrumentation/automatic-instrumentation/
  tracePropagationTargets: [
    "localhost",
    "httpbin.org",
    /^\//,
    /^https:\/\/yourserver\.io\/api/,
    /^https:\/\/httpbin\.org\/get/,
  ],
});
