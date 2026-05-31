import React from 'react';

export default function Profile() {
  // Mock data representing a logged-in user (Future Google Auth payload)
  const mockUser = {
    name: "Ragnar Lothbrok",
    email: "ragnar@valhalla.com",
    avatar: "https://api.dicebear.com/7.x/bottts/svg?seed=viking"
  };

  return (
    <div style={{ padding: '20px', maxWidth: '400px', margin: '0 auto' }}>
      <h2>User Profile</h2>
      <hr />
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <img 
          src={mockUser.avatar} 
          alt="Avatar" 
          style={{ width: '100px', borderRadius: '50%', backgroundColor: '#222' }} 
        />
        <h3>{mockUser.name}</h3>
        <p style={{ color: '#888' }}>{mockUser.email}</p>
      </div>
    </div>
  );
}