import React, { useState } from 'react';

export default function Home() {
  // Temporary hardcoded messages to test our UI rendering loop
  const [messages, setMessages] = useState([
    { id: 1, message: "Hello from User One!", userId: 1 },
    { id: 2, message: "Skål! This is User Two checking in.", userId: 2 }
  ]);

  return (
    <div style={{ border: '1px solid #444444', padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
      <h2>Viking Chatroom</h2>
      <hr />
      
      {/* 1. Chat Feed Loop */}
      <div style={{ border: '1px solid #444444', height: '300px', overflowY: 'scroll', padding: '10px', margin: '20px 0', borderRadius: '6px' }}>
        {messages.map((msg) => (
          <div key={msg.id} style={{ display: 'flex', justifyContent: 'space-between', background: '#222', padding: '8px', margin: '8px 0', borderRadius: '4px' }}>
            <div>
              <strong style={{ color: msg.userId === 1 ? '#4ef' : '#f4e' }}>User {msg.userId}: </strong>
              <span>{msg.message}</span>
            </div>
            {/* Delete button (we will wire up the /chatlog/{id}/{userId} call here soon) */}
            <button style={{ background: '#cf6679', color: '#fff', border: 'none', padding: '2px 8px', borderRadius: '4px', cursor: 'pointer' }}>
              Delete
            </button>
          </div>
        ))}
      </div>

      {/* 2. Message Creation Input Box */}
      <div style={{ display: 'flex', gap: '10px' }}>
        <input 
          type="text" 
          placeholder="Type a Viking message..." 
          style={{ flexGrow: 1, padding: '8px', borderRadius: '4px', border: '1px solid #555', background: '#111', color: '#fff' }}
        />
        <input 
          type="text" 
          placeholder="Enter User Id..." 
          style={{ flexGrow: 1, padding: '8px', borderRadius: '4px', border: '1px solid #555', background: '#111', color: '#fff' }}
        />
        <button style={{ padding: '8px 16px', background: '#03dac6', color: '#000', border: 'none', borderRadius: '4px', fontWeight: 'bold' }}>
          Send
        </button>
      </div>
    </div>
  );
}