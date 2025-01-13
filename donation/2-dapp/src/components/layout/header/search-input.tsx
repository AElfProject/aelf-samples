import { Button } from "@/components/ui/button";
import { SearchIcon } from "@/components/ui/icons";
import { Input } from "@/components/ui/input";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const SearchInput = () => {
  const [value, setValue] = useState<string>("");
  const navigate = useNavigate();
  const searchValue = decodeURIComponent(location.search.replace("?search=", ""));

  const handleSearch = () => {
    navigate(value ? `?search=${value}` : "");
  };

  useEffect(() => {
    setValue(searchValue);
  }, [searchValue]);

  const onHandleChange = ({target: { value }}: {target: { value: string }}) => {
    setValue(value);
    if (!value) {
      navigate("/");
    }
  };

  return (
    <div className="search-container">
      <Input
        className="search-input"
        name="value"
        value={value}
        onChange={onHandleChange}
        onKeyDown={({ key }) => key === "Enter" && handleSearch()}
        placeholder="Search for campaigns"
      />
      <Button onClick={handleSearch}>
        <SearchIcon />
      </Button>
    </div>
  );
};

export default SearchInput;
