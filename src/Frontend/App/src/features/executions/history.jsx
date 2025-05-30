import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import DataTable from "datatables.net-bs4";
import AnalysisActionButton from "../../components/actionButtons/AnalysisActionButton";
import "datatables.net-plugins/dataRender/datetime.mjs";

function ExecutionHistory() {
  const navigate = useNavigate();
  const [tickers, setTickers] = useState([]);
  const [update, setUpdate] = useState(false);
  const fetcher = new Fetcher();

  useEffect(() => {
    setUpdate(false);
    fetcher.get("AnalysisExecutions/User/1/Info").then((result) => {
      setTickers(result);
      setUpdate(true);
    });
  }, []);

  useEffect(() => {
    if (tickers && tickers.length > 0)
      new DataTable("#example1", {
        order: [[0, "desc"]],
        columnDefs: [
          {
            targets: 7,
            render: DataTable.render.datetime("Do MMM YYYY"),
          },
          {
            targets: 6,
            render: DataTable.render.datetime("Do MMM YYYY"),
          },
        ],
      });
  }, [tickers, update]);

  const startExecution = async (execution) => {
    const mr = await fetcher.send("AnalysisExecutions", "PATCH", {
      executionId: execution.id,
    });
    if (mr.isSuccess) ToastUtility.success(mr.message);
    else ToastUtility.error(mr.message);
  };

  const executeAction = (type, execution) => {
    if ("Start" === type || "Restart" === type) {
      startExecution(execution);
      return;
    }
    if ("ViewInChart" === type) {
      navigate(`/execution/${execution.id}/chart`);
      return;
    }
  };

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
                    <td>%{(item.progress * 100).toFixed(2)}</td>
                    <td>{item.startDate}</td>
                    <td>{item.endDate}</td>
                    <td>{item.pluginExecutions?.length}</td>
                    <td>
                      <AnalysisActionButton
                        title={"Actions"}
                        execution={item}
                        onAction={executeAction}
                      />
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
