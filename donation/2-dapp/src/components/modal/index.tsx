import { ReactNode, MouseEventHandler } from "react";
import style from "./modal.module.scss";

interface PopupOverlayProps {
  children: ReactNode;
  isVisible: boolean;
  type?: string; // Adjust the type as per your requirements if type has specific values (e.g., a union of strings)
}

interface PopupInnerProps {
  children: ReactNode;
}

interface PopupBodyProps {
  children: ReactNode;
}

interface PopupCloseProps {
  onClick?: MouseEventHandler<HTMLDivElement>;
}

interface PopupTitleProps {
  children: ReactNode;
}

interface ModalProps {
  isVisible: boolean;
  title: ReactNode;
  onClose: MouseEventHandler<HTMLDivElement>;
  children: ReactNode;
  type?: string; // Adjust the type as per your requirements
}

const PopupOverlay = ({ children, isVisible, type }: PopupOverlayProps) => (
  <div
    className={`${style["modal-overlay"]} ${
      isVisible ? style["in-view"] : ""
    } ${style[`${type}`]}`}
  >
    {children}
  </div>
);

const PopupInner = ({ children }: PopupInnerProps) => (
  <div className={style.modalInner}>{children}</div>
);

const PopupBody = ({ children }: PopupBodyProps) => (
  <div className={style.modalBody}>{children}</div>
);

const PopupClose = ({ onClick }: PopupCloseProps) => (
  <div className={style["x-mark-wrapper"]} onClick={onClick}>
    <span className={style["x-mark"]}></span>
    <span className={style["x-mark"]}></span>
    <span className={style["mask"]}></span>
  </div>
);

const PopupTitle = ({ children }: PopupTitleProps) => (
  <h2 className={style.title}>{children}</h2>
);

function Modal(props: ModalProps) {
  const { isVisible, title, onClose, children, type } = props;
  return (
    <PopupOverlay isVisible={isVisible} type={type}>
      <PopupInner>
        <PopupClose onClick={onClose} />
        <PopupBody>
          <PopupTitle>{title}</PopupTitle>
          {children}
        </PopupBody>
      </PopupInner>
    </PopupOverlay>
  );
}

export default Modal;
