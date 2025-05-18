import { Link } from "react-router-dom";
import AuthService from "../services/Auth.Service";

function Sidebar() {
  const authService = new AuthService();
  return (
    <>
      <nav className="mt-2">
        <ul
          className="nav nav-pills nav-sidebar flex-column nav-compact text-sm nav-child-indent"
          data-widget="treeview"
          role="menu"
          data-accordion="false"
        >
          <li className="nav-item menu-open">
            <a href="/" className="nav-link active">
              <i className="nav-icon fas fa-tachometer-alt"></i>
              <p>Dashboard</p>
            </a>
          </li>
          <li className="nav-item">
            <a href="#" className="nav-link">
              <i className="nav-icon fas fa-copy"></i>
              <p>
                Symbols
                <i className="fas fa-angle-left right"></i>
                <span className="badge badge-info right">6</span>
              </p>
            </a>
            <ul className="nav nav-treeview">
              <li className="nav-item">
                <Link to="/symbols" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Symbol List</p>
                </Link>
              </li>
              <li className="nav-item">
                <Link to="/tracklist" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Track List</p>
                </Link>
              </li>
            </ul>
          </li>
          <li className="nav-item">
            <a href="#" className="nav-link">
              <i className="nav-icon fas fa-briefcase"></i>
              <p>
                Executions<i className="right fas fa-angle-left"></i>
              </p>
            </a>
            <ul className="nav nav-treeview">
              <li className="nav-item">
                <a href="#" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>New</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="#" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>
                    Plugins<i className="right fas fa-angle-left"></i>
                  </p>
                </a>
                <ul className="nav nav-treeview">
                  <li className="nav-item">
                    <a href="#" className="nav-link">
                      <i className="far fa-dot-circle nav-icon"></i>
                      <p>New</p>
                    </a>
                  </li>
                  <li className="nav-item">
                    <a href="#" className="nav-link">
                      <i className="far fa-dot-circle nav-icon"></i>
                      <p>List</p>
                    </a>
                  </li>
                </ul>
              </li>
              <li className="nav-item">
                <a href="#" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>
                    History<i className="right fas fa-angle-left"></i>
                  </p>
                </a>
                <ul className="nav nav-treeview">
                  <li className="nav-item">
                    <Link to="/execution/history" className="nav-link">
                      <i className="far fa-circle nav-icon"></i>
                      <p>My History</p>
                    </Link>
                  </li>
                  <li className="nav-item">
                    <a href="#" className="nav-link">
                      <i className="far fa-dot-circle nav-icon"></i>
                      <p>Others' Executions</p>
                    </a>
                  </li>
                  <li className="nav-item">
                    <a href="#" className="nav-link">
                      <i className="far fa-dot-circle nav-icon"></i>
                      <p>Logs</p>
                    </a>
                  </li>
                </ul>
              </li>
            </ul>
          </li>
          <li className="nav-item">
            <a href="#" className="nav-link">
              <i className="nav-icon fas fa-users"></i>
              <p>
                Users
                <i className="right fas fa-angle-left"></i>
              </p>
            </a>
            <ul className="nav nav-treeview">
              <li className="nav-item">
                <a href="pages/charts/chartjs.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>List</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="pages/charts/flot.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Roles/Permissions</p>
                </a>
              </li>
            </ul>
          </li>
          <li className="nav-item">
            <a href="#" className="nav-link">
              <i className="nav-icon fas fa-wallet"></i>
              <p>
                Trading
                <i className="fas fa-angle-left right"></i>
              </p>
            </a>
            <ul className="nav nav-treeview">
              <li className="nav-item">
                <a href="pages/UI/general.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Wallet</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="pages/UI/general.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>API Keys</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="pages/UI/icons.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Settings</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="pages/UI/buttons.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>History</p>
                </a>
              </li>
              <li className="nav-item">
                <a href="pages/UI/buttons.html" className="nav-link">
                  <i className="far fa-circle nav-icon"></i>
                  <p>Logs</p>
                </a>
              </li>
            </ul>
          </li>
          <li className="nav-header"></li>
          <li className="nav-header"></li>
          <li className="nav-item">
            <a href="./index.html" className="nav-link ">
              <p>Profile</p>
            </a>
          </li>
          <li className="nav-item">
            <a href="#" className="nav-link">
              <i className="nav-icon fas fa-user-alt"></i>
              <p>Yakup Budancamanak</p>
            </a>
          </li>
          <li className="nav-item">
            <a
              href="#"
              onClick={(e) => {
                console.log(e);
                authService.logout_user();
              }}
              className="nav-link"
            >
              <i className="nav-icon fas fa-sign-out-alt"></i>
              <p className="text">Logout</p>
            </a>
          </li>
        </ul>
      </nav>
    </>
  );
}

export default Sidebar;
