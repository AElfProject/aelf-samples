// eslint-disable-next-line @typescript-eslint/no-var-requires
// const nextJest = require('next/jest');
import nextJest from 'next/jest';
import type { Config } from 'jest';

/** @type {import('jest').Config} */
const createJestConfig = nextJest({
  // Provide the path to your Next.js app to load next.config.js and .env files in your test environment
  dir: './',
});

// Add any custom config to be passed to Jest
const config: Config = {
  coverageProvider: 'v8',
  collectCoverage: true,
  coverageDirectory: 'coverage',
  coverageReporters: ['json-summary', 'text'],
  testEnvironment: 'jsdom',
  // Add more setup options before each test is run
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
  moduleNameMapper: {
    // '^@/(.*)$': '<rootDir>/src/$1',
    '^@/(.*)$': '<rootDir>/src/$1',
    '^lodash-es$': 'lodash',
    '^swiper$': '<rootDir>/._tests__/__mocks__/swiper.js',
  },
  transformIgnorePatterns: [
    'node_modules/(?!(swiper)/)',
    'node_modules/.pnpm/(?!(swiper)/)',
  ],
};

// createJestConfig is exported this way to ensure that next/jest can load the Next.js config which is async
// module.exports = createJestConfig(config);
export default createJestConfig(config);
