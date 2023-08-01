import { BACKEND_URL } from "@/shared/consts";
import axios from "axios";

export default class projectService {
  static url: string = "/projects";

  static async get(project_id: number, token: string) {
    try {
      const response = await axios.get(
        BACKEND_URL + this.url + `/${project_id}`,
        {
          withCredentials: true,
          headers: {
            token: token,
          },
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async getAll(page: number, token: string) {
    try {
      const response = await axios.get(
        BACKEND_URL + this.url + `?page=${page}`,
        {
          withCredentials: true,
          headers: {
            token: token,
          },
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async getComments(project_id: number, token: string) {
    try {
      const response = await axios.get(
        BACKEND_URL + this.url + `/${project_id}/comments`,
        {
          withCredentials: true,
          headers: {
            token: token,
          },
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }
}
