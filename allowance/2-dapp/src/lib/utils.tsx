import { type ClassValue, clsx } from "clsx";
import { toast } from "react-toastify";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const removeNotification = (id: number | string, time?: number) => {
  setTimeout(() => toast.done(id), time || 3000);
};

export const CustomToast = ({ title, message }: any) => (
  <div>
    <h4>{title}</h4>
    <p>{message}</p>
  </div>
);
export const calculateTimeRemaining = (updatedAt: string) => {
  const nowTime = Math.floor(new Date().getTime() / 1000);
  const difference = nowTime - Number(updatedAt);

  const hours = Math.floor(difference / 3600);
  const minutes = Math.floor((difference % 3600) / 60);
  const seconds = difference % 60;

  if (hours > 0) {
    return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
  } else if (minutes > 0) {
    return `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
  } else {
    return `${seconds} second${seconds !== 1 ? 's' : ''} ago`;
  }
};


export const dateFormat = (date: string) => {
  const unixTimestamp = Number(date);
  const formattedDate = new Date(unixTimestamp * 1000).toLocaleString()
  return formattedDate
}
