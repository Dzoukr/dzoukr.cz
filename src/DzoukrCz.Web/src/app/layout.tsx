import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Dzoukr.cz',
  description: 'Roman Provazník',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html>
      <body data-theme="dark" className="min-h-screen">
        {children}
      </body>
    </html>
  );
}

