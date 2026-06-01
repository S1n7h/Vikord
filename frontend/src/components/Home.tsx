import React, { useState, useEffect, useRef } from 'react';


const BASE_URL = "http://localhost:5206";

interface ChatMessage {
  id: number;
  message: string;
  userId?: number; // Notice this is a number type!
}

export default function Home() {
  // Temporary hardcoded messages to test our UI rendering loop
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [message, setMessage] = useState("");
  const [userId, setUserId] = useState("");
  const [isShiftPressed, setIsShiftPressed] = useState(false);

  // Tracks the ID of the message the user is currently hovering over
  const [hoveredMessageId, setHoveredMessageId] = useState<number | null>(null);

  // Create a handle that can point to an HTML element
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  const updateMessage = async () => {
    const Messages = await fetch(`${BASE_URL}/chatlog`);
    const data = await Messages.json();
    console.log(data);
    setMessages(data);      
  };

  const addMessage = async (message: string, userId: string) => {
    //post the game in here
    if (message && userId) {
      const payload = await fetch(`${BASE_URL}/chatlog`, {
        method : "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          "Message" : message,
          "UserId": parseInt(userId, 10)
        })
      })  
      const data = await payload.text();
      console.log("Sent:", data);
    }

    setMessage("");
    setUserId("");

    await updateMessage();
    //scroll down
    setTimeout(() => {
      messagesEndRef.current?.scrollIntoView({ behavior: 'instant' });
    }, 50);
  };

  //you only need the userId of the user cus the delete button is already present
  const deleteMessage = async(id: string, userId: string) => {
    const payload = await fetch(`${BASE_URL}/chatlog/${id}/${userId}`, {
      method : "DELETE",
      headers: {
        "Content-Type": "application/json"
      }
    })  
    const data = await payload.text();
    console.log("Deleted:", data);
    await updateMessage();
  }

  //update messages on refreshing page or any component
  useEffect(() => {
      updateMessage();
    }, []);

    useEffect(() => {
      const handleKeyDown = (e: KeyboardEvent) => {
        if (e.key === 'Shift') setIsShiftPressed(true);
      };

      const handleKeyUp = (e: KeyboardEvent) => {
        if (e.key === 'Shift') setIsShiftPressed(false);
      };

      window.addEventListener('keydown', handleKeyDown);
      window.addEventListener('keyup', handleKeyUp);

      // Clean up listeners when the component unmounts
      return () => {
        window.removeEventListener('keydown', handleKeyDown);
        window.removeEventListener('keyup', handleKeyUp);
      };
    }, []);

  return (
    <div style={{ border: '1px solid #444444', padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
      <h2>Viking Chatroom</h2>
      <hr />
      
      {/* 1. Chat Feed Loop */}
      <div style={{ border: '1px solid #444444', height: '300px', overflowY: 'scroll', 
                    padding: '10px', margin: '20px 0', borderRadius: '6px' }}>
        {messages.map((msg) => (
          <div key={msg.id} 

          //Set the ID when mouse moves over this item
          onMouseEnter={() => setHoveredMessageId(msg.id)}

          //Clear it out when the mouse leaves
          onMouseLeave={() => setHoveredMessageId(null)}
          
          style={{ display: 'flex', justifyContent: 'space-between', background: '#222', 
                                      padding: '8px', margin: '8px 0', borderRadius: '4px' }}>
            <div>
              <strong color='#c0d8ba'>User {msg.userId} </strong>
              <span>{msg.message}</span>
            </div>
            {isShiftPressed && hoveredMessageId === msg.id && msg.userId === parseInt(userId, 10) && (
              <button style={{ background: '#cf6679', color: '#fff', border: 'none', 
                              padding: '2px 8px', borderRadius: '4px', cursor: 'pointer' }}
                              onClick={() => deleteMessage(msg.id.toString(), userId)}>
              Delete
            </button>)}            
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>

      {/* 2. Message Creation Input Box */}
      <div style={{ display: 'flex', gap: '10px' }}>
        <input 
          type="text" 
          placeholder="Type a Viking message..." 
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          style={{ flexGrow: 1, padding: '8px', borderRadius: '4px', border: '1px solid #555', background: '#111', color: '#fff' }}
        />
        <input 
          type="text" 
          placeholder="Enter User Id..." 
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          style={{ flexGrow: 1, padding: '8px', borderRadius: '4px', border: '1px solid #555', background: '#111', color: '#fff' }}
        />
        <button onClick={() => addMessage(message, userId)}
          color = '#5b48c9'>
          Send
        </button>
      </div>
    </div>
  );
}