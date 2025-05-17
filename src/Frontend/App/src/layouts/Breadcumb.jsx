import { Link, useLocation } from "react-router-dom";

function Breadcumb() {
  const location = useLocation();
  const pathnames = location.pathname.split("/").filter((x) => x);
  const active_page = () => {
    if (pathnames == null || pathnames.length == 0) return "";
    return pathnames[pathnames.length - 1];
  };
  let breadcrumbPath = "";

  return (
    <>
      <div className="col-sm-6">
        <h1 className="m-0">{active_page()}</h1>
      </div>
      <div className="col-sm-6">
        <ol className="breadcrumb float-sm-right">
          <li className="breadcrumb-item">
            <Link to="/">Home</Link>
          </li>
          {pathnames.map((name, index) => {
            breadcrumbPath += `/${name}`;
            const isLast = index === pathnames.length - 1;
            console.log(pathnames, breadcrumbPath);

            return isLast ? (
              <li className="breadcrumb-item active" key={breadcrumbPath}>
                {" "}
                {name}
              </li>
            ) : (
              <li className="breadcrumb-item" key={breadcrumbPath}>
                {" "}
                / <Link to={breadcrumbPath}>{name}</Link>
              </li>
            );
          })}
        </ol>
      </div>
    </>
  );
}

export default Breadcumb;
