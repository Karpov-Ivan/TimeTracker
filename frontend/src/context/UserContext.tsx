import { User } from "@/api/dto/user";
import { useContext, createContext } from "react";

interface UserContextType {
  isSignedIn: boolean;
  setIsSignedIn: (_: boolean) => void;
  user: User | undefined;
  setUser: (_: User | undefined) => void;
}

export const UserContext = createContext<UserContextType>({
  isSignedIn: false,
  setIsSignedIn: (_: boolean) => {},
  user: undefined,
  setUser: (_: User | undefined) => {},
});

export const useUserContext = () => {
  return useContext(UserContext);
};
