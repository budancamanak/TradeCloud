export function login(email, password, finishCallback) {
  let fd = new FormData();
  fd.append("email", email);
  fd.append("password", password);

  const data = {
    email: email,
    password: password,
  };

  const BASE_URL = "http://192.168.1.10:80";
  $.ajax(BASE_URL + "/Login", {
    method: "POST",
    data: JSON.stringify(data),
    cache: false,
    contentType: "application/json",
    processData: false,
    async: false,
    success: function (result) {
      if (result.isSuccess) {
        localStorage.setItem("access_token",result.data);
        userTrackList();
        // cookies.set("accessToken", "jwt " + result.access_token);
        // cookies.set("aibackendToken", result.aibackend_token);
        // ToastUtility.login("Logged in successfully, redirecting..");
        // let redir = result.redirect_to;
        let skip_finish = !!redir;
        if (!redir) {
          redir = "/";
          skip_finish = false;
        }
        setTimeout(function () {
          window.location.href = redir;
        }, 2000);
        if (finishCallback && !skip_finish) finishCallback(result);
      } else {
        console.log(result);
        // ToastUtility.error("Failed to login.");
      }
    },
    error: function (result) {
      console.error(result.responseJSON.description);
      // ToastUtility.error(result.responseJSON.description);
      // document.getElementById("emailField").style.border = "1px solid #df2020";
      // document.getElementById("passwordField").style.border =
      //   "1px solid #df2020";
    },
  });
}

export function userTrackList() {
  const BASE_URL = "http://192.168.1.10:80";
  $.ajax(BASE_URL + "/AvailableTickers", {
    method: "GET",
    headers: {
      Authorization: localStorage.getItem("accessToken"),
    },
    contentType: "application/json",
    async: false,
    success: function (result) {
      console.log("UserTrackList>>", result);
    },
    error: function (result) {
      console.error(result);
    },
  });
}
