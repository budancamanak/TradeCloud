export function validateEmail(mail) {
  return /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail);
}



// export default async function fetcher(url, options) {
//   return (await fetch(url, updateOptions(options))).json();
// }



