# Template Intro

## folder structure

- /app: Contains all the routes, components, and logic for your application, this is where you'll be mostly working from.
- /app/lib: Contains functions used in your application, such as reusable utility functions and data fetching functions.
- /app/ui: Contains all the UI components for your application, such as cards, tables, and forms.
- /public: Contains all the static assets for your application, such as images.
- /scripts: Contains a seeding script that you'll use to populate your database in a later chapter. [We do not need use this by default]

- Config Files: You'll also notice config files such as next.config.js at the root of your application. 
Most of these files are created and pre-configured when you start a new project using create-next-app. 
You will not need to modify them in this course.

## Main features

### The <Image> component

The <Image> Component is an extension of the HTML <img> tag, and comes with automatic image optimization, such as:

- Preventing layout shift automatically when images are loading.
- Resizing images to avoid shipping large images to devices with a smaller viewport.
- Lazy loading images by default (images load as they enter the viewport).
- Serving images in modern formats, like WebP and AVIF, when the browser supports it.

### Navigating Between Pages

 <Link> allows you to do client-side navigation with JavaScript.

Furthermore, in production, whenever <Link> components appear in the browser's viewport, 
Next.js automatically prefetches the code for the linked route in the background.
