import { useState, useEffect } from "react";
// import $ from "jquery";
import AuthService from "../../services/Auth.Service";
import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import DataTable from "datatables.net-bs4";
// import "../../lib/plugins/jquery/jquery.min";
// import "../../lib/plugins/jquery-ui/jquery-ui.min";
// import "../../lib/plugins/datatables/jquery.dataTables.min";

function SymbolList() {
  const [tickers, setTickers] = useState([]);
  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("AvailableTickers").then((result) => {
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
          </div>
          <div className="card-body">
            <table id="example1" className="table table-bordered table-striped">
              <thead>
                <tr>
                  <th>Id</th>
                  <th>Name</th>
                  <th>Symbol</th>
                  <th>Exchange</th>
                  <th>Decimal Point</th>
                </tr>
              </thead>
              <tbody>
                {tickers.map((item) => (
                  <tr key={item.id}>
                    <td>{item.id}</td>
                    <td>{item.name}</td>
                    <td>{item.symbol}</td>
                    <td>{item.exchangeName}</td>
                    <td>{item.decimalPoint}</td>
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

export default SymbolList;
