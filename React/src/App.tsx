 import { useState,useEffect } from 'react'

import './App.css'

function App() {
  const [users, setUsers] = useState([])
  useEffect(() => {
    fetch('/api/users').then(data=>data.json()).then(data=>setUsers(data))
  },[])
  const loginHandler = () => {
    fetch('/api/account/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email: 'admin@admin', password: 'adminPassword123!' }),
    })
  }
  const logoutHandler = () => {
    fetch('/api/account/logout', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    })
  }
  return (
    <>
     <button onClick={loginHandler}>login</button>
      <button onClick={logoutHandler}>logout</button>
      {users.map((user) => (<div key={user.id}>{user.email}</div>))}
    </>
  )
}

export default App
