import React from "react";
import styles from "./Modal.module.css";

interface ModalProps {
  visible: boolean;
  setVisible: React.Dispatch<React.SetStateAction<boolean>>;
  children?: React.ReactElement | React.ReactNode;
}

const Modal = ({ visible, setVisible, children }: ModalProps) => {
  const rootClasses = [styles.modal];
  if (visible) {
    rootClasses.push(styles.active);
  }

  return (
    <div className={rootClasses.join(" ")} onClick={() => setVisible(false)}>
      <div
        className={styles.modal_content}
        onClick={(e: React.MouseEvent<HTMLDivElement>) => e.stopPropagation()}
      >
        {children}
      </div>
    </div>
  );
};

export default Modal;
