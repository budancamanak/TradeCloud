import { useState, useEffect } from "react";
import $ from "jquery";
import AuthService from "../../services/Auth.Service";
import { ToastUtility } from "../../utils/toast-utility";

function SymbolList() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(true);
  const authService = new AuthService();

  // useEffect(() => {
  //   $("body").addClass("login-page");
  //   return () => {
  //     $("body").removeClass("login-page");
  //   };
  // }, []);

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
      <div>SYMBOL LIST</div>
    </>
  );
}

export default SymbolList;
