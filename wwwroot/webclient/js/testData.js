var loggedInID = "1";

var exampleMeetings = { meetings: [
    {
        ID: "1",
        From: "20220408T0800",
        Until: "20220408T0900",
        Title: "Test meeting OZNUK session",
        Description: "Atos Portos Divočákos",
        Owner: "1"
    },

    {
        ID: "2",
        From: "20220508T1000",
        Until: "20220508T1100",
        Title: "Test meeting OZNUK session num 2",
        Description: "Atos Portos Divočákos",
        Owner: "1"
    },

    {
        ID: "3",
        From: "20220508T1100",
        Until: "20220508T1200",
        Title: "Group meeting Test meeting OZNUK session num 3",
        Description: "Atos Portos Divočákos",
        Owner: "2" // id není přihlášeného uživatele => jedná se o group meeting
    },

    {
        ID: "4",
        From: "20220508T1200",
        Until: "20220508T1300",
        Title: "Test meeting OZNUK session num 3",
        Description: "Atos Portos Divočákos",
        Owner: "1"
    },

    {
        ID: "5",
        From: "20220508T1200",
        Until: "20220508T1300",
        Title: "Test meeting OZNUK session num 3",
        Description: "Atos Portos Divočákos",
        Owner: "1"
    }
]}; 