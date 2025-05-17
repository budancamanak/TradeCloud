import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import AppRouter from "./routing/AppRouter.jsx";
import { ToastContainer } from "react-toastify";
import './index.css'

createRoot(document.getElementById("root")).render(
  <>
    <AppRouter />
    <ToastContainer />
  </>
);
