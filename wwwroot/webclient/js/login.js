var customLoginJson = {
    "Login": "LOGIN",
    "Password": "Password",
    "Successful_Authentication": null
};



async function CustomLogin()
{
    document.getElementById("result").innerHTML += JSON.stringify((await Login()))+"<br>";
}

async function CheckLogin()
{
    var response = await fetch("https://localhost:7223/Authentication/CheckLogin");
    document.getElementById("result").innerHTML += await response.text()+"<br>";
}

async function Logout()
{
    var response = await fetch("https://localhost:7223/Authentication/Logout",{method:'POST'});
    document.getElementById("result").innerHTML += await response.text() +"<br>";
}




async function Login(url = "https://localhost:7223/Authentication/Login", data = customLoginJson) {
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