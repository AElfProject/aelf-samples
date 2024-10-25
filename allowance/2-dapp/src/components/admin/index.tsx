import { Fragment } from "react/jsx-runtime";
import { Button } from "../ui/button";
import { ROLE } from "@/lib/constant";

interface IProps {
  currentWalletAddress: string | undefined;
  role: {
    Admin: string;
    Parent: string;
    Child: string;
  };
  handleOpenModal: (type: string, isEdit?: boolean, address?: string) => void;
}
const AdminComponent = ({
  currentWalletAddress,
  role,
  handleOpenModal,
}: IProps) => {
  return (
    <Fragment>
      <h1>Admin</h1>
      <div className="allowance-head">
        <h2>Set Authority (Role)</h2>
        <div className="button-wrapper">
          <Button
            disabled={!!role.Admin || !currentWalletAddress}
            onClick={() => handleOpenModal(ROLE.admin)}
          >
            Set Admin
          </Button>
          <Button
            disabled={!!role.Parent || !currentWalletAddress}
            onClick={() => handleOpenModal(ROLE.parent)}
          >
            Set Parent
          </Button>
          <Button
            disabled={!!role.Child || !currentWalletAddress}
            onClick={() => handleOpenModal(ROLE.child)}
          >
            Set Child
          </Button>
        </div>
      </div>
      <div className="data-table-container">
        <h2>Authorities Data</h2>
        <div className="table-head">
          <p>Role</p>
          <p>Wallet Address</p>
        </div>
        <div className="table-body">
          <div className="table-data-row">
            <p>Admin</p>
            <p className="address">{role.Admin || "---"}</p>
          </div>
          <div className="table-data-row">
            <p>Parent</p>
            <p className="address">{role.Parent || "---"}</p>
            {role.Parent && (
              <Button
                onClick={() => handleOpenModal(ROLE.parent, true, role.Parent)}
              >
                Edit
              </Button>
            )}
          </div>
          <div className="table-data-row">
            <p>Child</p>
            <p className="address">{role.Child || "---"}</p>
            {role.Child && (
              <Button
                onClick={() => handleOpenModal(ROLE.admin, true, role.Child)}
              >
                Edit
              </Button>
            )}
          </div>
        </div>
      </div>
    </Fragment>
  );
};

export default AdminComponent;
