import { Animal, Art, Bank, Community, DisasterRelief, Education, Environment, Game, Healthcare, Home, Orphanage, Research, Sports, Technology } from "@/components/ui/icons";

export const CATEGORY_OPTIONS = [
  { value: "education", label: "Education" },
  { value: "orphanage", label: "Orphanage" },
  { value: "oldage_home", label: "Old Age Home" },
  { value: "streamer", label: "Streamer" },
  { value: "healthcare", label: "Healthcare" },
  { value: "environment", label: "Environment" },
  { value: "animal_welfare", label: "Animal Welfare" },
  { value: "disaster_relief", label: "Disaster Relief" },
  { value: "sports", label: "Sports" },
  { value: "cultural_heritage", label: "Cultural Heritage" },
  { value: "community_development", label: "Community Development" },
  { value: "science_research", label: "Science & Research" },
  { value: "arts_entertainment", label: "Arts & Entertainment" },
  { value: "technology", label: "Technology" },
];

export const CAMPAIGN_TYPE = {
  education: "education",
  orphanage: "orphanage",
  oldage_home: "oldage_home",
  streamer: "streamer",
  healthcare: "healthcare",
  environment: "environment",
  animal_welfare: "animal_welfare",
  disaster_relief: "disaster_relief",
  sports: "sports",
  cultural_heritage: "cultural _eritage",
  community_development: "community_development",
  science_research: "science_research",
  arts_entertainment: "arts_entertainment",
  technology: "technology",
};

export const CAMPAIGN_FILTERS = [
  CAMPAIGN_TYPE.education,
  CAMPAIGN_TYPE.oldage_home,
  CAMPAIGN_TYPE.orphanage,
  CAMPAIGN_TYPE.streamer,
  CAMPAIGN_TYPE.healthcare,
  CAMPAIGN_TYPE.environment,
  CAMPAIGN_TYPE.animal_welfare,
  CAMPAIGN_TYPE.disaster_relief,
  CAMPAIGN_TYPE.sports,
  CAMPAIGN_TYPE.cultural_heritage,
  CAMPAIGN_TYPE.community_development,
  CAMPAIGN_TYPE.science_research,
  CAMPAIGN_TYPE.arts_entertainment,
  CAMPAIGN_TYPE.technology,
];

export const CAMPAIGN_CATEGORY_ICONS = {
  [CAMPAIGN_TYPE.education]: <Education />,
  [CAMPAIGN_TYPE.oldage_home]: <Home />,
  [CAMPAIGN_TYPE.orphanage]: <Orphanage />,
  [CAMPAIGN_TYPE.streamer]: <Game />,
  [CAMPAIGN_TYPE.healthcare]: <Healthcare />,
  [CAMPAIGN_TYPE.environment]: <Environment />,
  [CAMPAIGN_TYPE.animal_welfare]: <Animal />,
  [CAMPAIGN_TYPE.disaster_relief]: <DisasterRelief />,
  [CAMPAIGN_TYPE.sports]: <Sports />,
  [CAMPAIGN_TYPE.cultural_heritage]: <Bank />,
  [CAMPAIGN_TYPE.community_development]: <Community />,
  [CAMPAIGN_TYPE.science_research]: <Research />,
  [CAMPAIGN_TYPE.arts_entertainment]: <Art />,
  [CAMPAIGN_TYPE.technology]: <Technology />,
};