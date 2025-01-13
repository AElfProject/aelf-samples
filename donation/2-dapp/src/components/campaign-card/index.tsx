import "./campaign-card.scss";
import { CAMPAIGN_CATEGORY_ICONS } from "@/lib/constant";
import { Button } from "../ui/button";
import { useNavigate } from "react-router-dom";
import { DonationCampaign } from "@/types/donation";
import { calculateTimeRemaining, convertTokenToAmount } from "@/lib/utils";

const formateString = (text: string, maxLength: number) =>
  !text
    ? text
    : text.length > maxLength
    ? text.slice(0, maxLength) + "..."
    : text;

const CampaignCard = ({
  data,
  isProfile,
  handleDelete,
  handleWithdraw,
}: {
  data: DonationCampaign;
  isProfile?: boolean;
  handleDelete?: (id: string) => void;
  handleWithdraw?: (id: string) => void;
}) => {
  const navigate = useNavigate();
  const remainingTime = calculateTimeRemaining(data.endTime);
  const isActiveCampaign =
    Math.floor(new Date().getTime() / 1000) < Number(data.endTime);
  const isGoalCompleted =
    Number(data.currentAmount) === Number(data.goalAmount);

  return (
    <div className={"campaign-card"}>
      <img
        src={data.imageUrl}
        alt="campaign-image"
        loading="lazy"
        onClick={() => navigate(`/${data.id}`)}
      />
      <div className="info">
        <div className="category">
          {CAMPAIGN_CATEGORY_ICONS[data.type]}
          <p>{data.type.replace("_", " ")}</p>
        </div>
        <p className="title">{formateString(data?.title, 35)}</p>
        <p className="desc">{formateString(data?.description, 45)}</p>
        <div className="donate-info">
          <div className="box">
            <strong>{convertTokenToAmount(data.currentAmount)} ELF</strong>
            <p>Raised of {convertTokenToAmount(data.goalAmount)} ELF</p>
          </div>
          {isActiveCampaign && !isGoalCompleted ? (
            <div className="box">
              <strong>{remainingTime.duration}</strong>
              <p>{remainingTime.formate} Left</p>
            </div>
          ) : (
            <div className="box comp-over">
              <strong>
                {isGoalCompleted ? "Goal Amount Over" : "Campaign Over"}
              </strong>
            </div>
          )}
        </div>
        {!isProfile ? (
          <>
            <div className="creator">
              <img
                src="/user-profile.svg"
                className="user-profile"
                alt="User Profile"
                title="Profile"
              />
              <p>{formateString(data.creator, 35)}</p>
            </div>
            <Button
              disabled={!isActiveCampaign}
              onClick={() => navigate(`/${data.id}`)}
            >
              {isActiveCampaign ? "Donate" : "Campaign Over"}
            </Button>
          </>
        ) : (
          <>
            <Button
              className="outline"
              disabled={!isActiveCampaign}
              onClick={() => navigate("/edit-campaign/" + data.id)}
            >
              Edit Campaign
            </Button>
            {handleDelete && (
              <Button
                className="delete-btn outline"
                disabled={!isActiveCampaign}
                onClick={() => handleDelete(data.id)}
              >
                Delete Campaign
              </Button>
            )}
            {handleWithdraw && (
              <Button
                className="outline"
                disabled={isActiveCampaign || data.isWithdrawn}
                onClick={() => handleWithdraw(data.id)}
              >
                {data.isWithdrawn
                  ? "Withdrawal Completed"
                  : "Withdraw Raised Amount"}
              </Button>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default CampaignCard;
