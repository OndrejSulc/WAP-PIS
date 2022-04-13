function createLoginJSON(login, pw){
    return {
    "Login": login,
    "Password": pw,
    "Successful_Authentication": null,
    "IsCEO": null
    }
};

async function Login()
{
    var login = document.getElementById("login_login").value;
    var pw = document.getElementById("login_pw").value;
    var json = createLoginJSON(login,pw);
    var response = await postLogin(json);

    document.getElementById("result").innerHTML += JSON.stringify(response)+"<br>";
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

async function postLogin(data) {

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
      body: JSON.stringify(data) 
    });
    return await response.json();
}