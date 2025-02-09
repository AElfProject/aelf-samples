import { ReactNode, MouseEventHandler } from "react";
import style from "./modal.module.scss";

interface PopupOverlayProps {
  children: ReactNode;
  isVisible: boolean;
  type?: string; // Adjust the type as per your requirements if type has specific values (e.g., a union of strings)
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

interface PopupInnerProps {
  children: ReactNode;
}

const PopupInner = ({ children }: PopupInnerProps) => (
  <div className={style.modalInner}>{children}</div>
);

interface PopupBodyProps {
  children: ReactNode;
}

const PopupBody = ({ children }: PopupBodyProps) => (
  <div className={style.modalBody}>{children}</div>
);

interface PopupCloseProps {
  onClick?: MouseEventHandler<HTMLDivElement>;
}

const PopupClose = ({ onClick }: PopupCloseProps) => (
  <div className={style["x-mark-wrapper"]} onClick={onClick}>
    <span className={style["x-mark"]}></span>
    <span className={style["x-mark"]}></span>
    <span className={style["mask"]}></span>
  </div>
);

interface PopupTitleProps {
  children: ReactNode;
}

const PopupTitle = ({ children }: PopupTitleProps) => (
  <h2 className={style.title}>{children}</h2>
);

interface ModalProps {
  isVisible: boolean;
  title: ReactNode;
  onClose: MouseEventHandler<HTMLDivElement>;
  children: ReactNode;
  type?: string; // Adjust the type as per your requirements
}

function Modal({ isVisible, title, onClose, children, type }: ModalProps) {
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
