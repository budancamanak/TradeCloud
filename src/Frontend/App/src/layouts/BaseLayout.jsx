import { useState } from "react";
import Navbar from "./Navbar.jsx";
import Sidebar from "./Sidebar.jsx";
import AppLogo from "../assets/img/AdminLTELogo.png";
import UserLogo from "../assets/img/user3-128x128.jpg";
import { Outlet } from "react-router-dom";

function BaseLayout() {
  return (
    <>
    <div className="wrapper">
      <div className="preloader flex-column justify-content-center align-items-center">
        <img
          className="animation__shake"
          src={AppLogo}
          alt="Hold on tight..."
          height="60"
          width="60"
        />
      </div>
      <Navbar></Navbar>

      <aside className="main-sidebar sidebar-dark-primary elevation-4">
        <a href="index.html" className="brand-link">
          <img
            src={AppLogo}
            alt="AdminLTE Logo3"
            className="brand-image img-circle elevation-3"
            style={{ opacity: 0.8 }}
          />
          <span className="brand-text font-weight-light">Trade Cloud</span>
        </a>

        <div className="sidebar">
          <div className="user-panel mt-3 pb-3 mb-3 d-flex">
            <div className="image">
              <img
                src={UserLogo}
                className="img-circle elevation-2"
                alt="User Image"
              />
            </div>
            <div className="info">
              <a href="#" className="d-block">
                John Doe
              </a>
            </div>
          </div>

          <div className="form-inline">
            <div className="input-group" data-widget="sidebar-search">
              <input
                className="form-control form-control-sidebar"
                type="search"
                placeholder="Search"
                aria-label="Search"
              />
              <div className="input-group-append">
                <button className="btn btn-sidebar">
                  <i className="fas fa-search fa-fw"></i>
                </button>
              </div>
            </div>
          </div>
          <Sidebar />
        </div>
      </aside>

      <div className="content-wrapper">
        <div className="content-header">
          <div className="container-fluid">
            <div className="row mb-2">
              <div className="col-sm-6">
                <h1 className="m-0">Dashboard</h1>
              </div>
              <div className="col-sm-6">
                <ol className="breadcrumb float-sm-right">
                  <li className="breadcrumb-item">
                    <a href="#">Home</a>
                  </li>
                  <li className="breadcrumb-item active">Dashboard</li>
                </ol>
              </div>
            </div>
          </div>
        </div>

        <section className="content">
          <div className="container-fluid">
            <div className="row">
              <Outlet></Outlet>
            </div>
          </div>
        </section>
      </div>
      <footer className="main-footer">
        <strong>
          Copyright &copy; 2014-2025{" "}
          <a href="/">TradeCloud</a>.
        </strong>
        All rights reserved.
        <div className="float-right d-none d-sm-inline-block">
          <b>Version</b> 1.0.0
        </div>
      </footer>

      <aside className="control-sidebar control-sidebar-dark"></aside>
      </div>
    </>
  );
}

export default BaseLayout;
