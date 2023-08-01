import { MAIN_ROUTE } from "@/shared/consts";
import { authRoutes, noAuthRoutes } from "@/shared/routes";
import { Route, Routes, Navigate } from "react-router-dom";
import { useUserContext } from "@/context/UserContext";
import { useEffect } from "react";
import authService from "@/api/authService";

const AppRouter = () => {
  const { isSignedIn, setIsSignedIn, setUser } = useUserContext();

  useEffect(() => {
    authService.getMe().then(
      (result) => {
        if (result?.status === 200 && result.data.login) {
          setUser(result.data);
          setIsSignedIn(true);
        }
      },
      (error) => console.log(error)
    );
  }, []);

  return (
    <Routes>
      {!isSignedIn &&
        noAuthRoutes.map(({ path, element: Component }) => (
          <Route key={path} path={path} element={<Component />} />
        ))}
      {isSignedIn &&
        authRoutes.map(({ path, element: Component }) => (
          <Route key={path} path={path} element={<Component />} />
        ))}
      <Route path="*" element={<Navigate to={MAIN_ROUTE} />} />
    </Routes>
  );
};

export default AppRouter;
