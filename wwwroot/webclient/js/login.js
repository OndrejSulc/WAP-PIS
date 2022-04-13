async function checkLogin()
{
    var response = await fetch("/Authentication/CheckLogin");
    document.getElementById("result").innerHTML += await response.text()+"<br>";
}

async function logout()
{
    var response = await fetch("/Authentication/Logout",{method:'POST'});
    document.getElementById("result").innerHTML += await response.text() +"<br>";
}

async function login(login, pw) {

    var body={
        "Login": login,
        "Password": pw,
        "Successful_Authentication": null,
        "IsCEO": null
        }

    const response = await fetch("/Authentication/Login", {
      method: 'POST', 
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json'
      },
      redirect: 'follow', 
      referrerPolicy: 'no-referrer', 
      body: JSON.stringify(body) 
    });

    return await response.json();
}