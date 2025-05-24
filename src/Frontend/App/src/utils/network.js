class Fetcher {
  constructor() {
    this.BASE_URL = process.env.APP_BASE_URL;
  }
  updateOptions = (options) => {
    const update = { ...options };
    if (localStorage.access_token) {
      update.headers = {
        ...update.headers,
        Authorization: `Bearer ${localStorage.access_token}`,
      };
    }
    return update;
  };

  createOptions = (method, data) => {
    let opts = {
      method: method,
      headers: {
        "Content-Type": "application/json",
      },
    };
    if (data) {
      opts["body"] = JSON.stringify(data);
    }
    return opts;
  };

  fetcher = async (url, options) => {
    const finalUrl = this.BASE_URL + url;
    return (await fetch(finalUrl, this.updateOptions(options))).json();
  };

  post = async (url, data) => {
    const finalUrl = this.BASE_URL + url;
    return (
      await fetch(
        finalUrl,
        this.updateOptions(this.createOptions("POST", data))
      )
    ).json();
  };

  send = async (url, method, data) => {
    const finalUrl = this.BASE_URL + url;
    return (
      await fetch(
        finalUrl,
        this.updateOptions(this.createOptions(method, data))
      )
    ).json();
  };

  get = async (url) => {
    const finalUrl = this.BASE_URL + url;
    return (
      await fetch(finalUrl, this.updateOptions(this.createOptions("GET")))
    ).json();
  };
}

export default Fetcher;
