import { useEffect, useState } from "react";

function App() {
  const [msg, setMsg] = useState("");

  useEffect(() => {
    fetch("http://api:5000/api/message")
      .then((res) => res.json())
      .then((data) => setMsg(data.message))
      .catch((err) => console.error(err));
  }, []);

  return (
    <div>
      <h1>{msg || "Loading..."}</h1>
    </div>
  );
}

export default App;
