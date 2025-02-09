import { type ClassValue, clsx } from "clsx";
import { toast } from "react-toastify";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const removeNotification = (id: number | string, time?: number) => {
  setTimeout(() => toast.done(id), time || 3000);
};

export const CustomToast = ({ title, message }: {title: string; message:string }) => (
  <div>
    <h4>{title}</h4>
    <p>{message}</p>
  </div>
);

// Get Index for the Board from 0 to 8 using x and y
export const getIndexFromPosition = (x: number, y: number) => {
  if (x === 0 && y === 0) {
    return 0;
  } else if (x === 0 && y === 1) {
    return 1;
  } else if (x === 0 && y === 2) {
    return 2;
  } else if (x === 1 && y === 0) {
    return 3;
  } else if (x === 1 && y === 1) {
    return 4;
  } else if (x === 1 && y === 2) {
    return 5;
  } else if (x === 2 && y === 0) {
    return 6;
  } else if (x === 2 && y === 1) {
    return 7;
  } else if (x === 2 && y === 2) {
    return 8;
  }
};
