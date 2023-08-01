export const MAIN_ROUTE: string = "/";
export const LOGIN_ROUTE: string = "/login";
export const PROJECTS_ROUTE: string = "/projects";
export const PROJECT_ROUTE: string = "/projects/:id";

export const IP = location.hostname;
//export const PORT = 5179;
export const PORT = 4450;
export const HOST_URL = "http://" + IP + ":" + PORT;

export const API_URL = "/api";
export const BACKEND_URL = HOST_URL + API_URL;

export enum AUTH_TYPE {
  REGULAR_AUTH = "regular",
  GITLAB_AUTH = "gitlab",
  REGISTRATION = "registration",
}

export const LIMIT = 7;

export const DAYS = 14;
export const WEEKS = 12;

export const MONTHS = {
  "01": "Январь",
  "02": "Февраль",
  "03": "Март",
  "04": "Апрель",
  "05": "Май",
  "06": "Июнь",
  "07": "Июль",
  "08": "Август",
  "09": "Сентябрь",
  "10": "Октябрь",
  "11": "Ноябрь",
  "12": "Декабрь",
};
