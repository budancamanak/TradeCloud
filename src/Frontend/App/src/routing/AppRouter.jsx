import { Component } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import BaseLayout from "../layouts/BaseLayout";
import Login from "../features/auth/login/Login";
import SymbolList from "../features/symbols/SymbolList";
import TrackList from "../features/symbols/TrackList";

function AppRouter() {
  const logged_in = !!localStorage.getItem("access_token");
  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={logged_in ? <BaseLayout /> : <Login />}>
            <Route
              path="/symbols"
              element={logged_in ? <SymbolList /> : <Login />}
            />
            <Route
              path="/tracklist"
              element={logged_in ? <TrackList /> : <Login />}
            />
          </Route>
          <Route
            path="/login"
            element={!logged_in ? <Login /> : <BaseLayout />}
          />
          <Route path="/forgot-password" element={<BaseLayout />} />
          <Route path="/password_reset/:token" element={<BaseLayout />} />
        </Routes>
      </Router>
    </>
  );
}
export default AppRouter;
