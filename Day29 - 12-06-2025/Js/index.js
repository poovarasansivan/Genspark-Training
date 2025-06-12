// function greet(name) {
//     return `Hello, ${name}!`;
// }

// function greet(name, callback) {
//     console.log(`Hello, ${name}!`);
//     callback();
// }

// function reply() {
//     console.log("How are you?");
// }

// greet("Alice", reply);


// Function to fetch and display users from an API using different methods: Callback, Promise, and Async/Await

const output = document.getElementById("output");

function displayUsers(users) {
  output.innerHTML = ""; 
  setTimeout(() => {
    users.forEach((user) => {
      const userDiv = document.createElement("div");
      userDiv.className = "card";
      userDiv.innerHTML = `
        <img src="${user.image}" alt="${user.firstName}">
        <h3>${user.firstName} ${user.lastName}</h3>
        <p><strong>Email:</strong> ${user.email}</p>
        <p><strong>Age:</strong> ${user.age}</p>
      `;
      output.appendChild(userDiv);
    });
  }, 1000); 
}


// Getting a users by using callback function

function getUsersCallback(callback) {
  const xhr = new XMLHttpRequest();
  xhr.open("GET", "https://dummyjson.com/users");
  xhr.onload = function () {
    if (xhr.status === 200) {
      callback(null, JSON.parse(xhr.responseText));
    } else {
      callback("Error fetching users");
    }
  };
  xhr.send();
}

function getUsersCallbackVersion() {
  getUsersCallback((err, data) => {
    if (err) {
      output.innerHTML = `<p style="color:red;">Callback Error: ${err}</p>`;
    } else {
      displayUsers(data.users);
    }
  });
}

// Getting a users by using Promise

function getUsersPromise() {
  return fetch("https://dummyjson.com/users").then((res) => {
    if (!res.ok) throw new Error("Failed to fetch");
    return res.json();
  });
}

function getUsersPromiseVersion() {
  getUsersPromise()
    .then((data) => displayUsers(data.users))
    .catch((err) => {
      output.innerHTML = `<p style="color:red;">Promise Error: ${err.message}</p>`;
    });
}

// Async/Await Version of getting users

async function getUsersAsyncAwaitVersion() {
  try {
    const res = await fetch("https://dummyjson.com/users");
    if (!res.ok) 
      throw new Error("Fetch failed");
    const data = await res.json();
    displayUsers(data.users);
  } catch (err) {
    output.innerHTML = `<p style="color:red;">Async/Await Error: ${err.message}</p>`;
  }
}
