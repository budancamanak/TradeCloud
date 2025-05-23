import { useState, useEffect } from "react";
// import $ from "jquery";
import AuthService from "../../services/Auth.Service";
import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import DataTable from "datatables.net-bs4";
import AnalysisActionButton from "../../components/actionButtons/AnalysisActionButton";
// import "../../lib/plugins/jquery/jquery.min";
// import "../../lib/plugins/jquery-ui/jquery-ui.min";
// import "../../lib/plugins/datatables/jquery.dataTables.min";

function ExecutionHistory() {
  const [tickers, setTickers] = useState([]);
  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("AnalysisExecutions/User/1/Info").then((result) => {
      console.log("setting tickers");
      setTickers(result);
    });
  }, []);
  useEffect(() => {
    console.log("init dtable");
    if (tickers && tickers.length > 0) new DataTable("#example1");
  }, [tickers]);

  return (
    <>
      <div className="col-12">
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">
              Symbols that are registered within the system
            </h3>
            <div className="card-tools">
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="collapse"
              >
                <i className="fas fa-minus"></i>
              </button>
            </div>
          </div>
          <div className="card-body">
            <table id="example1" className="table table-bordered table-striped">
              <thead>
                <tr>
                  <th>Id</th>
                  <th>Plugin</th>
                  <th>Symbol</th>
                  <th>Timeframe</th>
                  <th>Status</th>
                  <th>Progress</th>
                  <th>Start Date</th>
                  <th>End Date</th>
                  <th>Execution Count</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {tickers.map((item) => (
                  <tr key={item.id}>
                    <td>{item.id}</td>
                    <td>{item.plugin}</td>
                    <td>{item.ticker}</td>
                    <td style={{ textTransform: "uppercase" }}>
                      {item.timeframe}
                    </td>
                    <td className={`${item.status}`}>{item.status}</td>
                    <td>%{item.progress * 100}</td>
                    <td>{item.startDate}</td>
                    <td>{item.endDate}</td>
                    <td>{item.pluginExecutions?.length}</td>
                    <td>
                      <AnalysisActionButton
                        title={"Actions"}
                        execution={item}
                        onAction={(type, item) => console.log(type, item)}
                      />
                      {/* <div class="btn-group">
                        <button type="button" class="btn btn-default">
                          Action
                        </button>
                        <button
                          type="button"
                          class="btn btn-default dropdown-toggle dropdown-icon"
                          data-toggle="dropdown"
                        >
                          <span class="sr-only">Toggle Dropdown</span>
                        </button>
                        <div class="dropdown-menu" role="menu">
                          <a class="dropdown-item" href="#">
                            Details
                          </a>
                          <a class="dropdown-item" href="#">
                            Restart
                          </a>
                          <a class="dropdown-item" href="#">
                            View in Chart
                          </a>
                          <div class="dropdown-divider"></div>
                          <a class="dropdown-item" href="#">
                            Delete
                          </a>
                        </div>
                      </div> */}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </>
  );
}

export default ExecutionHistory;
