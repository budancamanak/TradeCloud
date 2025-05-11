import Fetcher from "../utils/network";

class AuthService {
  constructor() {
    this.fetcher = new Fetcher();
  }

  login_user = async (email, password) => {
    var mr = await this.fetcher.post("Login", {
      email: email,
      password: password,
    });
    if (mr.isSuccess) {
      // todo set user in auth context
      localStorage.setItem("access_token", mr.data);
      setTimeout(function () {
        window.location.href = "/";
      }, 1200);
    }
    return mr;
  };

  logout_user = () => {
    localStorage.removeItem("access_token");
    window.location.href = "/";
  };
}

export default AuthService;
