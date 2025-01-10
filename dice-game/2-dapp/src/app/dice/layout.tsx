'use client';
import WebLoginProvider from '@/app/lib/webLogin/providers';

export default function Layout({
  children, // will be a page or nested layout
}: {
  children: React.ReactNode;
}) {
  return (
    <WebLoginProvider>
      {children}
    </WebLoginProvider>
  );
}
