var customLoginJson = {
    "Login": "user",
    "Password": "password",
    "Successful_Authentication": null,
    "IsCEO": null
};



async function Login()
{
    document.getElementById("result").innerHTML += JSON.stringify((await postLogin()))+"<br>";
}

async function CheckLogin()
{
    var response = await fetch("/Authentication/CheckLogin");
    document.getElementById("result").innerHTML += await response.text()+"<br>";
}

async function Logout()
{
    var response = await fetch("/Authentication/Logout",{method:'POST'});
    document.getElementById("result").innerHTML += await response.text() +"<br>";
}


async function postLogin(url = "/Authentication/Login", data = customLoginJson) {
    // Default options are marked with *
    const response = await fetch(url, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, *cors, same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      redirect: 'follow', // manual, *follow, error
      referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
      body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
  }