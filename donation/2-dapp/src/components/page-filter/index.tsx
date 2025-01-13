import { CAMPAIGN_CATEGORY_ICONS, CAMPAIGN_FILTERS } from "@/lib/constant";
import "./page-filter.scss";

interface IPageFilter {
  selectedFilter: string;
  setSelectedFilter: (type: string) => void;
}

const PageFilter = ({ selectedFilter, setSelectedFilter }: IPageFilter) => {
  return (
    <div className="filter-wrapper">
      {CAMPAIGN_FILTERS.map((type, index) => {
        return (
          <span
            key={index}
            className={selectedFilter === type ? "active" : ""}
            onClick={() => {
              if (selectedFilter === type) {
                setSelectedFilter("");
              } else {
                setSelectedFilter(type);
              }
            }}
          >
            {CAMPAIGN_CATEGORY_ICONS[type]} {type}
          </span>
        );
      })}
    </div>
  );
};

export default PageFilter;
