import { type ClassValue, clsx } from "clsx";
import { toast } from "react-toastify";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const removeNotification = (id: number | string, time?: number) => {
  setTimeout(() => toast.done(id), time || 3000);
};

export const handleError = (loadingId: number, error: unknown) => {
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

export const delay = (ms: number) =>
  new Promise((resolve) => setTimeout(resolve, ms));

export const CustomToast = ({
  title,
  message,
}: {
  title: string;
  message: string;
}) => (
  <div>
    <h4>{title}</h4>
    <p>{message}</p>
  </div>
);
