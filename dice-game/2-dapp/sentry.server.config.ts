// This file configures the initialization of Sentry on the server.
// The config you add here will be used whenever the server handles a request.
// https://docs.sentry.io/platforms/javascript/guides/nextjs/

import * as Sentry from "@sentry/nextjs";

Sentry.init({
  // dsn: 'https://8f885a00435b7b3402d6e7359c8435d2@o4507309896302592.ingest.us.sentry.io/4507309898727424',
  dsn: 'https://a05761160e2e11ba80888e9a44c71586@o4507309896302592.ingest.us.sentry.io/4507616574701568',

  // Adjust this value in production, or use tracesSampler for greater control
  tracesSampleRate: 1,

  // Setting this option to true will print useful information to the console while you're setting up Sentry.
  debug: false,

  // Uncomment the line below to enable Spotlight (https://spotlightjs.com)
  // spotlight: process.env.NODE_ENV === 'development',

});
