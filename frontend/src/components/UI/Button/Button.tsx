import styles from "./Button.module.css";

interface ButtonProps {
  children: React.ReactNode;
  className?: string;
  onClick?: (e: React.MouseEvent<HTMLButtonElement>) => void;
  type?: React.ButtonHTMLAttributes<HTMLButtonElement>["type"];
}

const Button = ({ children, className, ...props }: ButtonProps) => {
  const style =
    className !== undefined
      ? `${styles.button} ${styles[className]}`
      : `${styles.button}`;

  return (
    <button {...props} className={style}>
      {children}
    </button>
  );
};

export default Button;
