import { useState } from "react";
import Eye from "@/assets/eye.svg";
import EyeOff from "@/assets/eye-off.svg";
import styles from "./Input.module.css";

interface InputProps {
  id: string;
  name: string;
  type: string;
  label?: string;
  min?: string;
  placeholder?: string;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
  minlength?: string;
  required?: boolean;
}

const Input = ({ label, type, ...props }: InputProps) => {
  const [inputType, setInputType] = useState<string>("password");
  const [inputIcon, setInputIcon] = useState<string>(Eye);

  const handleClick = () => {
    setInputType(inputType === "text" ? "password" : "text");
    setInputIcon(inputType === "text" ? Eye : EyeOff);
  };

  return (
    <div className={styles.input__container}>
      {label && <label className={styles.input__label}>{label}</label>}
      <div className={styles.input__content}>
        <input
          className={styles.input}
          type={type === "password" ? inputType : type}
          {...props}
        />
        {type === "password" && (
          <span onClick={handleClick}>
            <img src={inputIcon} alt="eye" />
          </span>
        )}
      </div>
    </div>
  );
};

export default Input;
