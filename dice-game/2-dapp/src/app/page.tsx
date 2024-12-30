import { redirect } from 'next/navigation';

export default function Home() {
  redirect('/dice');
  return null;
}
