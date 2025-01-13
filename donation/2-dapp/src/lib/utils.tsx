import { type ClassValue, clsx } from "clsx";
import { Id, toast } from "react-toastify";
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

export const calculateTimeRemaining = (endDate: string) => {
  const nowTime = Math.floor(new Date().getTime() / 1000);
  const difference = Number(endDate) - nowTime;

  const days = Math.floor(difference / (3600 * 24));
  const hours = Math.floor((difference % (3600 * 24)) / 3600);
  const minutes = Math.floor((difference % 3600) / 60);
  const seconds = difference % 60;

  if (days > 0) {
    return { duration: days, formate: `Day${days !== 1 ? "s" : ""}` };
  } else if (hours > 0) {
    return { duration: hours, formate: `Hour${hours !== 1 ? "s" : ""}` };
  } else if (minutes > 0) {
    return { duration: minutes, formate: `Minute${minutes !== 1 ? "s" : ""}` };
  } else {
    return { duration: seconds, formate: `Second${seconds !== 1 ? "s" : ""}` };
  }
};

export const remainingCounter = (endDate: string) => {
  const nowTime = Math.floor(new Date().getTime() / 1000);
  const difference = Number(endDate) - nowTime;
  if (difference <= 0) {
    return { days: 0, hours: 0, minutes: 0, seconds: 0, difference: 0 };
  }
  const days = Math.floor(difference / (3600 * 24));
  const hours = Math.floor((difference % (3600 * 24)) / 3600);
  const minutes = Math.floor((difference % 3600) / 60);
  const seconds = difference % 60;

  return { days, hours, minutes, seconds, difference };
};

export const dateFormat = (date: string) => {
  const unixTimestamp = Number(date);
  const formattedDate = new Date(unixTimestamp * 1000).toLocaleString();
  return formattedDate;
};

export const getCurrentDate = () => {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0"); // Add leading zero
  const day = String(today.getDate()).padStart(2, "0"); // Add leading zero
  return `${year}-${month}-${day}`;
};

export const handleError = (loadingId: Id, error: unknown) => {
  if (error instanceof Error) {
    toast.update(loadingId, {
      render: error.message,
      type: "error",
      isLoading: false,
    });
  } else {
    toast.update(loadingId, {
      render: "An unexpected error occurred.",
      type: "error",
      isLoading: false,
    });
  }
  removeNotification(loadingId);
};

export const convertTokenToAmount = (amount: number | string): number => Number(amount) / 100000000;
export const convertAmountToToken = (amount: number | string): number => Number(amount) * 100000000;

export const toTwoDigitNumber = (number: string | number) => {
  return Number(number) < 10 ? `0${number}` : `${number}`;
}