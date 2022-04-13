function createCreateUserJSON(Username,Password, Name, Surname, Date_Of_Birth, IsSecretary, ManagerIDForNewSecretary){
    return {
    "Username":Username,
    "Password":Password,
    "Name":Name,
    "Surname":Surname,
    "Date_Of_Birth":"0001-01-01T00:00:00",//Date_Of_Birth,
    "IsSecretary":IsSecretary,
    "ManagerIDForNewSecretary": ManagerIDForNewSecretary,
    "Status_Message":null,
    "Status":null,
    }
};

async function CreateUser()
{
    var login = document.getElementById("um_login").value;
    var pw = document.getElementById("um_pw").value;
    var name = document.getElementById("um_name").value;
    var surname = document.getElementById("um_surname").value;
    var dateOfBirth = document.getElementById("um_date_of_birth").value;
    var issecretary = document.getElementById("um_issecretary").checked;
    var managerID = document.getElementById("um_manager_id").value;

    console.log(issecretary);
    var json = createCreateUserJSON(login, pw, name, surname, dateOfBirth, issecretary, managerID);

    console.log(json);

    var response = await postCreateUser(json);
    document.getElementById("result").innerHTML += JSON.stringify(response)+"<br>";
}


async function postCreateUser(data) {

    const response = await fetch("/UserManagement/CreateNewUser", {
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