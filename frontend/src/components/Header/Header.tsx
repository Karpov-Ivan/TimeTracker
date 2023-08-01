import Logo from "@/assets/gitlab-logo.svg";
import { useUserContext } from "@/context/UserContext";
import { MAIN_ROUTE } from "@/shared/consts";
import { Link, useNavigate } from "react-router-dom";
import authService from "@/api/authService";
import styles from "./Header.module.css";

interface HeaderProps {
  title: string;
}

const Header = ({ title }: HeaderProps) => {
  const { isSignedIn, setIsSignedIn, setUser } = useUserContext();
  const navigate = useNavigate();

  const handleSignOut = () => {
    authService.signOut(undefined).then(
      (result) => {
        if (result?.status === 200) {
          setUser(undefined);
          setIsSignedIn(false);
          navigate(MAIN_ROUTE);
        }
      },
      (error) => console.log(error)
    );
  };

  return (
    <nav className={styles.header}>
      <div className={styles.header__content}>
        <div className={styles.header__content_left}>
          <Link to={MAIN_ROUTE}>
            <div className={styles.header__logo}>
              <img src={Logo} alt="logo" />
            </div>
          </Link>
          {title && <div>{title}</div>}
        </div>
        <div>
          {isSignedIn && (
            <div onClick={handleSignOut} style={{ cursor: "pointer" }}>
              {"Выйти"}
            </div>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Header;
