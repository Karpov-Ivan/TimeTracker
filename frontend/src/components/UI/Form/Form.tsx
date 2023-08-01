import styles from "./Form.module.css";

interface FormProps {
  children: React.ReactNode;
  onSubmit?: (e: React.FormEvent<HTMLFormElement>) => void;
}

const Form = ({ children, ...props }: FormProps) => {
  return (
    <form {...props} className={styles.form}>
      {children}
    </form>
  );
};

export default Form;
