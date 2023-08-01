import Modal from "@/components/UI/Modal";
import Button from "@/components/UI/Button";
import { useState } from "react";
import { useUserContext } from "@/context/UserContext";
import Input from "@/components/UI/Input";
import authService from "@/api/authService";
import styles from "./TokenValidation.module.css";

interface TokenValidationProps {
  login: string;
  visible: boolean;
  setVisible: React.Dispatch<React.SetStateAction<boolean>>;
}

const TokenValidation = ({
  login,
  visible,
  setVisible,
}: TokenValidationProps) => {
  const { setIsSignedIn, user, setUser } = useUserContext();
  const [inputValue, setInputValue] = useState("");

  const handleSubmit = () => {
    authService.addToken({ login: login, token: inputValue }).then(
      (result) => {
        if (result?.status === 200) {
          setUser({
            login: user?.login || "",
            token: inputValue,
          });
          setIsSignedIn(true);
        }
      },
      (error) => console.log(error)
    );
  };

  return (
    <Modal visible={visible} setVisible={setVisible}>
      <div className={styles.token_validation__container}>
        <div className={styles.token_validation__title}>Валидация токена</div>
        <div className={styles.token_validation__description}>
          <span>Как создать свой токен?</span>
          <ol>
            <li>В правом верхнем углу выберите свой аватар.</li>
            <li>Выберите Edit profile.</li>
            <li>На левой боковой панели выберите Access Tokens.</li>
            <li>
              Введите название и дату истечения срока действия для токена.
            </li>
            <li>Выберите нужные области.</li>
            <li>Выберите Create personal access token.</li>
          </ol>
        </div>
        <Input
          id="token_validation"
          name="token_validation"
          type="text"
          value={inputValue}
          label="Токен"
          onChange={(e) => setInputValue(e.target.value)}
        />
        <div className={styles.token_validation__button}>
          <Button onClick={handleSubmit}>Ввести токен</Button>
        </div>
      </div>
    </Modal>
  );
};

export default TokenValidation;
