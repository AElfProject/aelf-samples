/**
 * 1. https://testing-library.com/docs/queries/about#screen
 * 2. https://nextjs.org/docs/app/building-your-application/testing/jest
 */
import '@testing-library/jest-dom'
import { render, screen } from '@testing-library/react'
import Page from '../src/app/demos/jest/page';

describe('JestDemo', () => {
  it('renders hello', () => {
    render(<Page />)

    const hello = screen.getByText('Hello Jest');

    expect(hello).toBeInTheDocument();
  });
  // This case will fail. You can try your own.
  // it('renders hello error', () => {
  //   render(<Page />)
  //
  //   const helloError = screen.getByText('Hello Jest Error');
  //
  //   expect(helloError).toBeInTheDocument();
  // });
})
