import { toTwoDigitNumber } from "@/lib/utils";

const Counter = ({
  timeDiff,
}: {
  timeDiff: {
    days: number;
    hours: number;
    minutes: number;
    seconds: number;
    difference: number;
  };
}) => {
  return (
    <div className="counter-container">
      <div className="counter-box">
        <p>{toTwoDigitNumber(timeDiff.days)}</p>
        <span>days</span>
      </div>
      <div className="counter-box">
        <p>{toTwoDigitNumber(timeDiff.hours)}</p>
        <span>hours</span>
      </div>
      <div className="counter-box">
        <p>{toTwoDigitNumber(timeDiff.minutes)}</p>
        <span>minutes</span>
      </div>
      <div className="counter-box">
        <p>{toTwoDigitNumber(timeDiff.seconds)}</p>
        <span>seconds</span>
      </div>
    </div>
  );
};

export default Counter;
