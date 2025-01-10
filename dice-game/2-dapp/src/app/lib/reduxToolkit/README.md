# Redux Toolkit

1. https://redux-toolkit.js.org/usage/nextjs
2. https://redux.js.org/style-guide/

Any component that interacts with the Redux store 
(creating it, providing it, reading from it, or writing to it) needs to be a client component. 
This is because accessing the store requires React context, 
and context is only available in client components.
