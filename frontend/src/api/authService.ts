import { BACKEND_URL } from "@/shared/consts";
import axios from "axios";

export default class authService {
  static url: string = "/auth";

  static async getMe() {
    try {
      const response = await axios.get(BACKEND_URL + this.url, {
        withCredentials: true,
      });
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async getAccessToken(code: string) {
    try {
      const response = await axios.get(BACKEND_URL + this.url + `/access_token?code=${code}`, {
        withCredentials: true,
      });
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async getLink() {
    try {
      const response = await axios.get(BACKEND_URL + this.url + "/get_link", {
        withCredentials: true,
      });
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async signUp(values: any) {
    try {
      const response = await axios.post(
        BACKEND_URL + this.url + `/sign_up`,
        values,
        {
          withCredentials: true,
          xsrfCookieName: "csrf_",
          xsrfHeaderName: "X-Csrf-Token",
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async signIn(values: any) {
    try {
      const response = await axios.post(
        BACKEND_URL + this.url + `/sign_in`,
        values,
        {
          withCredentials: true,
          xsrfCookieName: "csrf_",
          xsrfHeaderName: "X-Csrf-Token",
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async signOut(values: any) {
    try {
      const response = await axios.post(
        BACKEND_URL + this.url + "/sign_out",
        values,
        {
          withCredentials: true,
          xsrfCookieName: "csrf_",
          xsrfHeaderName: "X-Csrf-Token",
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }

  static async addToken(values: any) {
    try {
      const response = await axios.post(
        BACKEND_URL +
          this.url +
          `/add_token?login=${values.login}`,
        values,
        {
          withCredentials: true,
          xsrfCookieName: "csrf_",
          xsrfHeaderName: "X-Csrf-Token",
          headers: {
            token: values.token,
          },
        }
      );
      return response;
    } catch (err) {
      console.log(err);
    }
  }
}
