async function createUser(Username,Password, Name, Surname, Date_Of_Birth, IsSecretary, ManagerIDForNewSecretary) {

    var body = {
        "Username":Username,
        "Password":Password,
        "Name":Name,
        "Surname":Surname,
        "Date_Of_Birth":Date_Of_Birth,
        "IsSecretary":IsSecretary,
        "ManagerIDForNewSecretary": ManagerIDForNewSecretary,
        "Status_Message":null,
        "Status":null,
        }


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
      body: JSON.stringify(body) 
    });
    return await response.json();
}


async function deleteUser(UserID) {

    var body = {
        "UserID":UserID,
        "Status_Message":null,
        "Status":null,
        }

    console.log(body);
    const response = await fetch("/UserManagement/DeleteUser", {
      method: 'DELETE', 
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


async function getAllUsers() {

  const response = await fetch("/UserManagement/GetAllUsers", {
    method: 'GET', 
    mode: 'cors',
    cache: 'no-cache',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    redirect: 'follow', 
    referrerPolicy: 'no-referrer', 
 
  });
  return await response.json();
}