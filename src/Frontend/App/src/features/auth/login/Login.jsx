import { useState, useEffect } from "react";
import $ from "jquery";
import AuthService from "../../../services/Auth.Service";
import { ToastUtility } from "../../../utils/toast-utility";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(true);
  const authService = new AuthService();

  useEffect(() => {
    $("body").addClass("login-page");
    return () => {
      $("body").removeClass("login-page");
    };
  }, []);

  const loginAttempt = async (e) => {
    console.log("attempting login", email, password, rememberMe);
    e.preventDefault();
    var mr = await authService.login_user(email, password);
    if (mr.isSuccess) ToastUtility.login(mr.message);
    else ToastUtility.error(mr.message);
    console.log(mr);

    // mr = await fetcher.get("AvailableTickers");
    // console.log(mr);
  };

  return (
    <>
      <div className="login-box">
        <div className="card card-outline card-primary">
          <div className="card-header text-center">
            <a href="/" className="h1">
              <b>Trade</b>Cloud
            </a>
          </div>
          <div className="card-body">
            <p className="login-box-msg">Sign in to start your session</p>

            <form
              onSubmit={(e) => {
                loginAttempt(e);
              }}
              method="post"
            >
              <div className="input-group mb-3">
                <input
                  type="email"
                  className="form-control"
                  placeholder="Email"
                  onChange={(e) => setEmail(e.target.value)}
                />
                <div className="input-group-append">
                  <div className="input-group-text">
                    <span className="fas fa-envelope"></span>
                  </div>
                </div>
              </div>
              <div className="input-group mb-3">
                <input
                  type="password"
                  className="form-control"
                  placeholder="Password"
                  onChange={(e) => setPassword(e.target.value)}
                />
                <div className="input-group-append">
                  <div className="input-group-text">
                    <span className="fas fa-lock"></span>
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-8">
                  <div className="icheck-primary">
                    <input
                      type="checkbox"
                      id="remember"
                      defaultChecked={rememberMe}
                      defaultValue={rememberMe}
                      onChange={(e) => setRememberMe(!rememberMe)}
                    />
                    <label for="remember">Remember Me</label>
                  </div>
                </div>

                <div className="col-4">
                  <button type="submit" className="btn btn-primary btn-block">
                    Sign In
                  </button>
                </div>
              </div>
            </form>

            <div className="social-auth-links text-center mt-2 mb-3">
              {/* <a href="#" className="btn btn-block btn-primary">
                <i className="fab fa-facebook mr-2"></i> Sign in using Facebook
              </a>
              <a href="#" className="btn btn-block btn-danger">
                <i className="fab fa-google-plus mr-2"></i> Sign in using Google+
              </a> */}
            </div>

            <p className="mb-1">
              <a href="/forgot-password">I forgot my password</a>
            </p>
            <p className="mb-0">
              <a href="/register" className="text-center">
                Register a new membership
              </a>
            </p>
          </div>
        </div>
      </div>
    </>
  );
}

export default Login;
