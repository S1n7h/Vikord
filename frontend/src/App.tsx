import { useState } from 'react';
import Home from './components/Home';
import Profile from './components/Profile';

export default function App() {
  // State to track which page we are looking at: 'home' or 'profile'
  const [currentView, setCurrentView] = useState('home');
  

  return (
    <div style={{ background: '#121212', color: '#e0e0e0', minHeight: '100vh', fontFamily: 'sans-serif' }}>
      
      {/* Dynamic Global Navigation Bar */}
      <nav style={{ display: 'flex', gap: '20px', padding: '15px', background: '#1f1f1f', borderBottom: '1px solid #333' }}>
        <button 
          onClick={() => setCurrentView('home')} 
          style={{ background: 'none', border: 'none', color: currentView === 'home' ? '#03dac6' : '#fff', fontWeight: 'bold', cursor: 'pointer' }}
        >
          Home Chat
        </button>
        <button 
          onClick={() => setCurrentView('profile')} 
          style={{ background: 'none', border: 'none', color: currentView === 'profile' ? '#03dac6' : '#fff', fontWeight: 'bold', cursor: 'pointer' }}
        >
          My Profile
        </button>
      </nav>

      {/* Conditionally Render Content Based on State */}
      <main>
        {currentView === 'home' ? <Home /> : <Profile />}
      </main>

    </div>
  );
}