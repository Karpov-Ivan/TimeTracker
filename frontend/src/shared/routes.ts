import {
  MAIN_ROUTE,
  PROJECTS_ROUTE,
  PROJECT_ROUTE,
  LOGIN_ROUTE,
} from "@/shared/consts";
import PageAuth from "@/pages/PageAuth";
import PageProject from "@/pages/PageProject";
import PageProjects from "@/pages/PageProjects";

export const authRoutes = [
  {
    path: PROJECTS_ROUTE,
    element: PageProjects,
  },
  {
    path: PROJECT_ROUTE,
    element: PageProject,
  },
  {
    path: MAIN_ROUTE,
    element: PageProjects,
  },
];

export const noAuthRoutes = [
  {
    path: LOGIN_ROUTE,
    element: PageAuth,
  },
  {
    path: MAIN_ROUTE,
    element: PageAuth,
  },
];
