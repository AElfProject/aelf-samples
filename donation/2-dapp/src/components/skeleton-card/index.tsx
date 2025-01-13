import "./skeleton-card.scss";

const SkeletonCard = () => {
  return (
    <div className="skeleton-card">
      <div className="image-box skeleton"></div>
      <div className="info">
        <div className="category skeleton"></div>
        <p className="title skeleton"></p>
        <p className="desc skeleton"></p>
        <div className="donate-info">
          <div className="box box-1 skeleton"></div>
          <div className="box box-2 skeleton"></div>
        </div>
        <div className="creator">
          <div className="image-circle skeleton"></div>
          <p className="skeleton"></p>
        </div>
        <div className="button skeleton"></div>
      </div>
    </div>
  );
};

export default SkeletonCard;
