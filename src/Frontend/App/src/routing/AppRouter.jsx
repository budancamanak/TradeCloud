import { Component } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
// import Layout from "../features/layout/Layout"
// import Login from "../features/user/Login"
// import Statistics from "../features/statistics/Statistics"
// import Consultation from "../features/consultation/Consultation"
// import ExternalConsultation from "../features/consultation/ExternalConsultation"
// import { Cookies } from "react-cookie";
import BaseLayout from "../layouts/BaseLayout";
import Login from "../features/auth/login/Login"
// import ForgotPassword from "../features/user/ForgotPassword"
// import ResetPassword from "../features/user/ResetPassword"
// import Deidentifies from "../features/deidentify/Deidentifies"
// import SlideViewer from "../features/slideviewer/SlideViewer"
// import ExternalSlidesView from "../features/consultation/ExternalSlides.view"

function AppRouter() {
  // const cookies = new Cookies();
  return (
    <>
      <Router>
        <Routes>
          <Route
            path="/"
            element={
              localStorage.getItem("access_token") ? (
                <BaseLayout                  
                />
              ) : (
                <Login />
              )
            }
          />

          <Route path="/login" element={<Login />} />
          <Route path="/forgot-password" element={<BaseLayout />} />
          <Route path="/password_reset/:token" element={<BaseLayout />} />
        </Routes>
      </Router>
    </>
  );
}
export default AppRouter;
