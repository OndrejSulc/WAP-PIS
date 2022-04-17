async function checkLogin()
{
    var response = await fetch("/Authentication/CheckLogin");
    var check_login = await response.text();
    if(check_login == "true"){
      window.location = "index.html";
    }
    //console.log(check_login);
}

async function checkLogout()
{
    var response = await fetch("/Authentication/CheckLogin");
    var check_login = await response.text();
    if(check_login == "false"){
      window.location = "login.html";
    }
    //console.log(check_login);
}


async function logout()
{
    var response = await fetch("/Authentication/Logout",{method:'POST'});
    var logout = await response.text();
    if(logout == "true"){
      window.location = "login.html";
    }
    //console.log(logout);
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

function loginSucced() {
	document.getElementById("sub").innerHTML ="Loading!";
   var element = document.getElementById("myForm");
   element.classList.add("loading");
   setTimeout(() => { document.getElementById("myForm").classList.add("ok");
   	document.getElementById("sub").innerHTML ="Welcome back!";
   	setTimeout(() => { document.getElementById("myForm").classList.remove("loading");
   		setTimeout(() => { document.getElementById("myForm").classList.remove("ok");setTimeout(() => { window.location = "index.html"; }, 50); }, 1000); }, 2000); }, 1000);
}

function loginFailed() {
	document.getElementById("sub").innerHTML ="Loading!";
   var element = document.getElementById("myForm");
   element.classList.add("loading");
   setTimeout(() => { document.getElementById("myForm").classList.add("nok");
   	document.getElementById("sub").innerHTML ="Try it again!";
   	setTimeout(() => { document.getElementById("myForm").classList.remove("loading");
   		setTimeout(() => { document.getElementById("myForm").classList.remove("nok"); }, 1000); }, 2000); }, 1000);
}