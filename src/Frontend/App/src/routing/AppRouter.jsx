import { Component } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import BaseLayout from "../layouts/BaseLayout";
import Login from "../features/auth/login/Login";

function AppRouter() {
  return (
    <>
      <Router>
        <Routes>
          <Route
            path="/"
            element={
              localStorage.getItem("access_token") ? <BaseLayout /> : <Login />
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
