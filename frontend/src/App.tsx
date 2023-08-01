import AppRouter from "@/components/AppRouter";
import { UserContext } from "@/context/UserContext";
import { useState } from "react";
import { BrowserRouter } from "react-router-dom";
import { User } from "@/api/dto/user";

const App = () => {
  const [isSignedIn, setIsSignedIn] = useState<boolean>(false);
  const [user, setUser] = useState<User>();

  return (
    <>
      <UserContext.Provider
        value={{
          isSignedIn: isSignedIn,
          setIsSignedIn: setIsSignedIn,
          user: user,
          setUser: setUser,
        }}
      >
        <BrowserRouter>
          <AppRouter />
        </BrowserRouter>
      </UserContext.Provider>
    </>
  );
};

export default App;
