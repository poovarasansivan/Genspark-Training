import './App.css';

function App() {
  return (
    <div className='home'>
      <div className='home__container'>
        <h1>Welcome to the Dockerized React App</h1>
        <p>This application is running inside a Docker container.</p>
        <p>Feel free to explore and modify the code!</p>
        <p>Check the Dockerfile and docker-compose.yml for more details.</p>
        <p>Enjoy your Docker journey!</p>
        <p>Current time: {new Date().toLocaleTimeString()}</p>
        <p>Current date: {new Date().toLocaleDateString()}</p>
      </div>
    </div>
  );
}

export default App;
