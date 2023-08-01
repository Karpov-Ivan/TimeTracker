import Header from "@/components/Header";
import Button from "@/components/UI/Button";
import Form from "@/components/UI/Form";
import Input from "@/components/UI/Input";
import { useFormik } from "formik";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import TokenValidation from "@/components/TokenValidation";
import authService from "@/api/authService";
import { AUTH_TYPE } from "@/shared/consts";
import styles from "./PageAuth.module.css";
import { useUserContext } from "@/context/UserContext";

const PageAuth = () => {
  const [authType, setAuthType] = useState<AUTH_TYPE>(AUTH_TYPE.REGULAR_AUTH);
  const [visible, setVisible] = useState<boolean>(false);

  const { setIsSignedIn, isSignedIn, setUser } = useUserContext();

  useEffect(() => {
    const url = new URL(window.location.href);
    const code = url.searchParams.get("code");

    if (code)
      authService.getAccessToken(code).then(
        (result) => {
          if (result?.status === 200) {
            const url = new URL(window.location.href);
            url.searchParams.delete("code");
            setUser(result.data);
            setIsSignedIn(true);
          }
        },
        (error) => console.log(error)
      );
  }, []);

  const onSubmit = (values: any) => {
    switch (authType) {
      case AUTH_TYPE.REGULAR_AUTH:
        values.login &&
          values.password &&
          authService.signIn(values).then(
            (result) => {
              if (result?.status === 200) {
                setVisible(true);
              }
            },
            (error) => console.log(error)
          );
        break;
      case AUTH_TYPE.GITLAB_AUTH:
        authService.getLink().then(
          (result) => { 
            console.log(isSignedIn);
            if (result?.status === 200) {
              setIsSignedIn(true);
              console.log(isSignedIn, result.data);
              window.location.replace(result.data);
            }
          },
          (error) => console.log(error)
        );
        break;
      case AUTH_TYPE.REGISTRATION:
        values.login &&
          values.password &&
          authService.signUp(values).then(
            (result) => {
              if (result?.status === 200) {
                setVisible(true);
              }
            },
            (error) => console.log(error)
          );
        break;
    }
  };

  const formik = useFormik({
    initialValues: {
      login: "",
      password: "",
    },
    onSubmit,
  });

  return (
    <div>
      <Header title={"TimeTracker"} />
      <div className={styles.page_auth__container}>
        <div className={styles.page_auth__title}>TimeTracker</div>
        <div className={styles.page_auth__content}>
          <div className={styles.page_auth__content__description}>
            <span>Приложение, позволяющее отслеживать время в GitLab.</span>
            <Link
              to={"https://about.gitlab.com/"}
              style={{ textDecoration: "none" }}
            >
              <span className={styles.page_auth__content__description_light}>
                Подробнее о GitLab &#8594;
              </span>
            </Link>
          </div>
          <Form onSubmit={formik.handleSubmit}>
            <Input
              id="login"
              name="login"
              label="Имя пользователя"
              value={formik.values.login}
              onChange={formik.handleChange}
              type="text"
            />
            <Input
              id="password"
              name="password"
              label="Пароль"
              value={formik.values.password}
              onChange={formik.handleChange}
              type="password"
              minlength="8"
            />
            <div className={styles.page_auth__content__auth_row}>
              {authType === AUTH_TYPE.REGISTRATION ? (
                <div>
                  Уже есть аккаунт?{" "}
                  <span
                    onClick={() => setAuthType(AUTH_TYPE.REGULAR_AUTH)}
                    style={{ cursor: "pointer", color: "var(--primary-300)" }}
                  >
                    Войти
                  </span>
                </div>
              ) : (
                <div>
                  Нет аккаунта?{" "}
                  <span
                    onClick={() => setAuthType(AUTH_TYPE.REGISTRATION)}
                    style={{ cursor: "pointer", color: "var(--primary-300)" }}
                  >
                    Зарегистрироваться
                  </span>
                </div>
              )}
              {authType === AUTH_TYPE.REGISTRATION ? (
                <Button type="submit">Зарегистрироваться</Button>
              ) : (
                <div className={styles.page_auth__content__buttons}>
                  <Button type="submit">Войти</Button>
                  <Button
                    onClick={() => setAuthType(AUTH_TYPE.GITLAB_AUTH)}
                    type="submit"
                  >
                    Войти через GitLab
                  </Button>
                </div>
              )}
            </div>
          </Form>
        </div>
      </div>
      {visible && (
        <TokenValidation
          login={formik.values.login}
          visible={visible}
          setVisible={setVisible}
        />
      )}
    </div>
  );
};

export default PageAuth;
