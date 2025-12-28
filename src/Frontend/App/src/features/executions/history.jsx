import { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";

import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import DataTable from "datatables.net-bs4";
import AnalysisActionButton from "../../components/actionButtons/AnalysisActionButton";
import "datatables.net-plugins/dataRender/datetime.mjs";
import WebSocketService from "../../services/WebSocket.Service";

function ExecutionHistory() {
  const navigate = useNavigate();
  const [tickers, setTickers] = useState([]);
  const [initialLoadComplete, setInitialLoadComplete] = useState(false);
  const dataTableRef = useRef(null);
  const fetcher = new Fetcher();

  useEffect(() => {
      fetcher.get("AnalysisExecutions/User/Info").then((result) => {
        setTickers(result);
        setInitialLoadComplete(true);

        // Subscribe to running executions
        result.forEach((item) => {
          if (item.status === "Running" || item.status === "Init") {
            WebSocketService.subscribeToExecution(item.id, {
              onProgress: (id, progress) => {
                setTickers((prev) =>
                  prev.map((t) => (t.id === id ? { ...t, progress } : t))
                );
              },
              onStatus: (id, status) => {
                setTickers((prev) =>
                  prev.map((t) => (t.id === id ? { ...t, status } : t))
                );
              },
              onCompleted: (id, success, error) => {
                if (success) ToastUtility.success(`Execution ${id} completed`);
                else ToastUtility.error(`Execution ${id} failed: ${error}`);
                WebSocketService.unsubscribeFromExecution(id);
              },
            });
          }
        });
      });

      return () => {
        tickers.forEach((item) => {
          WebSocketService.unsubscribeFromExecution(item.id);
        });
      };
    }, []);

  useEffect(() => {
    // Only initialize DataTable once after initial load, not on every tickers update
    if (initialLoadComplete && !dataTableRef.current) {
      // Small delay to ensure DOM is ready with the rendered rows
      const timer = setTimeout(() => {
        if (document.querySelector("#example1 tbody tr")) {
          dataTableRef.current = new DataTable("#example1", {
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
        }
      }, 0);

      return () => clearTimeout(timer);
    }
  }, [initialLoadComplete]);

  // Cleanup DataTable on unmount
  useEffect(() => {
    return () => {
      if (dataTableRef.current) {
        dataTableRef.current.destroy();
        dataTableRef.current = null;
      }
    };
  }, []);

const startExecution = async (execution) => {
    const mr = await fetcher.send("AnalysisExecutions", "PATCH", {
      executionId: execution.id,
    });
    if (mr.isSuccess) {
      ToastUtility.success(mr.message);
      // Subscribe to this execution's progress
      WebSocketService.subscribeToExecution(execution.id, {
        onProgress: (id, progress) => {
          setTickers((prev) =>
            prev.map((t) => (t.id === id ? { ...t, progress } : t))
          );
        },
        onStatus: (id, status) => {
          setTickers((prev) =>
            prev.map((t) => (t.id === id ? { ...t, status } : t))
          );
        },
        onCompleted: (id, success, error) => {
          if (success) ToastUtility.success(`Execution ${id} completed`);
          else ToastUtility.error(`Execution ${id} failed: ${error}`);
          WebSocketService.unsubscribeFromExecution(id);
        },
      });
    } else {
      ToastUtility.error(mr.message);
    }
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
