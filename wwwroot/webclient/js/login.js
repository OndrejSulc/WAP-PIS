async function checkLogin()
{
    var response = await fetch("/Authentication/CheckLogin");
    var check_login = await response.json();
    console.log(check_login);
    if(check_login.loggedIn == true){
      window.location = "index.html";
    }
}
    

async function checkLogout()
{
    var response = await fetch("/Authentication/CheckLogin");
    var check_login = await response.json();
    if(check_login.loggedIn == false){
      console.log(check_login);
      window.location = "login.html";
    }
    //console.log(check_login);
}


async function logout()
{
    var response = await fetch("/Authentication/Logout",{method:'POST'});
    var check_logout = await response.text();
    console.log(check_logout);
    if(check_logout == "true"){
      window.location = "login.html";
    }
    
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